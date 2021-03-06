import { ValidationControllerFactory, ValidationRules } from 'aurelia-validation';
import { DialogController, DialogCloseResult } from 'aurelia-dialog';
import { inject } from 'aurelia-framework';

import { VisitorTrackService } from './../core/visitor-track-service';
import { User, UserRole } from './../core/models';
import { FormValidator } from '../core/form-validator';

@inject(DialogController, VisitorTrackService, ValidationControllerFactory)
export class UserDialog extends FormValidator {
  private dialogController: DialogController;
  private service: VisitorTrackService;
  public model: User;
  public roles: UserRole[];

  constructor(dialogController: DialogController, service: VisitorTrackService, factory: ValidationControllerFactory) {
    super(factory);
    this.dialogController = dialogController;
    this.service = service;
    this.model = {} as User;
    this.roles = [];
  }

  public async activate(model?: User): Promise<void> {
    this.model = model ? Object.assign({}, model) : {} as User;
    this.roles = await this.service.getUserRoles();
    await this.registerValidation();
  }

  protected registerValidationRules(): void {
    ValidationRules.ensure('displayName')
      .required()
      .ensure('emailAddress')
      .required()
      .email()
      .ensure('roleId')
      .required()
      .on(this.model);
  }

  public cancel(): void {
    this.dialogController.cancel();
  }

  public async save(): Promise<void> {
    const isValid = await this.validate();
    if (!isValid) return;

    let id = this.model.id;

    const upsert = id ? this.service.updateUser(id, this.model) : this.service.insertUser(this.model);

    id = await upsert;

    this.dialogController.ok(id);
  }
}
