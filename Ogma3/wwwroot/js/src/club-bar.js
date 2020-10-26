(function () {
    const route = document.getElementById('data-route');
    const id = document.getElementById('data-id');
    const btn = document.getElementById('join-btn');
    const xcsrf = document.querySelector('[name=__RequestVerificationToken]');

    route.remove();
    id.remove();

    btn.addEventListener('click', _ => {
        axios.post(route.dataset.route,
            {
                ClubId: Number(id.dataset.id)
            }, {
                headers: {"RequestVerificationToken": xcsrf.value}
            }
        )
            .then(res => {
                btn.className = res.data ? 'button max leave' : 'button max join';
                btn.innerText = res.data ? 'Leave club' : 'Join club';
            })
            .catch(console.error)
    });
})() 