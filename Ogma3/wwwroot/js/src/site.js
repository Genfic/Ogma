function hexToArgb(hex, alpha) {
    let str = hex.trim('#');
    let rgb = str.match(/.{1,3}/g);
    rgb = rgb.map(c => parseInt(c, 16));
    return 'rgba('+rgb[0]+', '+rgb[1]+', '+rgb[2]+', '+alpha+')';
}
 