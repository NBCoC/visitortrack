import { Lookup } from './../core/models';
import { autoinject } from 'aurelia-framework';
import { FormValidator } from '../core/form-validator';
import { DialogController } from 'aurelia-dialog';
import { Api } from '../core/api';
import { ValidationControllerFactory, ValidationRules } from 'aurelia-validation';
import { User } from '../core/models';

@autoinject()
export class UserDialog extends FormValidator {
  private dialogController: DialogController;
  private api: Api;
  public model: User;
  public roles: Lookup[];

  constructor(dialogController: DialogController, api: Api, factory: ValidationControllerFactory) {
    super(factory);
    this.dialogController = dialogController;
    this.api = api;
    this.model = {} as User;
    this.roles = [];
  }

  public async activate(model?: User) {
    this.roles = await this.api.getUserRoles();
    this.model = model !== undefined ? Object.assign({}, model) : ({ roleId: 0 } as User);
    await this.registerValidation();
  }

  protected registerValidationRules() {
    ValidationRules.ensure('displayName')
      .required()
      .ensure('emailAddress')
      .required()
      .email()
      .ensure('roleId')
      .required()
      .on(this.model);
  }

  public async save() {
    const isValid = await this.validate();
    if (!isValid) return;

    let id = this.model.id;

    if (this.model.id) {
      await this.api.updateUser(this.model.id, this.model);
    } else {
      id = await this.api.insertUser(this.model);
    }

    this.dialogController.ok(id);
  }

  public async cancel() {
    await this.dialogController.cancel();
  }
}
