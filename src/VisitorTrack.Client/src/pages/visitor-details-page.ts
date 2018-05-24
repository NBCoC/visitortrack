import { CheckListItemDialog } from './../dialogs/check-list-item-dialog';
import { DialogService } from 'aurelia-dialog';
import { inject } from 'aurelia-framework';
import { VisitorTrackService } from './../core/visitor-track-service';
import { Visitor, User, VisitorCheckListItem } from './../core/models';
import { ConfirmDialog } from '../dialogs/confirm-dialog';
import { Router } from 'aurelia-router';
import { VisitorDialog } from '../dialogs/visitor-dialog';

@inject(Router, DialogService, VisitorTrackService)
export class VisitorDetailsPage {
  private router: Router;
  private dialogService: DialogService;
  private service: VisitorTrackService;
  public user: User;
  public model: Visitor;

  constructor(router: Router, dialogService: DialogService, service: VisitorTrackService) {
    this.router = router;
    this.dialogService = dialogService;
    this.service = service;

    this.user = {} as User;
    this.model = {} as Visitor;
  }

  public async activate(params: any) {
    this.user = this.service.getSignedUser();
    await this.getVisitor(params.id);
  }

  public editVisitor() {
    this.dialogService.open({ viewModel: VisitorDialog, model: this.model }).whenClosed(result => {
      if (result.wasCancelled) {
        return;
      }
      this.getVisitor(this.model.id);
    });
  }

  public deleteVisitor() {
    this.dialogService
      .open({ viewModel: ConfirmDialog, model: `Are you sure you want to delete ${this.model.fullName}?` })
      .whenClosed(result => {
        if (result.wasCancelled) {
          return;
        }
        this.service.deleteVisitor(this.model.id).then(() => this.router.navigate('home'));
      });
  }

  public toggleCheckListItemCompleted(item: VisitorCheckListItem) {
    item.isChecked = !item.isChecked;

    if (!this.user.isEditor) return;

    if (item.completedOn) {
      this.dialogService
        .open({ viewModel: ConfirmDialog, model: `Are you sure you want to uncheck ${item.description}?` })
        .whenClosed(result => {
          if (result.wasCancelled) {
            return;
          }
          this.service.updateVisitorCheckList(this.model.id, item).then(() => this.getVisitor(this.model.id));
        });

      return;
    }

    this.dialogService.open({ viewModel: CheckListItemDialog, model: item }).whenClosed(result => {
      if (result.wasCancelled) {
        return;
      }
      this.getVisitor(this.model.id);
    });
  }

  private async getVisitor(id: string) {
    this.model = await this.service.getVisitor(id);

    this.model.checkList = this.model.checkList.map(item => {
      item.visitorId = this.model.id;
      item.completedBy = this.user.displayName;

      if (item.completedOn) {
        item.isChecked = true;
      }
      return item;
    });
  }
}
