import { notifyError } from './notifications';
import { autoinject } from 'aurelia-framework';
import { EventAggregator } from 'aurelia-event-aggregator';
import { HttpClient, RequestMessage, HttpResponseMessage } from 'aurelia-http-client';
import {
  AuthenticateUser,
  AuthenticationResult,
  User,
  LoadingEvent,
  UpdatePassword,
  VisitorSearch,
  UserRole,
  VisitorReportItem,
  Visitor,
  AgeGroup,
  VisitorCheckListItem
} from './models';
import { getCredentials, setCredentials, clearCredentials } from './local-storage';

@autoinject
export class VisitorTrackService {
  private client: HttpClient;
  private eventAggregator: EventAggregator;
  private credentials: AuthenticationResult;

  private userRolePromise: Promise<UserRole[]>;

  private ageGroupPromise: Promise<AgeGroup[]>;

  constructor(client: HttpClient, eventAggregator: EventAggregator) {
    const that = this;

    client.configure(cfg => {
      cfg.withBaseUrl('https://visitor-track-func.azurewebsites.net/api/');
      cfg.withHeader('Content-Type', 'application/json');
      cfg.withInterceptor({
        request(message): RequestMessage {
          that.eventAggregator.publish(new LoadingEvent(true));
          message.headers.add('X-Visitor-Track-Token', that.credentials.token || '');
          return message;
        },
        response(message): HttpResponseMessage {
          that.eventAggregator.publish(new LoadingEvent(false));
          return message;
        },
        requestError(error): RequestMessage {
          that.eventAggregator.publish(new LoadingEvent(false));
          throw error;
        },
        responseError(error): HttpResponseMessage {
          that.eventAggregator.publish(new LoadingEvent(false));
          const message =
            error.statusCode === 400
              ? JSON.parse(error.response)
              : 'API error occurred. Please contact System Administrator';
          notifyError(message);
          throw error;
        }
      });
    });

    that.client = client;
    that.eventAggregator = eventAggregator;
    that.credentials = getCredentials();
  }

  public getSignedUser(): User {
    return this.credentials.user;
  }

  public isSignedIn(): boolean {
    return this.credentials.user != undefined;
  }

  public signIn(content: AuthenticateUser): Promise<boolean> {
    return this.client.post('AuthenticateUserHttpTrigger', content).then(result => {
      const payload = JSON.parse(result.response) as AuthenticationResult;
      const adminRole = payload.user.roleName == 'Admin';
      const editorRole = payload.user.roleName == 'Editor';

      payload.user.isAdmin = adminRole;
      payload.user.isEditor = adminRole || editorRole;

      setCredentials(payload);
      this.credentials = payload;

      return true;
    });
  }

  public signOut(): Promise<void> {
    const func = () => {
      this.credentials = {} as AuthenticationResult;
      clearCredentials();
    };
    return Promise.resolve(func());
  }

  public updatePassword(content: UpdatePassword): Promise<boolean> {
    return this.client
      .post(`UpdateUserPasswordHttpTrigger?contextUserId=${this.getSignedUser().id}`, content)
      .then(result => true);
  }

  public resetUserPassword(id: string): Promise<boolean> {
    return this.client
      .get(`ResetUserPasswordHttpTrigger?contextUserId=${this.getSignedUser().id}&entityId=${id}`)
      .then(result => true);
  }

  public insertUser(content: User): Promise<string> {
    return this.client
      .post(`CreateUserHttpTrigger?contextUserId=${this.getSignedUser().id}`, content)
      .then(result => JSON.parse(result.response));
  }

  public updateUser(id: string, content: User): Promise<string> {
    return this.client
      .put(`UpdateUserHttpTrigger?contextUserId=${this.getSignedUser().id}&entityId=${id}`, content)
      .then(result => id);
  }

  public deleteUser(id: string): Promise<boolean> {
    return this.client
      .delete(`DeleteUserHttpTrigger?contextUserId=${this.getSignedUser().id}&entityId=${id}`)
      .then(result => true);
  }

  public getUsers(): Promise<User[]> {
    return this.client.get('GetAllUsersHttpTrigger').then(result => JSON.parse(result.response));
  }

  public getUserRoles(): Promise<UserRole[]> {
    if (!this.userRolePromise) {
      this.userRolePromise = this.client.get('GetUserRolesHttpTrigger').then(result => JSON.parse(result.response));
    }
    return this.userRolePromise;
  }

  public getAgeGroups(): Promise<AgeGroup[]> {
    if (!this.ageGroupPromise) {
      this.ageGroupPromise = this.client.get('GetAgeGroupsHttpTrigger').then(result => JSON.parse(result.response));
    }
    return this.ageGroupPromise;
  }

  public searchVisitors(text: string): Promise<VisitorSearch[]> {
    return this.client.get(`SearchVisitorsHttpTrigger?text=${text || ''}`).then(result => JSON.parse(result.response));
  }

  public getVisitor(id: string): Promise<Visitor> {
    return this.client.get(`GetVisitorHttpTrigger?entityId=${id}`).then(result => JSON.parse(result.response));
  }

  public getReport(): Promise<VisitorReportItem[]> {
    return this.client.get('GetReportHttpTrigger').then(result => JSON.parse(result.response));
  }

  public insertVisitor(content: Visitor): Promise<string> {
    return this.client
      .post(`CreateVisitorHttpTrigger?contextUserId=${this.getSignedUser().id}`, content)
      .then(result => JSON.parse(result.response));
  }

  public updateVisitor(id: string, content: Visitor): Promise<string> {
    return this.client
      .put(`UpdateVisitorHttpTrigger?contextUserId=${this.getSignedUser().id}&entityId=${id}`, content)
      .then(result => id);
  }

  public deleteVisitor(id: string): Promise<boolean> {
    return this.client
      .delete(`DeleteVisitorHttpTrigger?contextUserId=${this.getSignedUser().id}&entityId=${id}`)
      .then(result => true);
  }

  public updateVisitorCheckList(id: string, content: VisitorCheckListItem): Promise<string> {
    return this.client
      .put(`UpdateVisitorCheckListItemHttpTrigger?contextUserId=${this.getSignedUser().id}&entityId=${id}`, content)
      .then(result => id);
  }
}
