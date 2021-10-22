(async () => {
	const notificationBtn = document.querySelector("[data-notifications]") as HTMLButtonElement;
	if (!notificationBtn) return;
	
	const res = await fetch(`${notificationBtn.dataset.notifications}/count`);
	const data: number = await res.json();
	
	if ((data ?? 0) > 0) {
		notificationBtn.title = `${data} notifications`;
		
		const child = document.createElement("span");
		
		child.innerText = data <= 99
			? data.clamp(0, 99).toString()
			: '99+';
		
		notificationBtn.appendChild(child);
	}
})();