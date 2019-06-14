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
const CaseSensitivePathsPlugin = require('case-sensitive-paths-webpack-plugin')
const CircularDependencyPlugin = require('circular-dependency-plugin')
const BundleAnalyzerPlugin = require('webpack-bundle-analyzer').BundleAnalyzerPlugin
const ContextReplacementPlugin = require('webpack').ContextReplacementPlugin
const EnvironmentPlugin = require('webpack').EnvironmentPlugin

const appJsPath = path.join(__dirname, './wwwroot/js/app')
const utilComponents = path.join(appJsPath, 'util')
const appComponents = path.join(appJsPath, 'appComponents')
const routeComponents = path.join(appJsPath, 'Routes')
const configComponents = path.join(appJsPath, 'Configuration')
const bundelJsPath = path.join(__dirname, './wwwroot/js/build')
const semaUiComponents = path.join(__dirname, './node_modules/sema-ui-components')

module.exports = env => {
  const plugins = [
    // Replaces `process.ENV_NODE` with string value of `nodeEnv` //
    new EnvironmentPlugin({ NODE_ENV: 'production' }),
    // Enforces case sensitive paths in Webpack requires //
    new CaseSensitivePathsPlugin(),
    // Fails build if bundle contains circular dependencies //
    new CircularDependencyPlugin({
      exclude: /node_modules/,
      failOnError: true
    }),
    // Only english locale for momentjs //
    new ContextReplacementPlugin(/moment[/\\]locale$/, /en|fi|sv/)
  ]
  if (env && env.analyze) {
    plugins.push(
      new BundleAnalyzerPlugin({
        analyzerMode: 'server',
        analyzerPort: 8888,
        openAnalyzer: true
      })
    )
  }
  return {
    context: appJsPath,
    mode: 'production',
    entry: {
      ptv: [
        'babel-polyfill',
        './main.js'
      ]
    },
    output: {
      path: bundelJsPath,
      filename: '[name].bundle.js',
      publicPath: 'http://localhost:4000/'
    },
    module: {
      rules: [
        {
          test: /\.js$/,
          loader: 'babel-loader',
          exclude: /node_modules/,
          query: {
            presets: [
              'env',
              'react',
              'stage-0'
            ]
          }
        },
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
          exclude: [
            /node_modules/,
            utilComponents,
            appComponents,
            routeComponents,
            configComponents
          ],
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
          include: [
            semaUiComponents,
            utilComponents,
            appComponents,
            routeComponents,
            configComponents
          ],
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
        }
      ]
    },
    resolve: {
      extensions: ['.js'],
      modules: [
        'node_modules',
        appJsPath
      ]
    },
    plugins
  }
}
