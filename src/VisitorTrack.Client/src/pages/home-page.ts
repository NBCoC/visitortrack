import { VisitorDialog } from './../dialogs/visitor-dialog';
import { VisitorReportItem, User } from './../core/models';
import { ComponentAttached, inject } from 'aurelia-framework';
import { VisitorTrackService } from './../core/visitor-track-service';
import { DialogService } from 'aurelia-dialog';
import { RecentVisitorsDialog } from '../dialogs/recent-visitors-dialog';
import { Router } from 'aurelia-router';

@inject(VisitorTrackService, DialogService, Router)
export class HomePage implements ComponentAttached {
  private visitorTrackService: VisitorTrackService;
  private dialogService: DialogService;
  private router: Router;
  public reportData: VisitorReportItem[];
  public user: User;

  constructor(visitorTrackService: VisitorTrackService, dialogService: DialogService, router: Router) {
    this.visitorTrackService = visitorTrackService;
    this.dialogService = dialogService;
    this.router = router;
    this.reportData = [];
    this.user = {} as User;
  }

  public async attached(): Promise<void> {
    this.user = this.visitorTrackService.getSignedUser();
    this.reportData = await this.visitorTrackService.getReport();
  }

  public newVisitor(): void {
    this.dialogService.open({ viewModel: VisitorDialog, model: {} }).whenClosed(result => {
      if (result.wasCancelled) return;
      this.router.navigate(`#/main/visitor-details/${result.output}`);
    });
  }

  public viewDetails(monthId: number): void {
    const data = this.reportData.filter(item => item.monthId === monthId);
    this.dialogService.open({ viewModel: RecentVisitorsDialog, model: data }).whenClosed(result => {
      if (result.wasCancelled) return;
      this.router.navigate(`#/main/visitor-details/${result.output}`);
    });
  }
}
