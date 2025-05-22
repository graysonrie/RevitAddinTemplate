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
  resolve: (value: T) => void;
  reject: (error: unknown) => void;
}
const _invokeMap: Record<number, InvokeCallback<unknown>> = {};

export function invoke<T>(cmd: string, args = {}): Promise<T> {
  if (typeof window === "undefined") {
    return Promise.reject(
      new Error("WebView is only available in browser environment")
    );
  }

  const callId = ++_invokeId;
  const message = { callId, cmd, args };

  return new Promise<T>((resolve, reject) => {
    _invokeMap[callId] = {
      resolve: (value) => resolve(value as T),
      reject,
    };
    window.chrome.webview.postMessage(JSON.stringify(message));
  });
}

// Handle all responses
if (typeof window !== "undefined") {
  window.chrome.webview.addEventListener(
    "message",
    (event: { data: string }) => {
      try {
        const response = JSON.parse(event.data);
        const { callId, result, error } = response;
        const callback = _invokeMap[callId];
        if (callback) {
          if (error) callback.reject(error);
          else callback.resolve(result);
          delete _invokeMap[callId];
        }
      } catch (e) {
        console.error("Malformed response", e);
      }
    }
  );
}
