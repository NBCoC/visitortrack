var Builder = require('systemjs-builder');

var builder = new Builder();

builder.config({
  packages: {
    src: {
      defaultExtension: 'js'
    }
  },
  map: {
    main: 'src/main.js',
    Vue: 'node_modules/vue/dist/vue.min.js',
    Vuex: 'node_modules/vuex/dist/vuex.min.js',
    VueRouter: 'node_modules/vue-router/dist/vue-router.min.js',
    text: 'node_modules/systemjs-plugin-text/text.js',
    axios: 'node_modules/axios/dist/axios.min.js',
    BulmaCalendar: 'node_modules/bulma-extensions/bulma-calendar/dist/bulma-calendar.min.js',
    'plugin-babel': 'node_modules/systemjs-plugin-babel/plugin-babel.js',
    'systemjs-babel-build':
      'node_modules/systemjs-plugin-babel/systemjs-babel-browser.js',
    'babel-preset-env': 'node_modules/babel-preset-env/lib/index.js'
  },
  transpiler: 'plugin-babel',
  meta: {
    'src/*.html': {
      loader: 'text'
    },
    'src/*.js': {
      preset: ['babel-preset-env']
    }
  }
});

builder.buildStatic('main', './dist/bundle.js', {
  minify: true
});
