import { inject } from 'aurelia-framework';
import { VisitorLite, VisitorReportItem } from './../core/models';
import { DialogController } from 'aurelia-dialog';

@inject(DialogController)
export class ConfirmDialog {
  private dialogController: DialogController;
  public message: string;

  constructor(dialogController: DialogController) {
    this.dialogController = dialogController;

    this.message = '';
  }

  public activate(message: string): void {
    this.message = message;
  }

  public cancel(): void {
    this.dialogController.cancel();
  }

  public save(): void {
    this.dialogController.ok();
  }
}
