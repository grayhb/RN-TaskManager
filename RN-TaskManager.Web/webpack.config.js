/// <binding BeforeBuild='Run - Development' />
"use strict";
const { VueLoaderPlugin } = require('vue-loader');
const path = require('path');
const webpack = require('webpack');
const UglifyJsPlugin = require('uglifyjs-webpack-plugin');

module.exports = {
    mode: 'development',
    //rules: [
    //    {
    //        test: /\.s(c|a)ss$/,
    //        use: [
    //            'vue-style-loader',
    //            'css-loader',
    //            {
    //                loader: 'sass-loader',
    //                // Requires sass-loader@^7.0.0
    //                options: {
    //                    implementation: require('sass'),
    //                    fiber: require('fibers'),
    //                    indentedSyntax: true // optional
    //                },
    //                // Requires sass-loader@^8.0.0
    //                options: {
    //                    implementation: require('sass'),
    //                    sassOptions: {
    //                        fiber: require('fibers'),
    //                        indentedSyntax: true // optional
    //                    },
    //                },
    //            },
    //        ],
    //    },
    //],
    entry: {
        app: './vue-src/app.js',
        admin: './vue-src/admin.js',
    },
    plugins: [
        new VueLoaderPlugin()
    ],
    optimization: {
        minimize: false
    },
    //optimization: {
    //    minimizer: [
    //        new UglifyJsPlugin({
    //            cache: true,
    //            parallel: true,
    //            uglifyOptions: {
    //                compress: false,
    //                ecma: 6,
    //                mangle: true
    //            },
    //            sourceMap: true
    //        })
    //    ]
    //},
    output: {
        publicPath: "/bundles/js/",
        path: path.join(__dirname, '/wwwroot/bundles/js/'),
        filename: '[name].bundle.js'
    },
    module: {
        rules: [
            {
                test: /\.js$/,
                loader: 'babel-loader',
                exclude: /(node_modules)/,
                query: {
                    presets: ['es2017']
                }
            },
            {
                test: /\.css$/,
                loaders: ['style-loader', 'css-loader']
            },
            {
                test: /\.(png|jpg|gif)$/,
                use: {
                    loader: 'url-loader',
                    options: {
                        limit: 8192
                    }
                }
            },
            {
                test: /\.vue$/,
                loader: 'vue-loader'
            },
            {
                test: /\.(woff(2)?|ttf|eot|svg)(\?v=\d+\.\d+\.\d+)?$/,
                use: [
                    {
                        loader: 'file-loader',
                        options: {
                            name: '[name].[ext]',
                            outputPath: 'fonts/'
                        }
                    }
                ]
            }
        ]
    },
    resolve: {
        alias: {
            vue: 'vue/dist/vue.js'
        },
        extensions: ['.js', '.vue']
    },
};  