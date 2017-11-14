/**
* The MIT License
* Copyright (c) 2016 Population Register Centre (VRK)
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
const path = require('path')
const webpack = require('webpack')
const appJsPath = path.join(__dirname, './wwwroot/js/app')
const utilComponents = path.join(appJsPath, 'util')
const appComponents = path.join(appJsPath, 'appComponents')
const routeComponents = path.join(appJsPath, 'Routes')
const configComponents = path.join(appJsPath, 'Configuration')
const bundelJsPath = path.join(__dirname, './wwwroot/js/build')
const semaUiComponents = path.join(__dirname, './node_modules/sema-ui-components')
const svgInlineReplacer = path.join(__dirname, './webpack_external/svgInlineReplaceLoader.js')
const urlInlineReplacer = path.join(__dirname, './webpack_external/urlInlineReplaceLoader.js')
var CaseSensitivePathsPlugin = require('case-sensitive-paths-webpack-plugin')

module.exports = (env) => {
  const nodeEnv = env && env.prod ? 'production' : 'development'
  const isProd = nodeEnv === 'production'
  console.log('webpack options init - enviroment: ', nodeEnv)
  process.noDeprecation = true
  const plugins = [
    new webpack.EnvironmentPlugin({
      NODE_ENV: nodeEnv
    })
  ]

  if (isProd) {
    plugins.push(
      new webpack.LoaderOptionsPlugin({
        minimize: true,
        debug: false
      }),
      new webpack.optimize.UglifyJsPlugin({
        compress: {
          warnings: false,
          screw_ie8: true,
          conditionals: true,
          unused: true,
          comparisons: true,
          sequences: true,
          dead_code: true,
          evaluate: true,
          if_return: true,
          join_vars: true
        },
        output: {
          comments: false
        }
      })
    )
  } else {
    plugins.push(
      new webpack.NamedModulesPlugin(),
      new webpack.HotModuleReplacementPlugin()
    )
  }

  // if (env && env.caseSensitiveBuild) {
  plugins.push(new CaseSensitivePathsPlugin())
  // }

  return {
    devtool:'source-map',
    context: appJsPath,
    entry: {
      ptv: ['babel-polyfill', './main.js']
    },
    output: {
      path: bundelJsPath,
      filename: '[name].bundle.js',
      publicPath: 'http://localhost:4000/'
    },
    module: {
      rules: [
		{
		  test: /\.css$/,
		  exclude: /node_modules/,
		  use: [
			{ loader: 'style-loader' },
			{ loader: 'css-loader' }
		  ]
		},
        {
          test: /\.scss$/,
          exclude: [/node_modules/, utilComponents, appComponents, routeComponents, configComponents],
          use: [
            {
              loader: 'style-loader'
            },
            {
              loader: 'css-loader',
              options: {
                sourceMap: 'true'
              }
            },
            {
              loader: 'sass-loader'
            }
          ]
        },
        {
          test: /\.scss$/,
          include: [semaUiComponents, utilComponents, appComponents, routeComponents, configComponents],
          use: [
            {
              loader: 'style-loader'
            },
            {
              loader: 'css-loader',
              options: {
                'sourceMap': 'true',
                '-minimize': 'true',
                'modules': 'true',
                'importLoaders': '1',
                'localIdentName': '[name]__[local]___[hash:base64:5]'
              }
            },
            {
              loader: 'sass-loader'
            }
          ]
        },
        {
          test: /\.(png|jpg)$/,
          use: [
            {
              loader: 'url-loader',
              options: {
                'limit': '100000'
              }
            }
          ]
        },
        {
          test: /\.(mp4|ogg|svg)$/,
          use: [
            {
              loader: 'file-loader',
              query: {
                name: '[name].[ext]'
              }
            }
          ]
        },
        {
          test: /\.js$/,
          enforce: 'pre',
          use: [{
            loader: 'svg-inline-replace-loader'
          },
          {
            loader: 'url-inline-replace-loader'
          }
          ]
        },
        {
          test: /\.js$/,
          loader: 'babel-loader',
          exclude: /node_modules/,
          query: {
            presets: ['es2015', 'react', 'stage-0', 'stage-2']
          }
        }
      ]
    },
    resolve: {
      extensions: ['.webpack-loader.js', '.web-loader.js', '.loader.js', '.js', '.jsx'],
      // Lets you import local packages as if you were traversing from the root of ~/js/App directory //
      modules: [
        'node_modules',
        appJsPath
      ]
    },
    plugins,
    resolveLoader: {
      alias: {
        'svg-inline-replace-loader': svgInlineReplacer,
        'url-inline-replace-loader': urlInlineReplacer
      }
    },
    stats: {
      colors: {
        green: '\u001b[32m'
      }
    },
    devServer: {
      contentBase: appJsPath,
      host: '0.0.0.0',
      port: '4000',
      compress: isProd,
      historyApiFallback: true,
      hot: !isProd,
      inline: !isProd,
      stats: {
        assets: true,
        children: false,
        chunks: false,
        hash: false,
        modules: false,
        publicPath: false,
        timings: true,
        version: false,
        warnings: true,
        colors: {
          green: '\u001b[32m'
        }
      },
      headers: { 'Access-Control-Allow-Origin': '*' }
    }
  }
}
