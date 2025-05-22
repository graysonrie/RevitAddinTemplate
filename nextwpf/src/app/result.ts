export type Result<T, E> = { type: "ok"; value: T } | { type: "err"; error: E };

export function ok<T, E = never>(value: T): Result<T, E> {
  return { type: "ok", value };
}

export function err<T = never, E = unknown>(error: E): Result<T, E> {
  return { type: "err", error };
}
