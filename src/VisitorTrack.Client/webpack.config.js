'use strict';

const path = require('path');
const Webpack = require('webpack');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const { AureliaPlugin } = require('aurelia-webpack-plugin');
const resolve = filePath => path.resolve(__dirname, filePath);

module.exports = (env, argv) => {
  const isDevMode = argv.mode !== 'production';

  return {
    mode: isDevMode ? 'development' : 'production',
    devtool: isDevMode ? 'eval-source-map' : false,
    entry: {
      bundle: resolve('./node_modules/aurelia-bootstrapper')
    },
    output: {
      path: resolve('./dist'),
      publicPath: '/',
      filename: '[name].js?v=[chunkhash]'
    },
    resolve: {
      extensions: ['.ts', '.js'],
      modules: ['src', 'node_modules'].map(x => path.resolve(x))
    },
    module: {
      rules: [
        { test: /\.ts$/i, use: 'ts-loader' },
        { test: /\.html$/i, use: 'html-loader' },
        { test: /\.css$/i, loader: 'css-loader', issuer: /\.html?$/i },
        {
          test: /\.css$/i,
          loader: ['style-loader', 'css-loader'],
          issuer: /\.[tj]s$/i
        },
        {
          test: /.(ttf|eot|svg|otf|woff)(\?v=[0-9]\.[0-9]\.[0-9])?$/,
          use: [
            {
              loader: 'file-loader',
              options: {
                name: '[name].[ext]',
                outputPath: 'fonts/',
                publicPath: 'dist/fonts'
              }
            }
          ]
        }
      ]
    },
    plugins: [
      new Webpack.DefinePlugin({ DEV_MODE: JSON.stringify(isDevMode) }),
      new AureliaPlugin({ features: { svg: false } }),
      new Webpack.ProvidePlugin({ Promise: 'bluebird' }),
      new CopyWebpackPlugin([{ from: 'src/css/splash.css' }])
    ]
  };
};
