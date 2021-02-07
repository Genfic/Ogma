(_ => {
    const route = document.querySelector('[data-reads]');
    const story = document.querySelector('[data-story-id]');
    const buttons = [...document.querySelectorAll('button.read-status')];
    const csrf = document.querySelector('input[name=__RequestVerificationToken]').value;
    route.remove();
    story.remove();
    let reads = [];
    _getStatus();
    for (let b of buttons) {
        b.addEventListener('click', _ => _markRead(Number(b.dataset.id)));
    }
    function _markRead(id) {
        fetch(route.dataset.reads, {
            method: 'post',
            headers: {
                "RequestVerificationToken": csrf,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                story: Number(story.dataset.storyId),
                chapter: id
            })
        })
            .then(res => res.json())
            .then(data => {
            reads = data.read;
            _update();
        })
            .catch(console.error);
    }
    function _update() {
        var _a;
        for (let btn of buttons) {
            let read = (_a = reads === null || reads === void 0 ? void 0 : reads.includes(Number(btn.dataset.id))) !== null && _a !== void 0 ? _a : false;
            btn.classList.toggle('active', read);
            btn.querySelector('i').innerText = read ? 'visibility' : 'visibility_off';
        }
    }
    function _getStatus() {
        fetch(route.dataset.reads + '/' + story.dataset.storyId)
            .then(res => {
            if (res.status == 204)
                return [];
            else
                return res.json();
        })
            .then(data => {
            reads = data.read;
            _update();
        })
            .catch(console.error);
    }
})();
//# sourceMappingURL=chapter-reads.js.map