import { VisitorSearch } from './../core/models';
import { autoinject } from 'aurelia-framework';
import { Api } from './../core/api';

@autoinject()
export class SearchPage {
  private api: Api;

  public searchResult: VisitorSearch[];

  constructor(api: Api) {
    this.api = api;
    this.searchResult = [];
  }

  public async search(text: string) {
    this.searchResult = await this.api.searchVisitors(text);
  }
}
