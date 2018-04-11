export interface AuthenticateUser {
  emailAddress: string;
  password: string;
}

export interface AuthenticationResult {
  token: string;
  user: User;
}

export interface Lookup {
  id: number;
  name: string;
}

export interface User {
  id: string;
  displayName: string;
  emailAddress: string;
  roleId: number;
  roleName: string;
  isAdmin: boolean;
  isEditor: boolean;
}

export interface UpdatePassword {
  newPassword: string;
  oldPassword: string;
  confirmPassword: string;
}

export interface RecentVisitor {
  id: string;
  fullName: string;
  statusId: number;
  statusName: string;
}

export interface VisitorSearch extends RecentVisitor {
  ageGroupId: number;
  ageGroupName: string;
}

export interface Visitor extends VisitorSearch {
  description: string;
  becameMemberOn: Date;
  firstVisitedOn: Date;
  createdOn: Date;
  kidsAgeGroups: number[];
}

export class IsLoadingEvent {
  private data: boolean;

  constructor(args: boolean) {
    this.data = args;
  }

  public get args() {
    return this.data;
  }
}
