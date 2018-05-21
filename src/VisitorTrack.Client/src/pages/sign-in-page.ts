import { VisitorTrackService } from './../core/visitor-track-service';
import { autoinject } from 'aurelia-framework';
import { Router } from 'aurelia-router';
import { AuthenticateUser } from './../core/models';
import { FormValidator } from '../core/form-validator';
import { ValidationControllerFactory, ValidationRules } from 'aurelia-validation';

@autoinject()
export class SignInPage extends FormValidator {
  private router: Router;
  private service: VisitorTrackService;
  public model: AuthenticateUser;

  constructor(router: Router, service: VisitorTrackService, factory: ValidationControllerFactory) {
    super(factory);
    this.router = router;
    this.service = service;
    this.model = {} as AuthenticateUser;
  }

  public async activate(): Promise<void> {
    await this.registerValidation();
  }

  protected registerValidationRules() {
    ValidationRules.ensure('emailAddress')
      .required()
      .email()
      .ensure('password')
      .required()
      .on(this.model);
  }

  public async signIn() {
    const isValid = await this.validate();
    if (!isValid) return;

    await this.service.signIn(this.model);
    this.router.navigate('main/home');
  }
}
