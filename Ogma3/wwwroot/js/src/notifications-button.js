(async () => {
	const notificationBtn = document.querySelector("[data-notifications]");
	if (!notificationBtn) return;
	
	const { data } = await axios.get(`${notificationBtn.dataset.notifications}/count`);
	
	if ((data ?? 0) > 0) {
		notificationBtn.title = `${data} notifications`;
		const child = document.createElement("span");
		child.innerText = data <= 99
			? data.clamp(0, 99)
			: '99+';
		notificationBtn.appendChild(child);
	}
})();