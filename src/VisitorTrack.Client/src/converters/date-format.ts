import * as moment from 'moment';

export class DateFormatValueConverter {
  public toView(date: Date, format: string): string {
    if (!date) {
      return '';
    }
    return moment(date).format(format);
  }
}
