import { autoinject } from 'aurelia-framework';
import { DialogController } from 'aurelia-dialog';

@autoinject()
export class ConfirmDialog {
  private dialogController: DialogController;
  public message: string;

  constructor(dialogController: DialogController) {
    this.dialogController = dialogController;
    this.message = '';
  }

  public activate(message: string) {
    this.message = message;
  }

  public async confirm() {
    await this.dialogController.ok();
  }

  public async cancel() {
    await this.dialogController.cancel();
  }
}
