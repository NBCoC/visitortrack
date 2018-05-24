import { VisitorDialog } from './../dialogs/visitor-dialog';
import { VisitorReportItem, User, VisitorCheckListItem } from './../core/models';
import { ComponentAttached, inject } from 'aurelia-framework';
import { VisitorTrackService } from './../core/visitor-track-service';
import { DialogService } from 'aurelia-dialog';
import { Router } from 'aurelia-router';

@inject(VisitorTrackService, DialogService, Router)
export class HomePage implements ComponentAttached {
  private visitorTrackService: VisitorTrackService;
  private dialogService: DialogService;
  private router: Router;
  public reportData: VisitorReportItem[];
  public selectedReport: VisitorReportItem;
  public hasReport: boolean;
  public user: User;

  constructor(visitorTrackService: VisitorTrackService, dialogService: DialogService, router: Router) {
    this.visitorTrackService = visitorTrackService;
    this.dialogService = dialogService;
    this.router = router;
    this.reportData = [];
    this.selectedReport = {} as VisitorReportItem;
    this.hasReport = false;
    this.user = {} as User;
  }

  public async attached(): Promise<void> {
    this.user = this.visitorTrackService.getSignedUser();
    this.reportData = await this.visitorTrackService.getReport();
    this.selectReport(new Date().getMonth() + 1);
  }

  public newVisitor(): void {
    this.dialogService.open({ viewModel: VisitorDialog, model: {} }).whenClosed(result => {
      if (result.wasCancelled) return;
      this.router.navigate(`#/main/visitor-details/${result.output}`);
    });
  }

  public selectReport(monthId: number): void {
    const data = this.reportData.filter(item => item.monthId === monthId);

    this.selectedReport = {
      monthId: monthId,
      month: data.map(x => x.month)[0],
      members: data.map(x => x.members).reduce((a, b) => a.concat(b)),
      visitors: data.map(x => x.visitors).reduce((a, b) => a.concat(b))
    };

    this.hasReport = this.selectedReport.members.length > 0 || this.selectedReport.visitors.length > 0;
  }
}
