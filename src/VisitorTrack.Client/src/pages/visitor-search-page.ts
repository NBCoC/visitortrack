import { VisitorTrackService } from './../core/visitor-track-service';
import { autoinject } from 'aurelia-framework';
import { VisitorSearch } from '../core/models';

@autoinject()
export class VisitorSearchPage {
  private service: VisitorTrackService;

  public searchResult: VisitorSearch[];

  constructor(service: VisitorTrackService) {
    this.service = service;
    this.searchResult = [];
  }

  public async search(text: string) {
    if (!text) return;
    this.searchResult = await this.service.searchVisitors(text);
  }
}
