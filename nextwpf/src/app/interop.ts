import { Result, ok, err } from "./result";

declare global {
  interface Window {
    chrome?: {
      webview?: {
        addEventListener(
          type: string,
          listener: (event: { data: string }) => void
        ): void;
        removeEventListener(
          type: string,
          listener: (event: { data: string }) => void
        ): void;
        postMessage(message: string): void;
      };
    };
  }
}

let _invokeId = 0;
interface InvokeCallback<T> {
  resolve: (value: Result<T, string>) => void;
  reject: (error: unknown) => void;
  __expectedType: string;
}
const _invokeMap: Record<number, InvokeCallback<unknown>> = {};

const isWebViewEnvironment = () => {
  return typeof window !== "undefined" && window.chrome?.webview !== undefined;
};

// Type guard for array types
function isArrayOfType<T>(value: unknown, validator: (item: unknown) => item is T): value is T[] {
  return Array.isArray(value) && value.every(validator);
}

// Type guard for string
function isString(value: unknown): value is string {
  return typeof value === 'string';
}

// Validate data against expected type T
function validateType<T>(data: unknown, expectedType: string): data is T {
  if (expectedType === 'string[]') {
    return isArrayOfType(data, isString);
  }
  // Add more type validations as needed
  return true; // Default case, should be handled based on your needs
}

export function invoke<T>(cmd: string, args = {}, mockData?: T, expectedType = 'unknown'): Promise<Result<T, string>> {
  if (!isWebViewEnvironment()) {
    console.warn("WebView functionality is not available in this environment");
    if (mockData) {
      return Promise.resolve(ok(mockData));
    }
    return Promise.resolve(
      err<T, string>("WebView is only available in WPF WebView environment")
    );
  }

  const callId = ++_invokeId;
  const message = {
    command: cmd,
    args: args,
    id: callId.toString(),
  };

  console.log("Sending message:", message);

  return new Promise<Result<T, string>>((resolve, reject) => {
    const callback = {
      resolve: (value:unknown) => resolve(value as Result<T, string>),
      reject,
      __expectedType: expectedType
    };
    _invokeMap[callId] = callback;
    try {
      window.chrome?.webview?.postMessage(JSON.stringify(message));
    } catch (e) {
      resolve(err<T, string>(`Failed to send message: ${e}`));
    }
  });
}

// Handle all responses
if (isWebViewEnvironment()) {
  // Listen for both 'message' and 'webviewmessage' events
  const messageHandler = (event: { data: string }) => {
    console.log("Received message:", event.data);
    try {
      const response = JSON.parse(event.data);

      const { id, data, error } = response;
      if (!id) {
        console.warn("No id in response:", response);
        return;
      }

      const callback = _invokeMap[parseInt(id)];
      if (callback) {
        if (error) {
          callback.resolve(err<unknown, string>(error));
        } else {
          // Handle case where data might be a JSON string that needs parsing
          let parsedData = data;
          if (typeof data === 'string') {
            try {
              parsedData = JSON.parse(data);
            } catch {
              // If it's not valid JSON, keep the original string
              console.log("Data is a regular string, not JSON:", data);
            }
          }

          // Get the expected type from TypeScript
          const expectedType = callback.__expectedType || 'unknown';
          
          // Validate the parsed data against the expected type
          if (validateType<typeof parsedData>(parsedData, expectedType)) {
            callback.resolve(ok(parsedData));
          } else {
            callback.resolve(err<unknown, string>(`Invalid data type. Expected ${expectedType}`));
          }
        }
        delete _invokeMap[parseInt(id)];
      } else {
        console.warn("No callback found for id:", id);
      }
    } catch (e) {
      console.warn("Error processing response:", e);
    }
  };

  try {
    // Add listeners for both event types
    window.chrome?.webview?.addEventListener("message", messageHandler);
    window.chrome?.webview?.addEventListener("webviewmessage", messageHandler);

    // Log when the listeners are set up
    console.log("WebView message listeners initialized");
  } catch (e) {
    console.warn("Failed to initialize WebView message listeners:", e);
  }
} else {
  console.warn("WebView functionality is not available in this environment");
}
