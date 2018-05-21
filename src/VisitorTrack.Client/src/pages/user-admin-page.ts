import { VisitorTrackService } from './../core/visitor-track-service';
import { notifySuccess } from './../core/notifications';
import { DialogService } from 'aurelia-dialog';
import { autoinject } from 'aurelia-framework';
import { User } from './../core/models';
import { ConfirmDialog } from '../dialogs/confirm-dialog';
import { UserDialog } from '../dialogs/user-dialog';

@autoinject()
export class UserAdminPage {
  private apiService: VisitorTrackService;
  private dialogService: DialogService;
  public users: User[];

  constructor(apiService: VisitorTrackService, dialogService: DialogService) {
    this.apiService = apiService;
    this.dialogService = dialogService;
    this.users = [];
  }

  public async attached() {
    this.users = await this.apiService.getUsers();
  }

  public upsertUser(model?: User) {
    this.dialogService.open({ viewModel: UserDialog, model: model }).whenClosed(result => {
      if (result.wasCancelled) {
        return;
      }
      this.attached();
    });
  }

  public deleteUser(model: User) {
    this.dialogService
      .open({ viewModel: ConfirmDialog, model: `Are you sure you want to delete ${model.displayName}?` })
      .whenClosed(result => {
        if (result.wasCancelled) {
          return;
        }
        this.apiService.deleteUser(model.id).then(() => this.attached());
      });
  }

  public resetPassword(model: User) {
    this.dialogService
      .open({ viewModel: ConfirmDialog, model: `Are you sure you want to reset ${model.displayName}'s password?` })
      .whenClosed(result => {
        if (result.wasCancelled) {
          return;
        }
        this.apiService.resetUserPassword(model.id).then(() => {
          notifySuccess(`${model.displayName}'s password has been reset!`);
        });
      });
  }
}
