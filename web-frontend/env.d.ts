interface ImportMetaEnv {
  readonly GIBZAPP_API_HOST: string;
  readonly GIBZAPP_API_BASE_PATH: string;
  readonly GIBZAPP_API_TIMEOUT: string;
  readonly GIBZAPP_API_PROFILE_PICTURE_ENDPOINT: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
