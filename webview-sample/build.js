const { build } = require('esbuild')

build({
  entryPoints: {
    main: 'build/Main.js',
    panelcontentmain: 'build/PanelContentMain.js'
  },
  bundle: true,
  format: 'cjs',
  outdir: 'dist',
  external: ['vscode'],
  platform: 'node',
  sourcemap: true,
  watch: true
})
