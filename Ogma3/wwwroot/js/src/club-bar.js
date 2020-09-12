(function () {
    let route = document.getElementById('data-route');
    let id = document.getElementById('data-id');
    let btn = document.getElementById('join-btn');
    
    route.remove();
    id.remove();
    
    btn.addEventListener('click', _ => {
        axios.post(route.dataset.route, { clubId: Number(id.dataset.id) })
            .then(res => {
                btn.className = res.data ? 'button leave' : 'button join';
                btn.innerText = res.data ? 'Leave club' : 'Join club';
            })
            .catch(console.error)
    });
})() 