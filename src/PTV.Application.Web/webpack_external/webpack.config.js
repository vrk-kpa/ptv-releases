var path = require('path');
var webpack = require('webpack');
var project = require('../project.json');
var InlineEnviromentVariablesPlugin = require('inline-environment-variables-webpack-plugin');

var ROOT_PATH = path.resolve(__dirname);
var appJsPath = path.join(project.webroot, 'js', 'app');

module.exports = getOptions();

function getOptions() {
    var options = {};
    console.log('webpack options init - enviroment: ', process.env.NODE_ENV);
    options.entry = ['babel-polyfill', path.resolve(appJsPath, 'main.js')];

    options.output = {
        path: path.resolve(project.webroot + '/js/build'),
        filename: 'bundle.js'
    };

    if (process.env.NODE_ENV != 'production') {
        options.output.publicPath = 'http://localhost:4000/';
    }

    options.resolve = {
        extensions: ['', '.js'],
        // Lets you import local packages as if you were traversing from the root of ~/js/App directory //
        modules: [appJsPath, 'node_modules']
    };

    options.module = {
        loaders: [
            {
                test: /\.js$/,
                loader: 'babel',
                exclude: /node_modules/,
                query: {
                    presets: ['es2015', 'react', 'stage-0', 'stage-2']
                }
            },
            {
                test: /\.scss$/,
                exclude: /node_modules/,
                loaders: ['style', 'css?sourceMap', 'sass']
            },
            {
                test: /\.scss$/,
                include: path.resolve(__dirname, '../node_modules/sema-ui-components'),
                loaders: ['style', 'css?sourceMap&-minimize&modules&importLoaders=1&localIdentName=[name]__[local]___[hash:base64:5]', 'sass']
            },
            {
                test: /\.jsonx$/,
                loaders: ["json"]
            },
            {
                test: /\.(png|jpg)$/,
                loader: 'url-loader?limit=100000'
            },
            {
                test: /\.(mp4|ogg|svg)$/,
                loader: 'file-loader'
            }
        ]
    };

    options.devtool = 'source-map';

    if (process.env.NODE_ENV != 'production') {
        options.devServer = {
            host: "0.0.0.0",
            port: "4000",
            colors: true,
            historyApiFallback: true,
            hot: true,
            inline: true,
            stats: 'errors-only',
            headers: { "Access-Control-Allow-Origin": "*" }
        };

        options.plugins = [
            new webpack.HotModuleReplacementPlugin()
        ];
    } else {
        options.plugins = [
            new InlineEnviromentVariablesPlugin({ NODE_ENV: 'production' })
        ];
    }


    return options;
}
