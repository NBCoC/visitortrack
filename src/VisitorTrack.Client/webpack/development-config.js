const Merge = require('webpack-merge');
const CommonConfig = require('./common-config.js');

module.exports = Merge(CommonConfig, {
  mode: 'development',
  devtool: 'eval-source-map'
});
