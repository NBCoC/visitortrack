import { Api } from './../core/api';
import { autoinject } from 'aurelia-framework';
import { Router } from 'aurelia-router';
import { AuthenticateUser } from './../core/models';
import { FormValidator } from '../core/form-validator';
import { ValidationControllerFactory, ValidationRules } from 'aurelia-validation';

@autoinject()
export class SignInPage extends FormValidator {
  private router: Router;
  private api: Api;
  public model: AuthenticateUser;

  constructor(router: Router, api: Api, factory: ValidationControllerFactory) {
    super(factory);
    this.router = router;
    this.api = api;
    this.model = {} as AuthenticateUser;
  }

  protected registerValidationRules(): void {
    ValidationRules.ensure('emailAddress')
      .required()
      .email()
      .ensure('password')
      .required()
      .on(this.model);
  }

  public async signIn(): Promise<void> {
    const isValid = await this.validate();
    if (!isValid) return;

    await this.api.signIn(this.model);
    this.router.navigate('main/home');
  }
}
