let comments_vue = new Vue({
	el: "#comments-container",
	data: {
		body: "",
		thread: null,
		route: null,
		csrf: null,
		threadRoute: null,
		subscribeRoute: null,
		type: null,

		maxLength: null,

		comments: [],
		total: 0,

		// Pagination
		page: null,
		perPage: null,

		// Name of the OP
		opName: null,

		// Auth status
		isAuthenticated: false,
		canLock: false,

		// Subscription status
		isSubscribed: false,

		// Lock status
		isLocked: false,

		highlight: null,
		collapse: JSON.parse(window.localStorage.getItem("collapse-deleted")),

		isReady: false
	},
	methods: {

		// Submit the comment, load comments again, clean textarea
		submit: async function(e) {
			e.preventDefault();

			if (this.body.length >= this.maxLength) return;

			await axios.post(this.route, {
				body: this.body,
				thread: Number(this.thread),
				type: this.type
			}, {
				headers: { "RequestVerificationToken": this.csrf }
			});

			this.highlight = this.total + 1;
			this.page = 1;
			await this.load();
			this.body = "";
		},

		// Load comments for the thread
		load: async function() {
			const params = {
				thread: this.thread,
				page: this.page,
				highlight: this.highlight
			};

			const res = await axios.get(this.route, { params: params });

			this.total = res.data.total;
			this.page = res.data.page ?? this.page;
			this.isAuthenticated = res.headers["x-authenticated"].toLowerCase() === "true";

			this.comments = res.data.elements.map(
				(val, key) => ({
					val,
					key: (res.data.total - (this.page * this.perPage)) + (this.perPage - (key + 1))
				})
			);

			if (this.highlight) {
				this.$nextTick(() => comments_vue.changeHighlight());
			} else {
				this.navigateToPage();
			}

			if (this.isReady) return;
			this.$nextTick(function() {
				this.isReady = true;
			});
		},

		// Handle Enter key input
		enter: async function(e) {
			if (e.ctrlKey) await this.submit(e);
		},

		// Navigate to the previous page
		prevPage: async function() {
			await this.changePage(Math.max(1, this.page - 1));
		},

		// Navigate to the next page
		nextPage: async function() {
			await this.changePage(Math.min(this.page + 1, Math.ceil(this.total / this.perPage)));
		},

		// Navigate to the selected page
		changePage: async function(idx) {
			this.page = idx;
			this.navigateToPage();
			await this.load();
		},

		// Highlights the selected comment and scrolls it into view
		changeHighlight: function(idx = null, e = null) {
			if (e) e.preventDefault();
			this.highlight = idx ?? this.highlight;
			document
				.getElementById(`comment-${this.highlight}`)
				.scrollIntoView({ behavior: "smooth", block: "center", inline: "nearest" });
			history.replaceState(undefined, undefined, `#comment-${this.highlight}`);
		},

		// Navigates to `this.page` page
		navigateToPage: function() {
			const fragment = this.page > 1
				? `#page-${this.page}`
				: window.location.href.split("#")[0];

			history.replaceState(null, null, fragment);

			if (this.highlight) this.highlight = null;
		},

		// Open the report modal
		report: function(id) {
			this.$refs.reportModal.mutId = id;
			this.$refs.reportModal.visible = true;
		},

		// Subscribe or unsubscribe from the thread
		subscribe: function() {
			if (this.isSubscribed) {
				axios.delete(`${this.subscribeRoute}/thread`, {
					data: { threadId: this.thread },
					headers: { "RequestVerificationToken": this.csrf }
				})
					.then(res => this.isSubscribed = res.data)
					.catch(console.error);

			} else {
				axios.post(`${this.subscribeRoute}/thread`, { threadId: this.thread }, {
					headers: { "RequestVerificationToken": this.csrf }
				})
					.then(res => this.isSubscribed = res.data)
					.catch(console.error);
			}
		},

		// Lock or unlock the thread
		lock: async function() {
			if (!this.canLock) return false;
			this.isLocked = (await axios.post(`${this.threadRoute}/lock`, { id: this.thread })).data;
			return this.isLocked;
		}
	},

	computed: {
		comms: function() {
			// Check collapse preference
			if (this.collapse !== true) return this.comments;

			// If `collapse-deleted` is true, collapse the deleted comments
			let o = [];
			let concat = 0;

			for (const c of this.comments) {
				if (!c.val.deletedBy) {
					if (concat !== 0) o.push({ snip: `Removed ${concat} comments.` });
					concat = 0;
					o.push(c);
				} else {
					concat += 1;
				}
			}

			return o;
		}
	},

	async mounted() {
		// get initial data from SSR
		Object.assign(this.$data, ssrData);

		let hash = window.location.hash.split("-");

		if (hash[0] === "#page" && hash[1]) {
			this.page = Math.max(1, Number(hash[1] ?? 1));
		} else if (hash[0] === "#comment" && hash[1]) {
			this.page = 1;
			this.highlight = Number(hash[1]);
		} else {
			this.page = 1;
			history.replaceState(undefined, undefined, "");
		}

		// Subscription status
		this.isSubscribed = (await axios.get(`${this.subscribeRoute}/thread?threadId=${this.thread}`)).data;

		// Lock permissions
		this.canLock = (await axios.get(`${this.threadRoute}/permissions/${this.thread}`)).data.isAllowed;

		// Lock status
		this.isLocked = (await axios.get(`${this.threadRoute}/lock/status/${this.thread}`)).data;

		await this.load();
	}
});

(() => {
	document
		.getElementById("lock-thread")
		?.addEventListener("click", async (e) => {
			let status = await comments_vue.lock();
			e.target.innerText = status === true ? "Unlock" : "Lock";
		});
})();