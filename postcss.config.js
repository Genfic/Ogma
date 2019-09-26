module.exports = {
    map: false,
    plugins: [
        require('autoprefixer')(),
        require('postcss-discard-comments')({ removeAll: true }),
        require('postcss-zindex')(),
        require('css-mqpacker')(),
        require('cssnano')({ preset: 'default' })
    ]
};
