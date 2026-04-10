export class ApiScope {
  name: string;
  displayName: string;
  description: string;

  constructor(name: string, displayName: string, description: string) {
    this.name = name;
    this.displayName = displayName;
    this.description = description;
  }
}
