const path = require('path');

module.exports = {
	mode: "production",
	entry: './Ogma3/wwwroot/js/src/client.js',
	module: {
		rules: [
			{
				test: /\.tsx?$/,
				use: 'ts-loader',
				exclude: /node_modules/,
			},
		],
	},
	resolve: {
		extensions: ['.tsx', '.ts', '.js'],
	},
	output: {
		filename: "bundle.min.js",
		path: path.resolve(__dirname, 'Ogma3/wwwroot/js/dist/bundles')
	}
};