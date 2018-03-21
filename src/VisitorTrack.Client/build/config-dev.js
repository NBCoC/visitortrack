SystemJS.config({
  packages: {
    src: {
      defaultExtension: 'js'
    }
  },
  map: {
    main: 'src/main.js',
    Vue: 'node_modules/vue/dist/vue.js',
    Vuex: 'node_modules/vuex/dist/vuex.js',
    VueRouter: 'node_modules/vue-router/dist/vue-router.js',
    text: 'node_modules/systemjs-plugin-text/text.js',
    axios: 'node_modules/axios/dist/axios.js',
    BulmaCalendar: 'node_modules/bulma-extensions/bulma-calendar/dist/bulma-calendar.js',
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
