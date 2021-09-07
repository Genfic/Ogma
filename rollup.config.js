import typescript from '@rollup/plugin-typescript';

export default [
	{
		input: './Ogma3/wwwroot/js/src/client.js',
		output: {
			file: './Ogma3/wwwroot/js/dist/bundles/bundle.min.js',
			format: 'iife'
		},
		plugins: [typescript()]
	}
]