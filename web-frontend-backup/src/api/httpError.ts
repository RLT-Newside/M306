export interface HttpErrorLike {
  status?: number;
  response?: {
    status?: number;
    data?: unknown;
  };
  data?: unknown;
}

export function getHttpStatus(error: unknown): number | undefined {
  const httpError = error as HttpErrorLike | undefined;
  return httpError?.status ?? httpError?.response?.status;
}

export function getHttpErrorData<T>(error: unknown): T | undefined {
  const httpError = error as HttpErrorLike | undefined;
  return (httpError?.response?.data as T | undefined) ?? (httpError?.data as T | undefined);
}
