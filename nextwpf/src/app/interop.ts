import { Result, ok, err } from "./result";

declare global {
  interface Window {
    chrome: {
      webview: {
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
}
const _invokeMap: Record<number, InvokeCallback<unknown>> = {};

export function invoke<T>(cmd: string, args = {}): Promise<Result<T, string>> {
  if (typeof window === "undefined") {
    return Promise.resolve(
      err<T, string>("WebView is only available in browser environment")
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
    _invokeMap[callId] = {
      resolve: (value) => resolve(value as Result<T, string>),
      reject,
    };
    try {
      window.chrome.webview.postMessage(JSON.stringify(message));
    } catch (e) {
      resolve(err<T, string>(`Failed to send message: ${e}`));
    }
  });
}

// Handle all responses
if (typeof window !== "undefined") {
  // Listen for both 'message' and 'webviewmessage' events
  const messageHandler = (event: { data: string }) => {
    console.log("Received message:", event.data);
    try {
      const response = JSON.parse(event.data);

      const { id, data, error } = response;
      if (!id) {
        console.error("No id in response:", response);
        return;
      }

      const callback = _invokeMap[parseInt(id)];
      if (callback) {
        if (error) {
          callback.resolve(err<unknown, string>(error));
        } else {
          callback.resolve(ok(data));
        }
        delete _invokeMap[parseInt(id)];
      } else {
        console.error("No callback found for id:", id);
      }
    } catch (e) {
      console.error("Error processing response:", e);
    }
  };

  // Add listeners for both event types
  window.chrome.webview.addEventListener("message", messageHandler);
  window.chrome.webview.addEventListener("webviewmessage", messageHandler);

  // Log when the listeners are set up
  console.log("WebView message listeners initialized");
}
