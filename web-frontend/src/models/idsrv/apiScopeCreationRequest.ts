import { ApiScope } from './apiScope';

export class ApiScopeCreationRequest extends ApiScope {
  apiResources: string[];

  constructor(name: string, displayName: string, description: string, apiResources: string[]) {
    super(name, displayName, description);
    this.apiResources = apiResources;
  }
}
