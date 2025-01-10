import {
	GetApiComments,
	GetApiCommentsThread,
	PostApiComments,
	PostApiCommentsThreadLock,
} from "../generated/paths-public";

// @ts-ignore
new Vue({
	el: "#comments-container",
	data: {
		body: "",
		thread: null,
		csrf: null,
		type: null,

		minLength: 0,
		maxLength: 0,

		comments: [],
		total: 0,

		// Pagination
		page: 1,
		perPage: 10,

		// Name of the OP
		opName: null,

		// Auth status
		authenticatedAs: false,
		canLock: false,

		// Subscription status
		isSubscribed: false,

		// Lock status
		isLocked: false,

		highlight: null,
		collapse: JSON.parse(window.localStorage.getItem("collapse-deleted")),

		isReady: false,
	},
	methods: {
		// Submit the comment, load comments again, clean textarea
		submit: async function (e: Event) {
			e.preventDefault();

			if (this.body.length >= this.maxLength) return;

			const res = await PostApiComments(
				{
					body: this.body,
					thread: Number(this.thread),
					source: this.type,
				},
				{ RequestVerificationToken: this.csrf },
			);

			if (!res.ok) return;

			this.highlight = this.total + 1;
			this.page = 1;
			await this.load();
			this.body = "";
		},

		// Load comments for the thread
		load: async function () {
			const res = await GetApiComments(this.thread, this.page, this.highlight ?? -1);
			const data = res.data;

			this.total = data.total;
			this.page = data.page ?? this.page;
			this.authenticatedAs = res.headers.get("x-username").toLowerCase();

			this.comments = Object.entries(data.elements).map(([key, val]) => ({
				val,
				key: data.total - this.page * this.perPage + (this.perPage - (Number.parseInt(key) + 1)),
			}));

			if (this.highlight) {
				this.$nextTick(() => this.changeHighlight());
			} else {
				this.navigateToPage();
			}

			if (this.isReady) return;
			this.$nextTick(function () {
				this.isReady = true;
			});
		},

		// Handle Enter key input
		enter: async function (e: KeyboardEvent) {
			if (e.ctrlKey) await this.submit(e);
		},

		// Navigate to the previous page
		prevPage: async function () {
			await this.changePage(Math.max(1, this.page - 1));
		},

		// Navigate to the next page
		nextPage: async function () {
			await this.changePage(Math.min(this.page + 1, Math.ceil(this.total / this.perPage)));
		},

		// Navigate to the selected page
		changePage: async function (idx: number) {
			this.page = idx;
			this.navigateToPage();
			await this.load();
		},

		// Highlights the selected comment and scrolls it into view
		changeHighlight: function (idx: number | null = null, e: Event | null = null) {
			if (e) e.preventDefault();
			this.highlight = idx ?? this.highlight;
			document.getElementById(`comment-${this.highlight}`).scrollIntoView({
				behavior: "smooth",
				block: "center",
				inline: "nearest",
			});
			history.replaceState(undefined, undefined, `#comment-${this.highlight}`);
		},

		// Navigates to `this.page` page
		navigateToPage: function () {
			const fragment = this.page > 1 ? `#page-${this.page}` : window.location.href.split("#")[0];

			history.replaceState(null, null, fragment);

			if (this.highlight) this.highlight = null;
		},

		// Open the report modal
		report: function (id: number) {
			this.$refs.reportModal.mutId = id;
			this.$refs.reportModal.visible = true;
		},


		// Lock or unlock the thread
		lock: async function () {
			if (!this.canLock) return false;
			const res = await PostApiCommentsThreadLock({ threadId: this.thread });
			this.isLocked = res.ok && (res.data);
			return this.isLocked;
		},
	},

	computed: {
		comms: function () {
			// Check collapse preference
			if (this.collapse !== true) return this.comments;

			// If `collapse-deleted` is true, collapse the deleted comments
			const o = [];
			let concat = 0;

			for (const c of this.comments) {
				if (!c.val.deletedBy) {
					if (concat !== 0) {
						o.push({ snip: `Removed ${concat} comments.` });
					}
					concat = 0;
					o.push(c);
				} else {
					concat += 1;
				}
			}

			return o;
		},
	},

	async mounted() {
		const containerRef = this.$refs.container;
		this.csrf = containerRef.dataset.csrf;
		this.thread = containerRef.dataset.id;

		const fetchData = async () => {
			const threadRes = await GetApiCommentsThread(this.thread);
			if (threadRes.ok) {
				this.canLock = threadRes.headers.get("X-IsStaff").toLowerCase() === "true";

				const threadData = threadRes.data;
				this.isLocked = threadData.isLocked;
				this.perPage = threadData.perPage;
				this.minLength = threadData.minCommentLength;
				this.maxLength = threadData.maxCommentLength;
				this.type = threadData.source;
			}
		};


		const load = async () => {
			await this.load();
		};

		await Promise.allSettled([fetchData(), load()]);

		const hash = window.location.hash.split("-");
		if (hash[0] === "#page" && hash[1]) {
			this.page = Math.max(1, Number(hash[1] ?? 1));
		} else if (hash[0] === "#comment" && hash[1]) {
			this.page = 1;
			this.highlight = Number(hash[1]);
		} else {
			this.page = 1;
			history.replaceState(undefined, undefined, "");
		}
	},
});
