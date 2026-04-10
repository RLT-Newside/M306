import { idsrvAlova } from '../alova';
import type { ApiScope } from '@/models/idsrv/apiScope';
import type { ApiResource } from '@/models/idsrv/apiResource';
import type { ApiScopeCreationRequest } from '@/models/idsrv/apiScopeCreationRequest';
import type { Client, ClientWithSecrets } from '@/models/idsrv/client';
import type { IdentityRole } from '@/models/idsrv/identityRole';
import type { ApplicationUser } from '@/models/idsrv/applicationUser';
import type { RoleModificationRequest } from '@/models/idsrv/roleModificationRequest';

///// API RESOURCE /////

// Get all API resources
export const getApiResources = () => idsrvAlova.Get<ApiResource[]>('apiResource');

// Create new API resource
export const createApiResource = (apiResource: ApiResource) =>
  idsrvAlova.Post<ApiResource>('apiResource', JSON.stringify(apiResource));

// Delete existing API resource
export const deleteApiResource = (apiResourceName: string) =>
  idsrvAlova.Delete('apiResource', undefined, { params: { name: apiResourceName } });

///// API SCOPE /////

// Get all API scopes
export const getApiScopes = () => idsrvAlova.Get<ApiScope[]>('apiScope');

// Get API scope with given name
export const getApiScope = (apiScopeName: string) => idsrvAlova.Get(`apiScope/${apiScopeName}`);

// Create new API scope
export const createApiScope = (apiScope: ApiScopeCreationRequest) =>
  idsrvAlova.Post<ApiScope>('apiScope', JSON.stringify(apiScope));

// Delete existing API scope
export const deleteApiScope = (apiScopeName: string) =>
  idsrvAlova.Delete('apiScope', undefined, { params: { name: apiScopeName } });

///// CLIENT /////

// Get all clients
export const getClients = () => idsrvAlova.Get<Client[]>('clients');

// Get client with given id
export const getClient = (clientId: string) =>
  idsrvAlova.Get<Client>(`clients/${encodeURIComponent(clientId)}`);

// Create new client
// Create new client
export const createClient = (client: Client) =>
  idsrvAlova.Post<ClientWithSecrets>('clients', JSON.stringify(client));

// Update existing client
export const updateClient = (clientId: string, client: Client) =>
  idsrvAlova.Put<Client>(`clients/${encodeURIComponent(clientId)}`, JSON.stringify(client));

// Delete existing client
export const deleteClient = (clientId: string) =>
  idsrvAlova.Delete(`clients/${encodeURIComponent(clientId)}`);

// Generate new client secret
export const generateClientSecret = (
  clientId: string,
  payload?: { description?: string; expiration?: string }
) =>
  idsrvAlova.Post<{ secret: string; hashedSecret: string; expiration?: string }>(
    `clients/${encodeURIComponent(clientId)}/secrets`,
    payload ? JSON.stringify(payload) : undefined
  );

// Delete client secret
export const deleteClientSecret = (clientId: string, secretHash: string) =>
  idsrvAlova.Delete(
    `clients/${encodeURIComponent(clientId)}/secrets/${encodeURIComponent(secretHash)}`
  );

// Get client secrets (returns hashed versions only)
export const getClientSecrets = (clientId: string) =>
  idsrvAlova.Get<{ hash: string; description?: string; created: string; expiration?: string }[]>(
    `clients/${encodeURIComponent(clientId)}/secrets`
  );

///// ROLE /////

// Get all roles
export const getRoles = () => idsrvAlova.Get<IdentityRole[]>('role');

// Get role with given name
export const getRole = (roleName: string) => idsrvAlova.Get<IdentityRole>(`role/${roleName}`);

// Create new role
export const createRole = (role: Partial<IdentityRole>) =>
  idsrvAlova.Post<IdentityRole>('role', JSON.stringify(role));

// Delete existing role
export const deleteRole = (roleName: string) =>
  idsrvAlova.Delete('role', undefined, { params: { roleName: roleName } });

///// USER /////

// Get all users with pagination and search
export const getUsers = (
  page: number,
  itemsPerPage: number | null = null,
  search = '',
  roles?: string[]
) => {
  const params: Record<string, string | number> = {
    page: page,
  };

  if (itemsPerPage !== null) {
    params.pageSize = itemsPerPage;
  }

  if (search !== '') {
    params.search = search;
  }

  if (roles && roles.length > 0) {
    params.roleIds = roles.join(',');
  }

  return idsrvAlova.Get<{ users: ApplicationUser[]; total: number }>('user', { params });
};

// Get user with given id
export const getUser = (userId: string) => idsrvAlova.Get<ApplicationUser>(`user/${userId}`);

// Update users roles
export const updateUserRoles = (userId: string, roleModificationRequest: RoleModificationRequest) =>
  idsrvAlova.Put<ApplicationUser>(`user/${userId}`, JSON.stringify(roleModificationRequest));

// Delete user
export const deleteUser = (userId: string) => idsrvAlova.Delete(`user/${userId}`);
