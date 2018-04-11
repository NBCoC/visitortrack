import { notifyError } from './notifications';
import { autoinject } from 'aurelia-framework';
import { EventAggregator } from 'aurelia-event-aggregator';
import { HttpClient, RequestMessage, HttpResponseMessage } from 'aurelia-http-client';
import {
  AuthenticateUser,
  AuthenticationResult,
  User,
  IsLoadingEvent,
  UpdatePassword,
  VisitorSearch,
  Lookup
} from './models';
import { getCredentials, setCredentials, clearCredentials } from './local-storage';

@autoinject
export class Api {
  private client: HttpClient;
  private static eventAggregator: EventAggregator;
  private credentials: AuthenticationResult;

  constructor(client: HttpClient, eventAggregator: EventAggregator) {
    const that = this;

    client.configure(cfg => {
      cfg.withBaseUrl('https://visitor-track-func.azurewebsites.net/api/');
      cfg.withHeader('Content-Type', 'application/json');
      cfg.withInterceptor({
        request(message): RequestMessage {
          Api.eventAggregator.publish(new IsLoadingEvent(true));
          message.headers.add('X-Visitor-Track-Token', that.credentials.token || '');
          return message;
        },
        response(message): HttpResponseMessage {
          Api.eventAggregator.publish(new IsLoadingEvent(false));
          return message;
        },
        requestError(error): RequestMessage {
          Api.eventAggregator.publish(new IsLoadingEvent(false));
          throw error;
        },
        responseError(error): HttpResponseMessage {
          Api.eventAggregator.publish(new IsLoadingEvent(false));
          throw error;
        }
      });
    });

    this.client = client;
    Api.eventAggregator = eventAggregator;
    this.credentials = getCredentials();
  }

  public getSignedUser() {
    return this.credentials.user;
  }

  public isSignedIn() {
    return this.credentials.user != undefined;
  }

  public signIn(content: AuthenticateUser) {
    return this.client
      .post('AuthenticateUserHttpTrigger', content)
      .then(result => {
        const payload = JSON.parse(result.response) as AuthenticationResult;
        const adminRole = payload.user.roleName == 'Admin';
        const editorRole = payload.user.roleName == 'Editor';
        payload.user.isAdmin = adminRole;
        payload.user.isEditor = adminRole || editorRole;
        setCredentials(payload);
        this.credentials = payload;
        return true;
      })
      .catch(handleApiError);
  }

  public updatePassword(content: UpdatePassword) {
    return this.client
      .post(`UpdateUserPasswordHttpTrigger?contextUserId=${this.getSignedUser().id}`, content)
      .then(result => true)
      .catch(handleApiError);
  }

  public searchVisitors(text: string) {
    return this.client
      .get(`SearchVisitorsHttpTrigger?text=${text || ''}`)
      .then(result => JSON.parse(result.response))
      .catch(handleApiError);
  }

  public insertUser(content: User) {
    return this.client
      .post(`CreateUserHttpTrigger?contextUserId=${this.getSignedUser().id}`, content)
      .then(result => JSON.parse(result.response))
      .catch(handleApiError);
  }

  public updateUser(id: string, content: User) {
    return this.client
      .put(`UpdateUserHttpTrigger?contextUserId=${this.getSignedUser().id}&entityId=${id}`, content)
      .then(result => true)
      .catch(handleApiError);
  }

  public deleteUser(id: string) {
    return this.client
      .delete(`DeleteUserHttpTrigger?contextUserId=${this.getSignedUser().id}&entityId=${id}`)
      .then(result => true)
      .catch(handleApiError);
  }

  public resetUserPassword(id: string) {
    return this.client
      .get(`ResetUserPasswordHttpTrigger?contextUserId=${this.getSignedUser().id}&entityId=${id}`)
      .then(result => true)
      .catch(handleApiError);
  }

  public getUsers() {
    return this.client
      .get('GetAllUsersHttpTrigger')
      .then(result => JSON.parse(result.response))
      .catch(handleApiError);
  }

  public getUserRoles() {
    return this.client
      .get('GetUserRolesHttpTrigger')
      .then(result => JSON.parse(result.response))
      .catch(handleApiError);
  }

  public signOut() {
    const func = () => {
      this.credentials = {} as AuthenticationResult;
      clearCredentials();
    };
    return Promise.resolve(func());
  }
}

const handleApiError = error => {
  const message =
    error.statusCode === 400 ? JSON.parse(error.response) : 'API error occurred. Please contact System Administrator';
  notifyError(message);
  return Promise.reject(new Error(message));
};
