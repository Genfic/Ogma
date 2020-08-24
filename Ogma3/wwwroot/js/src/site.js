function hexToArgb(hex, alpha) {
    let str = hex.trim('#');
    let rgb = str.match(/.{1,3}/g);
    rgb = rgb.map(c => parseInt(c, 16));
    return 'rgba('+rgb[0]+', '+rgb[1]+', '+rgb[2]+', '+alpha+')';
}

Array.prototype.remove = function (element) {
    let idx = this.indexOf(element);
    if (idx > -1) {
        this.splice(idx, 1)
    }
}

Array.prototype.pushUnique = function (element) {
    if (this.includes(element)) return;
    this.push(element);
}