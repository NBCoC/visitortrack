'use strict';

const path = require('path');
const ExtractTextPlugin = require('extract-text-webpack-plugin');

const resolve = filePath => path.resolve(__dirname, filePath);

module.exports = {
  entry: {
    bundle: resolve('../src/main.js')
  },
  output: {
    path: resolve('../dist'),
    filename: '[name].js?v=[chunkhash]'
  },
  module: {
    rules: [
      {
        test: /\.js$/,
        exclude: /(node_modules)/,
        loader: 'babel-loader',
        query: {
          presets: ['env']
        }
      },
      {
        test: /\.vue$/,
        loader: 'vue-loader'
      },
      {
        test: /\.css$/,
        use: ExtractTextPlugin.extract({
          fallback: 'style-loader',
          use: [
            {
              loader: 'css-loader',
              options: {
                minimize: true
              }
            }
          ]
        })
      },
      {
        test: /.(ttf|otf|eot|svg|woff(2)?)(\?[a-z0-9]+)?$/,
        use: [
          {
            loader: 'file-loader',
            options: {
              name: '[name].[ext]',
              outputPath: 'fonts/',
              publicPath: './fonts'
            }
          }
        ]
      }
    ]
  },
  resolve: {
    alias: {
      vue$: resolve('../node_modules/vue/dist/vue.esm.js'),
      vuex: resolve('../node_modules/vuex/dist/vuex.js'),
      'vue-router': resolve('../node_modules/vue-router/dist/vue-router.js'),
      'material-design-lite': resolve('../node_modules/material-design-lite/dist/material.js'),
      'mdl-selectfield': resolve('../node_modules/mdl-selectfield/dist/mdl-selectfield.js'),
      'dialog-polyfill': resolve('../node_modules/dialog-polyfill/dialog-polyfill.js'),
      axios: resolve('../node_modules/axios/dist/axios.js'),
      bluebird: resolve('../node_modules/bluebird/js/browser/bluebird.core.js'),
      moment: resolve('../node_modules/moment/min/moment.js'),
      chart: resolve('../node_modules/chart.js/dist/Chart.js')
    }
  },
  plugins: [new ExtractTextPlugin('styles.min.css?v=[chunkhash]')]
};
