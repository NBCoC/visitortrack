import { inject } from 'aurelia-framework';
import { VisitorLite, VisitorReportItem } from './../core/models';
import { DialogController } from 'aurelia-dialog';

@inject(DialogController)
export class RecentVisitorsDialog {
  private dialogController: DialogController;
  public visitors: VisitorLite[];
  public members: VisitorLite[];
  public month: string;

  constructor(dialogController: DialogController) {
    this.dialogController = dialogController;

    this.visitors = [];
    this.members = [];
    this.month = '';
  }

  public activate(data: VisitorReportItem[]): void {
    this.visitors = data.map(x => x.visitors).reduce((a, b) => a.concat(b));
    this.members = data.map(x => x.members).reduce((a, b) => a.concat(b));
    this.month = data.map(x => x.month)[0];
  }

  public close(): void {
    this.dialogController.cancel();
  }

  public itemSelected(id: string): void {
    this.dialogController.ok(id);
  }
}
