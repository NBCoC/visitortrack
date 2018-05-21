export class SortValueConverter {
  public toView(array: any[], propertyName: string, asc?: boolean): any[] {
    if (!array || array.length === 0) {
      return [];
    }

    if (!propertyName) {
      return array;
    }

    if (asc === undefined) {
      asc = true;
    }

    let func = (a: any, b: any) => {
      let condition = asc ? a[propertyName] > b[propertyName] : b[propertyName] > a[propertyName];

      if (condition) {
        return 1;
      }

      condition = asc ? a[propertyName] < b[propertyName] : b[propertyName] < a[propertyName];

      if (condition) {
        return -1;
      }

      return 0;
    };

    return array.sort(func);
  }
}
