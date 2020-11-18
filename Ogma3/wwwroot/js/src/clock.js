(function () {
    document.querySelectorAll('time.timer').forEach(timer => {

        let time = dayjs(timer.datetime);
        
        setInterval(_ => {
            time = time.add(1, 's');
            timer.innerText = time.format('DD.MM.YYYY HH:mm:ss')
        }, 1000);
        
    });
})()