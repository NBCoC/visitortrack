import { ValidationControllerFactory, ValidationRules } from 'aurelia-validation';
import { DialogController, DialogCloseResult } from 'aurelia-dialog';
import { inject } from 'aurelia-framework';

import { VisitorTrackService } from './../core/visitor-track-service';
import { VisitorCheckListItem } from './../core/models';
import { FormValidator } from '../core/form-validator';

@inject(DialogController, VisitorTrackService, ValidationControllerFactory)
export class CheckListItemDialog extends FormValidator {
  private dialogController: DialogController;
  private service: VisitorTrackService;
  public model: VisitorCheckListItem;

  constructor(dialogController: DialogController, service: VisitorTrackService, factory: ValidationControllerFactory) {
    super(factory);
    this.dialogController = dialogController;
    this.service = service;
    this.model = {} as VisitorCheckListItem;
  }

  public async activate(model: VisitorCheckListItem): Promise<void> {
    this.model = Object.assign({}, model);
    await this.registerValidation();
  }

  protected registerValidationRules(): void {
    ValidationRules.ensure('completedOn')
      .required()
      .ensure('comment')
      .required()
      .ensure('visitorId')
      .required()
      .on(this.model);
  }

  public cancel(): void {
    this.dialogController.cancel();
  }

  public async save(): Promise<void> {
    const isValid = await this.validate();
    if (!isValid) return;

    const id = await this.service.updateVisitorCheckList(this.model.visitorId, this.model);

    this.dialogController.ok(id);
  }
}
