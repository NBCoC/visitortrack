import { notifySuccess } from './../core/notifications';
import { UpdatePassword, User } from './../core/models';
import { autoinject } from 'aurelia-framework';
import { FormValidator } from '../core/form-validator';
import { ValidationControllerFactory, ValidationRules } from 'aurelia-validation';
import { VisitorTrackService } from '../core/visitor-track-service';

@autoinject()
export class UserProfilePage extends FormValidator {
  private service: VisitorTrackService;
  public model: UpdatePassword;
  public user: User;

  constructor(service: VisitorTrackService, factory: ValidationControllerFactory) {
    super(factory);
    this.service = service;
    this.user = service.getSignedUser();
    this.model = {} as UpdatePassword;
  }

  protected registerValidationRules(): void {
    ValidationRules.ensure('newPassword')
      .required()
      .ensure('oldPassword')
      .required()
      .ensure('confirmPassword')
      .required()
      .satisfies(() => this.model.newPassword === this.model.confirmPassword)
      .withMessage('New and Confirm passwords do not match')
      .on(this.model);
  }

  public async activate(): Promise<void> {
    await this.registerValidation();
  }

  public async save(): Promise<void> {
    const isValid = await this.validate();
    if (!isValid) return;

    await this.service.updatePassword(this.model);
    notifySuccess('Your password has been changed!');
  }
}
