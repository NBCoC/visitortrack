export interface AuthenticateUser {
  emailAddress: string;
  password: string;
}

export interface AuthenticationResult {
  token: string;
  user: User;
}

export interface UserRole {
  key: number;
  value: string;
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

export interface CheckListItem {
  id: string;
  description: string;
  type: string;
}

export interface VisitorCheckListItem extends CheckListItem {
  completedOn?: Date;
  completedBy: string;
  comment: string;
  isChecked: boolean;
  visitorId: string;
}

export interface AgeGroup {
  key: number;
  value: string;
}

export interface VisitorLite {
  id: string;
  fullName: string;
  isActive: boolean;
  isMember: boolean;
  becameMemberOn: Date;
  firstVisitedOn: Date;
}

export interface VisitorSearch extends VisitorLite {
  ageGroupId: number;
  ageGroupName: string;
}

export interface Visitor extends VisitorSearch {
  description: string;
  contactNumber: string;
  emailAddress: string;
  createdOn: Date;
  checkList: VisitorCheckListItem[];
}

export interface VisitorReportItem {
  monthId: number;
  month: string;
  visitors: VisitorLite[];
  members: VisitorLite[];
}

export class LoadingEvent {
  private data: boolean;

  constructor(args: boolean) {
    this.data = args;
  }

  public get isLoading() {
    return this.data;
  }
}
