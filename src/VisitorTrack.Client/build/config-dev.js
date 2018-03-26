SystemJS.config({
  packages: {
    src: {
      defaultExtension: 'js'
    }
  },
  map: {
    main: 'src/main.js',
    vue: 'node_modules/vue/dist/vue.js',
    vuex: 'node_modules/vuex/dist/vuex.min.js',
    'vue-router': 'node_modules/vue-router/dist/vue-router.min.js',
    text: 'node_modules/systemjs-plugin-text/text.js',
    axios: 'node_modules/axios/dist/axios.min.js',
    'plugin-babel': 'node_modules/systemjs-plugin-babel/plugin-babel.js',
    'systemjs-babel-build':
      'node_modules/systemjs-plugin-babel/systemjs-babel-browser.js',
    'babel-preset-env': 'node_modules/babel-preset-env/lib/index.js',
    bluebird: 'node_modules/bluebird/js/browser/bluebird.core.min.js',
    'material-design-lite':
      'node_modules/material-design-lite/dist/material.min.js',
    'dialog-polyfill': 'node_modules/dialog-polyfill/dialog-polyfill.js',
    'mdl-selectfield': 'node_modules/mdl-selectfield/dist/mdl-selectfield.min.js',
    moment: 'node_modules/moment/min/moment.min.js',
    chart: 'node_modules/chart.js/dist/Chart.min.js'
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
