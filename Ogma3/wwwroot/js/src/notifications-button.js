(_ => {
	const notif = document.querySelector("[data-notifications]");
	fetch(`${notif.dataset.notifications}/count`)
		.then(res => res.json())
		.then(res => {
			if ((res ?? 0) > 0) {
				notif.title = `${res} notifications`;
				const child = document.createElement("span");
				child.innerText = res <= 99 
					? res.clamp(0, 99)
					: '99+';
				notif.appendChild(child);
			}
		});
})();