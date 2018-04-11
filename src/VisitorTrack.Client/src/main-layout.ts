import { ChangePasswordDialog } from './dialogs/change-password-dialog';
import { DialogService } from 'aurelia-dialog';
import { User } from './core/models';
import { Api } from './core/api';
import { autoinject, PLATFORM } from 'aurelia-framework';
import { Router, RouterConfiguration } from 'aurelia-router';

@autoinject()
export class MainLayout {
  private router: Router;
  private dialogService: DialogService;
  private api: Api;
  public user: User;

  constructor(api: Api, dialogService: DialogService) {
    this.api = api;
    this.dialogService = dialogService;
    this.user = api.getSignedUser();
  }

  public async signOut(): Promise<void> {
    await this.api.signOut();
    this.router.navigate('sign-in');
  }

  public changePassword(): void {
    this.dialogService.open({ viewModel: ChangePasswordDialog });
  }

  public configureRouter(config: RouterConfiguration, router: Router): void {
    config.map([
      {
        route: '',
        redirect: 'main/home'
      },
      {
        route: 'home',
        moduleId: PLATFORM.moduleName('pages/home-page'),
        name: 'home',
        title: 'Home',
        adminView: false,
        caseSensitive: true
      },
      {
        route: 'search',
        moduleId: PLATFORM.moduleName('pages/search-page'),
        name: 'search',
        title: 'Search',
        adminView: false,
        caseSensitive: true
      },
      {
        route: 'visitor/:id',
        moduleId: PLATFORM.moduleName('pages/visitor-page'),
        name: 'visitor',
        title: 'Visitor Details',
        adminView: false,
        caseSensitive: true
      },
      {
        route: 'administration/users',
        moduleId: PLATFORM.moduleName('pages/user-admin-page'),
        name: 'users',
        title: 'User Administration',
        adminView: true,
        caseSensitive: true
      }
    ]);

    this.router = router;
  }
}
