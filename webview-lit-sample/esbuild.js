#!/usr/bin/env node
const yargs = require('yargs');
const { build } = require('esbuild')
const fs = require('fs');
const { Readable } = require('stream');

const argv = yargs
    .option('watch', {
        alias: 'w',
        description: 'Switch watch mode',
        type: 'boolean',
    })
    .help()
    .alias('help', 'h')
    .argv;

fs.copyFileSync("node_modules/@vscode/webview-ui-toolkit/dist/toolkit.js", "dist/toolkit.js")

build({
    entryPoints: {
        extension: 'build/Extension.js',
        main: 'build/MyContainer.js'
        },
    bundle: true,
    minify: !argv.watch,
    format: 'cjs',
    outdir: 'dist',
    external: ['vscode'],
    platform: 'node',
    sourcemap: true,
    watch: argv.watch
}).catch(() => process.exit(1))