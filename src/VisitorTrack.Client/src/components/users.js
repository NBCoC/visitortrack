import template from './users.html';

export default {
  template,
  data() {
    return {
      message: 'Users Page',
      dataSource: [
        { id: 1, displayName: 'Jose Diaz', roleName: 'Admin', emailAddress: 'jdiaz@demo.com' },
        { id: 2, displayName: 'Liz Howell', roleName: 'Editor', emailAddress: 'howelll@test.com' },
        { id: 3, displayName: 'Tony Starks', roleName: 'Viewer', emailAddress: 'ts@starks.come' }    
      ]
    };
  }
};
