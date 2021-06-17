(_ => {
	const notificationBtn = document.querySelector("[data-notifications]");
	if (!notificationBtn) return;
	fetch(`${notificationBtn.dataset.notifications}/count`)
		.then(res => res.json())
		.then(res => {
			if ((res ?? 0) > 0) {
				notificationBtn.title = `${res} notifications`;
				const child = document.createElement("span");
				child.innerText = res <= 99 
					? res.clamp(0, 99)
					: '99+';
				notificationBtn.appendChild(child);
			}
		});
})();