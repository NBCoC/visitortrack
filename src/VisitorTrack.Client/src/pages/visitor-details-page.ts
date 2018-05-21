import { DialogService } from 'aurelia-dialog';
import { inject } from 'aurelia-framework';
import { VisitorTrackService } from './../core/visitor-track-service';
import { Visitor, User } from './../core/models';
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
    this.model = await this.service.getVisitor(params.id);
  }

  public editVisitor() {
    this.dialogService.open({ viewModel: VisitorDialog, model: this.model }).whenClosed(result => {
      if (result.wasCancelled) {
        return;
      }
      this.service.getVisitor(this.model.id).then(data => (this.model = data));
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
}
