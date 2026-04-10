export class RoleModificationRequest {
  userId: string;
  rolesToBeAdded: string[];
  rolesToBeRemoved: string[];

  constructor(userId: string, rolesToBeAdded: string[], rolesToBeRemoved: string[]) {
    this.userId = userId;
    this.rolesToBeAdded = rolesToBeAdded;
    this.rolesToBeRemoved = rolesToBeRemoved;
  }
}
