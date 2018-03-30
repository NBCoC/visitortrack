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
      axios: resolve('../node_modules/axios/dist/axios.js'),
      bluebird: resolve('../node_modules/bluebird/js/browser/bluebird.core.js'),
      'bulma-calendar': resolve('../node_modules/bulma-calendar/dist/bulma-calendar.js')
    }
  },
  plugins: [new ExtractTextPlugin('styles.min.css?v=[chunkhash]')]
};
