(function () {
    var fields = {
        query: document.getElementById('query'),
        rating: document.getElementById('rating'),
        sort: document.getElementById('sort'),
    };
    for (var _i = 0, _a = Object.entries(fields); _i < _a.length; _i++) {
        var _b = _a[_i], key = _b[0], val = _b[1];
        console.log(key + " : " + val.nodeValue);
    }
})();
