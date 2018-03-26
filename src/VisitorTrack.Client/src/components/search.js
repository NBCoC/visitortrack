import template from './search.html';

export default {
  template,
  data() {
    return {
      filter: '',
      dataSource: [
        { id: 1, fullName: 'Jose Diaz', statusName: 'Active', ageGroupName: 'Unknown' },
        { id: 2, fullName: 'Liz Howell', statusName: 'Member', ageGroupName: '60+' },
        { id: 3, fullName: 'Tony Starks', statusName: 'Inactive', ageGroupName: '29 - 39' }    
      ]
    };
  }
};
