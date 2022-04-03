(async () => {
    const route = document.querySelector("[data-reads]") as HTMLElement;
    const story = document.querySelector("[data-story-id]") as HTMLElement;
    const buttons = [...document.querySelectorAll("button.read-status")] as HTMLButtonElement[];
    const csrf = (document.querySelector("input[name=__RequestVerificationToken]") as HTMLInputElement).value;

    const headers = {
        "RequestVerificationToken": csrf,
        "Content-Type": "application/json"
    };

    const _body = (id: number) => JSON.stringify({
        story: Number(story.dataset.storyId),
        chapter: id
    });

    route.remove();
    story.remove();

    let reads: Array<number> = [];

    await _getStatus();

    const _readOrUnread = (id: number) => reads.includes(id) ? _markUnread(id) : _markRead(id);
    for (const b of buttons) {
        b.addEventListener("click", () => _readOrUnread(Number(b.dataset.id)));
    }

    async function _markRead(id: number) {
        const res = await fetch(route.dataset.reads, {
            method: "post",
            headers: headers,
            body: _body(id)
        });
        const data = await res.json();
        reads = data.read;
        _update();
    }

    async function _markUnread(id: number) {
        const res = await fetch(route.dataset.reads, {
            method: "delete",
            headers: headers,
            body: _body(id)
        });
        const data = await res.json();
        reads = data.read;
        _update();
    }

    function _update() {
        for (const btn of buttons) {
            const read = reads?.includes(Number(btn.dataset.id)) ?? false;

            btn.classList.toggle("active", read);
            btn.querySelector("i").innerText = read ? "visibility" : "visibility_off";
        }
    }

    async function _getStatus() {
        const res = await fetch(`${route.dataset.reads}/${story.dataset.storyId}`);

        reads = await res.json();
        _update();
    }

})();