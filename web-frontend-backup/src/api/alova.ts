import { createAlova } from 'alova';
import VueHook from 'alova/vue';
import adapterFetch from 'alova/fetch';

async function parseResponseBody(response: Response): Promise<unknown> {
  const contentType = response.headers.get('content-type') ?? '';
  if (contentType.includes('application/json')) {
    return response.json();
  }

  const text = await response.text();
  return text.length > 0 ? text : null;
}

async function handleApiResponse(response: Response): Promise<unknown> {
  if (!response.ok) {
    const error = new Error(`HTTP ${response.status}: ${response.statusText}`) as Error & {
      status?: number;
      response?: { status?: number; data?: unknown };
      data?: unknown;
    };

    error.status = response.status;

    try {
      const body = await parseResponseBody(response);
      error.data = body;
      error.response = { status: response.status, data: body };
    } catch {
      error.response = { status: response.status };
    }

    throw error;
  }

  if (response.status === 204) {
    return null;
  }

  return parseResponseBody(response);
}

export const authenticationAlova = createAlova({
  baseURL: import.meta.env.GIBZAPP_API_HOST + '/bffw',
  statesHook: VueHook,
  requestAdapter: adapterFetch(),
  responded: (response) => response.json(),
});

export const studentsManualAlova = createAlova({
  baseURL: import.meta.env.GIBZAPP_API_HOST + '/bffw/students-manual/v1/',
  statesHook: VueHook,
  requestAdapter: adapterFetch(),
  responded: (response) => response.json(),
  beforeRequest(method) {
    // Add header for anti-forgery validation.
    method.config.headers['X-CSRF'] = '1';
    method.config.headers['Content-Type'] = 'application/json';
  },
});

export const idsrvAlova = createAlova({
  baseURL: import.meta.env.GIBZAPP_API_HOST + '/bffw/idsrv/v1/',
  statesHook: VueHook,
  requestAdapter: adapterFetch(),
  responded: (response) => handleApiResponse(response),
  beforeRequest(method) {
    // Add header for anti-forgery validation.
    method.config.headers['X-CSRF'] = '1';
    method.config.headers['Content-Type'] = 'application/json';
  },
  cacheFor: null, // disable caching at all
});

export const profilePictureAlova = createAlova({
  baseURL: import.meta.env.GIBZAPP_API_HOST + import.meta.env.GIBZAPP_API_PROFILE_PICTURE_ENDPOINT,
  statesHook: VueHook,
  requestAdapter: adapterFetch(),
  responded: (response) => response.json(),
  beforeRequest(method) {
    // Add header for anti-forgery validation.
    method.config.headers['X-CSRF'] = '1';
    method.config.headers['Content-Type'] = 'application/json';
  },
  cacheFor: null, // disable caching at all
});

export const gibzRallyAlova = createAlova({
  baseURL: import.meta.env.GIBZAPP_API_HOST + '/bffw/rally/v1/',
  // baseURL: import.meta.env.GIBZAPP_API_HOST + '/v1/',
  statesHook: VueHook,
  requestAdapter: adapterFetch(),
  responded: (response) => handleApiResponse(response),
  beforeRequest(method) {
    // Add header for anti-forgery validation.
    method.config.headers['X-CSRF'] = '1';
    method.config.headers['Content-Type'] = 'application/json';
  },
  cacheFor: null, // disable caching at all
});

export const genericAlovaInstance = createAlova({
  statesHook: VueHook,
  requestAdapter: adapterFetch(),
  responded: (response) => response.json(),
  beforeRequest(method) {
    method.config.headers['Content-Type'] = 'application/json';
  },
  cacheFor: null, // disable caching at all
});
