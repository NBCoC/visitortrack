import { autoinject, PLATFORM } from 'aurelia-framework';
import { Redirect, NavigationInstruction, RouterConfiguration, Next, Router, PipelineStep } from 'aurelia-router';
import { VisitorTrackService } from './core/visitor-track-service';

export class App {
  private router: Router;

  public configureRouter(config: RouterConfiguration, router: Router): void {
    config.title = 'Visitor-Track';
    config.addPipelineStep('authorize', AuthorizeStep);
    config.mapUnknownRoutes(PLATFORM.moduleName('pages/404-page'));
    config.fallbackRoute('main/home');
    config.map([
      {
        route: ['', 'main'],
        moduleId: PLATFORM.moduleName('main-layout'),
        name: 'main',
        title: 'Main',
        caseSensitive: true
      },
      {
        route: 'sign-in',
        moduleId: PLATFORM.moduleName('pages/sign-in-page'),
        name: 'sign-in',
        title: 'Sign In',
        caseSensitive: true
      },
      {
        route: '401',
        moduleId: PLATFORM.moduleName('pages/401-page'),
        name: '401',
        title: '401 - Unauthorized',
        caseSensitive: true
      }
    ]);

    this.router = router;
  }
}

@autoinject
class AuthorizeStep implements PipelineStep {
  private service: VisitorTrackService;

  constructor(service: VisitorTrackService) {
    this.service = service;
  }

  public run(navigationInstruction: NavigationInstruction, next: Next): Promise<any> {
    const isSignInRoute = navigationInstruction.config.name === 'sign-in';
    const isSignedIn = this.service.isSignedIn();

    if (isSignInRoute && !isSignedIn) {
      return next();
    }

    if (isSignInRoute && isSignedIn) {
      return next.cancel();
    }

    if (isSignedIn) {
      const user = this.service.getSignedUser();

      let isAdminView = navigationInstruction.getAllInstructions().some(i => (i.config as any).adminView);

      if (isAdminView && !user.isAdmin) {
        return next.cancel(new Redirect('401'));
      }

      return next();
    }

    return next.cancel(new Redirect('sign-in'));
  }
}
