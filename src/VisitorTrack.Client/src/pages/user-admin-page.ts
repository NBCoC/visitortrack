import { notifySuccess } from './../core/notifications';
import { ConfirmDialog } from './../dialogs/confirm-dialog';
import { UserDialog } from './../dialogs/user-dialog';
import { DialogService } from 'aurelia-dialog';
import { autoinject } from 'aurelia-framework';
import { Api } from './../core/api';
import { User } from './../core/models';

@autoinject()
export class UserAdminPage {
  private api: Api;
  private dialogService: DialogService;
  public users: User[];

  constructor(api: Api, dialogService: DialogService) {
    this.api = api;
    this.dialogService = dialogService;
    this.users = [];
  }

  public async attached() {
    this.users = await this.api.getUsers();
  }

  public resetPassword(user: User) {
    this.dialogService
      .open({
        viewModel: ConfirmDialog,
        model: `Are you sure you want to reset the password for user ${
          user.displayName
        }? If so, make sure to notify the user!`
      })
      .whenClosed(result => {
        if (result.wasCancelled) return;
        this.api.resetUserPassword(user.id).then(() => notifySuccess(`Password reset - ${user.displayName}`));
      });
  }

  public deleteUser(user: User) {
    this.dialogService
      .open({
        viewModel: ConfirmDialog,
        model: `Are you sure you want to delete user ${user.displayName}? This cannot be undone!`
      })
      .whenClosed(result => {
        if (result.wasCancelled) return;
        this.api.deleteUser(user.id).then(() => this.attached());
      });
  }

  public upsertUser(user?: User) {
    this.dialogService.open({ viewModel: UserDialog, model: user }).whenClosed(result => {
      if (result.wasCancelled) return;
      this.attached();
    });
  }
}
