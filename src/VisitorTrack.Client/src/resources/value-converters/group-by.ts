export class GroupByValueConverter {
  public toView(array: any[], propertyName: string): any[] {
    if (!array || array.length === 0) {
      return [];
    }

    if (!propertyName) {
      return array;
    }

    let groups = {};

    array.forEach(item => {
      let group = item[propertyName];

      groups[group] = groups[group] || [];

      groups[group].push(item);
    });

    return Object.keys(groups).map(group => {
      return {
        groupName: group,
        items: groups[group]
      };
    });
  }
}
