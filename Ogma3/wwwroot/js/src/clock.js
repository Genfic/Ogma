import dayjs from 'https://cdn.skypack.dev/-/dayjs@v1.10.4-MoS2QVkxh1TZYPgJA5zq/dist=es2020,mode=imports/optimized/dayjs.js';

(function () {
    document.querySelectorAll('time.timer').forEach(timer => {

        let time = dayjs(timer.datetime);
        
        setInterval(_ => {
            time = time.add(1, 's');
            timer.innerText = time.format('DD.MM.YYYY HH:mm:ss')
        }, 1000);
        
    });
})()