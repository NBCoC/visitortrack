import { User } from './core/models';
import { VisitorTrackService } from './core/visitor-track-service';
import { autoinject, PLATFORM } from 'aurelia-framework';
import { Router, RouterConfiguration } from 'aurelia-router';

@autoinject()
export class MainLayout {
  private router: Router;
  private service: VisitorTrackService;
  public user: User;

  constructor(service: VisitorTrackService) {
    this.service = service;
    this.user = service.getSignedUser();
  }

  public async signOut(): Promise<void> {
    await this.service.signOut();
    this.router.navigate('sign-in');
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
        route: 'user-profile',
        moduleId: PLATFORM.moduleName('pages/user-profile-page'),
        name: 'user-profile',
        title: 'Profile',
        adminView: false,
        caseSensitive: true
      },
      {
        route: 'visitor-search',
        moduleId: PLATFORM.moduleName('pages/visitor-search-page'),
        name: 'visitor-search',
        title: 'Visitor Search',
        adminView: false,
        caseSensitive: true
      },
      {
        route: 'visitor-details/:id',
        moduleId: PLATFORM.moduleName('pages/visitor-details-page'),
        name: 'visitor-details',
        title: 'Visitor Details',
        adminView: false,
        caseSensitive: true
      },
      {
        route: 'user-administration',
        moduleId: PLATFORM.moduleName('pages/user-admin-page'),
        name: 'user-admin',
        title: 'User Administration',
        adminView: true,
        caseSensitive: true
      }
    ]);

    this.router = router;
  }
}
