export class Client {
  // Basic Identity
  clientId: string;
  clientName: string;
  clientUri?: string;
  logoUri?: string;
  description?: string;

  // Grant Types and Scopes
  allowedGrantTypes: string[];
  allowedScopes: string[];

  // URIs
  redirectUris: string[];
  postLogoutRedirectUris: string[];
  allowedCorsOrigins: string[];
  frontChannelLogoutUri?: string;
  backChannelLogoutUri?: string;

  // Security
  requirePkce: boolean;
  requireClientSecret: boolean;
  allowPlainTextPkce?: boolean;
  allowOfflineAccess?: boolean;

  // Token Settings
  identityTokenLifetime?: number; // seconds, default 300
  accessTokenLifetime?: number; // seconds, default 3600
  authorizationCodeLifetime?: number; // seconds, default 300
  absoluteRefreshTokenLifetime?: number; // seconds, default 2592000 (30 days)
  slidingRefreshTokenLifetime?: number; // seconds, default 1296000 (15 days)
  refreshTokenUsage?: number; // 0 = ReUse, 1 = OneTimeOnly
  refreshTokenExpiration?: number; // 0 = Sliding, 1 = Absolute
  accessTokenType?: number; // 0 = JWT, 1 = Reference

  // Consent
  requireConsent?: boolean;
  allowRememberConsent?: boolean;
  consentLifetime?: number; // seconds, null = no expiration

  // Logout
  frontChannelLogoutSessionRequired?: boolean;
  backChannelLogoutSessionRequired?: boolean;

  // Device Flow
  userCodeType?: string;
  deviceCodeLifetime?: number; // seconds, default 300

  // Advanced
  alwaysSendClientClaims?: boolean;
  alwaysIncludeUserClaimsInIdToken?: boolean;
  updateAccessTokenClaimsOnRefresh?: boolean;
  enableLocalLogin?: boolean;
  includeJwtId?: boolean;

  // Client Claims (custom claims to include in tokens)
  claims?: { type: string; value: string }[];

  constructor(
    clientId: string,
    clientName: string,
    allowedGrantTypes: string[],
    allowedScopes: string[],
    redirectUris: string[],
    postLogoutRedirectUris: string[],
    allowedCorsOrigins: string[],
    requirePkce: boolean,
    requireClientSecret: boolean
  ) {
    this.clientId = clientId;
    this.clientName = clientName;
    this.allowedGrantTypes = allowedGrantTypes;
    this.allowedScopes = allowedScopes;
    this.redirectUris = redirectUris;
    this.postLogoutRedirectUris = postLogoutRedirectUris;
    this.allowedCorsOrigins = allowedCorsOrigins;
    this.requirePkce = requirePkce;
    this.requireClientSecret = requireClientSecret;

    // Set sensible defaults
    this.requireConsent = false;
    this.allowRememberConsent = true;
    this.allowOfflineAccess = false;
    this.enableLocalLogin = true;
    this.identityTokenLifetime = 300;
    this.accessTokenLifetime = 3600;
    this.authorizationCodeLifetime = 300;
    this.absoluteRefreshTokenLifetime = 2592000;
    this.slidingRefreshTokenLifetime = 1296000;
    this.refreshTokenUsage = 1; // OneTimeOnly
    this.refreshTokenExpiration = 1; // Absolute
    this.accessTokenType = 0; // JWT
    this.frontChannelLogoutSessionRequired = true;
    this.backChannelLogoutSessionRequired = true;
    this.deviceCodeLifetime = 300;
    this.alwaysSendClientClaims = false;
    this.alwaysIncludeUserClaimsInIdToken = false;
    this.updateAccessTokenClaimsOnRefresh = false;
    this.includeJwtId = true;
    this.allowPlainTextPkce = false;
    this.claims = [];
  }
}

export class ClientWithSecrets extends Client {
  clientSecrets: string[];

  constructor(
    clientId: string,
    clientName: string,
    allowedGrantTypes: string[],
    allowedScopes: string[],
    redirectUris: string[],
    postLogoutRedirectUris: string[],
    allowedCorsOrigins: string[],
    requirePkce: boolean,
    requireClientSecret: boolean,
    clientSecrets: string[]
  ) {
    super(
      clientId,
      clientName,
      allowedGrantTypes,
      allowedScopes,
      redirectUris,
      postLogoutRedirectUris,
      allowedCorsOrigins,
      requirePkce,
      requireClientSecret
    );

    this.clientSecrets = clientSecrets;
  }
}

export class AuthenticatedUser {
  username: string;
  email: string;
  surname: string;
  familyname: string;

  constructor(username: string, email: string, surname: string, familyname: string) {
    this.username = username;
    this.email = email;
    this.surname = surname;
    this.familyname = familyname;
  }
}
