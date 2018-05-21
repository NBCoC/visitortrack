export class FilterValueConverter {
  public toView(array: any[], propertyName: string, text: string): any[] {
    if (!array || array.length === 0) {
      return [];
    }

    if (!propertyName || !text) {
      return array;
    }

    text = text.toLocaleLowerCase();

    return array.filter(item => {
      const value = item[propertyName];

      if (!value) return;

      return value.toLocaleLowerCase().indexOf(text) !== -1;
    });
  }
}
