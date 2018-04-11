import { notifySuccess } from './../core/notifications';
import { Api } from './../core/api';
import { UpdatePassword } from './../core/models';
import { autoinject } from 'aurelia-framework';
import { DialogController } from 'aurelia-dialog';
import { FormValidator } from '../core/form-validator';
import { ValidationControllerFactory, ValidationRules } from 'aurelia-validation';

@autoinject()
export class ChangePasswordDialog extends FormValidator {
  private dialogController: DialogController;
  private api: Api;
  public model: UpdatePassword;

  constructor(dialogController: DialogController, api: Api, factory: ValidationControllerFactory) {
    super(factory);
    this.dialogController = dialogController;
    this.api = api;
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
    this.model = {} as UpdatePassword;
    await this.registerValidation();
  }

  public async save(): Promise<void> {
    const isValid = await this.validate();
    if (!isValid) return;

    await this.api.updatePassword(this.model);
    this.dialogController.ok();
    notifySuccess('Your password has been changed!');
  }

  public async cancel(): Promise<void> {
    await this.dialogController.cancel();
  }
}
