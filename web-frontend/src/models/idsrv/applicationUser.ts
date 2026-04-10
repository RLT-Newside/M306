import type { IdentityRole } from './identityRole';

export class ApplicationUser {
  id: string;
  userName: string;
  displayName: string;
  email: string;
  userRoles: IdentityRole[];

  constructor(
    id: string,
    userName: string,
    displayName: string,
    email: string,
    userRoles: IdentityRole[]
  ) {
    this.id = id;
    this.userName = userName;
    this.displayName = displayName;
    this.email = email;
    this.userRoles = userRoles;
  }
}
