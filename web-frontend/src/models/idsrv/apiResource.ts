export class ApiResource {
  name: string;
  displayName: string;
  scopes: string[];

  constructor(name: string, displayName: string, scopes: string[]) {
    this.name = name;
    this.displayName = displayName;
    this.scopes = scopes;
  }
}
