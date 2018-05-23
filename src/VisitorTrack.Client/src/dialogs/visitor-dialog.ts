import { ValidationControllerFactory, ValidationRules } from 'aurelia-validation';
import { DialogController, DialogCloseResult } from 'aurelia-dialog';
import { inject } from 'aurelia-framework';

import { VisitorTrackService } from './../core/visitor-track-service';
import { Visitor, AgeGroup } from './../core/models';
import { FormValidator } from '../core/form-validator';

@inject(DialogController, VisitorTrackService, ValidationControllerFactory)
export class VisitorDialog extends FormValidator {
  private dialogController: DialogController;
  private service: VisitorTrackService;
  public model: Visitor;
  public ageGroups: AgeGroup[];

  constructor(dialogController: DialogController, service: VisitorTrackService, factory: ValidationControllerFactory) {
    super(factory);
    this.dialogController = dialogController;
    this.service = service;
    this.model = {} as Visitor;
    this.ageGroups = [];
  }

  public async activate(model: Visitor): Promise<void> {
    this.model = model ? Object.assign({}, model) : ({} as Visitor);
    this.ageGroups = await this.service.getAgeGroups();
    await this.registerValidation();
  }

  protected registerValidationRules(): void {
    ValidationRules.ensure('fullName')
      .required()
      .ensure('emailAddress')
      .email()
      .ensure('ageGroupId')
      .required()
      .ensure('contactNumber')
      .matches(/\d{3}-\d{3}-\d{4}/)
      .ensure('firstVisitedOn')
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

    const upsert = id ? this.service.updateVisitor(id, this.model) : this.service.insertVisitor(this.model);

    id = await upsert;

    this.dialogController.ok(id);
  }
}
