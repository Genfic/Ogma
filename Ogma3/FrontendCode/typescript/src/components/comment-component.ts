import { DeleteApiComments, GetApiCommentsMd, GetApiCommentsRevisions, PatchApiComments } from "@g/paths-public";
import type { GetRevisionResult } from "@g/types-public";
import { long } from "@h/tinytime-templates";

// @ts-ignore
Vue.component("comment", {
	props: {
		comment: {
			type: Object,
			required: true,
		},
		idx: {
			type: Number,
			required: true,
		},
		csrf: {
			type: String,
			required: true,
		},
		highlight: {
			type: Boolean,
			required: false,
			default: false,
		},
		authenticatedAs: {
			type: String,
			default: false,
		},
	},

	data: function () {
		return {
			editData: null,
			mutComment: {
				...this.comment,
				owned: this.comment.author?.userName.toLowerCase() === this.authenticatedAs,
			},
			revisions: [] as GetRevisionResult[],
			revisionsCache: null as GetRevisionResult[] | null,
			hide: this.comment.isBlocked,
		};
	},

	methods: {
		del: async function () {
			if (confirm("Are you sure you want to delete?")) {
				const res = await DeleteApiComments(this.comment.id, {
					RequestVerificationToken: this.csrf,
				});

				if (!res.ok) return;

				this.mutComment = { ...this.mutComment, deletedBy: "User" };
			}
		},

		edit: async function () {
			if (this.editData && this.editData.id === this.comment.id) return;

			this.editData = null;

			const res = await GetApiCommentsMd(this.comment.id);

			if (!res.ok) return;

			this.editData = {
				id: this.comment.id,
				body: res.data,
			};
		},

		update: async function (e: Event) {
			e.preventDefault();

			const res = await PatchApiComments(
				{
					body: this.editData.body,
					commentId: Number(this.editData.id),
				},
				{ RequestVerificationToken: this.csrf },
			);

			if (!res.ok) return;

			const data = res.data;
			Object.assign(this.mutComment, data);
			this.editData = null;
		},

		report: function () {
			this.$emit("report", this.comment.id);
		},

		// Handle Enter key input
		enter: async function (e) {
			if (e.ctrlKey) await this.update(e);
		},

		history: async function () {
			if (this.revisions.length > 0) {
				this.revisions = [];
			} else if (this.revisionsCache !== null) {
				this.revisions = this.revisionsCache;
			} else {
				const res = await GetApiCommentsRevisions(this.comment.id);
				if (!res.ok) return;
				this.revisionsCache = this.revisions = res.data;
			}
		},

		// Highlights the selected comment and scrolls it into view
		changeHighlight: function (e: Event) {
			e.preventDefault();
			this.$emit("change-hl", this.idx + 1);
		},

		toggleShow: function () {
			if (this.comment.isBlocked) {
				this.hide = !this.hide;
			}
		},

		date: (dt: string) => long.render(new Date(dt)),
	},
	template: `
        <div :id="'comment-' + (idx + 1)"
             class="comment" :class="highlight ? 'marked' : ''">

        <!-- Blocked comment -->
        <template v-if="this.hide">
            <div class="main" v-on:click="toggleShow">
                <div class="header">
                    Comment hidden by user blacklist
                </div>
            </div>
        </template>

        <!-- Deleted comment -->
        <template v-else-if="mutComment.deletedBy">
            <div class="main">
                <div class="header">

                    <time :datetime="mutComment.dateTime" class="time">{{ date(mutComment.dateTime) }}</time>
                    <p class="sm-line"></p>
                    <span>Comment deleted by {{ mutComment.deletedBy.toLowerCase() }}.</span>

                </div>
            </div>
        </template>

        <!-- Regular comment -->
        <template v-else>
            <div class="author">

                <a :href="'/user/' + mutComment.author.userName" class="name">{{ mutComment.author.userName }}</a>

                <div v-if="mutComment.author.roles[0]" class="role-tag">
                    <span class="name">{{ mutComment.author.roles[0].name }}</span>
                    <div class="bg" :style="{backgroundColor: mutComment.author.roles[0].color}"></div>
                </div>

                <img :src="mutComment.author.avatar" :alt="mutComment.author.userName + '\\'s avatar'"
                     class="avatar"
                     loading="lazy">

            </div>

            <div class="main" :class="comment.isBlocked ? 'blocked' : null">

                <div class="header">

                    <a class="link"
                       :href="'#comment-' + (idx + 1)"
                       v-on:click="changeHighlight($event)">
                        #{{ idx + 1 }}
                    </a>

                    <p class="sm-line"></p>

                    <time :datetime="mutComment.dateTime" class="time" v-on:click="toggleShow">
                        {{ date(mutComment.dateTime) }}
                    </time>

                    <div v-if="authenticatedAs" class="actions">

                        <button class="action-btn small red-hl" title="Report" v-on:click="report">
                            <o-icon icon="lucide:flag" class="material-icons-outlined icon" ></o-icon>
                        </button>

                        <template v-if="mutComment.owned">

                            <button class="action-btn small" title="Delete" v-on:click="del">
                            	<o-icon icon="lucide:trash-2" class="material-icons-outlined icon" ></o-icon>
                            </button>

                            <button class="action-btn small" title="Edit" v-on:click="edit">
                            	<o-icon icon="lucide:pencil" class="material-icons-outlined icon" ></o-icon>
                            </button>

                        </template>
                    </div>

                </div>

                <div v-if="mutComment.body && (!editData || editData.id !== mutComment.id)" class="body md"
                     v-html="mutComment.body"></div>

                <form class="form" v-if="editData && editData.id === mutComment.id">
            <textarea class="comment-box"
                      v-model="editData.body"
                      v-on:keydown.enter="enter"
                      name="body" id="edit-body"
                      rows="3"
                      aria-label="Comment">
            </textarea>

                    <div class="buttons">
                        <button class="confirm active-border" v-on:click="update">
            				<o-icon icon="lucide:pencil" class="material-icons-outlined" ></o-icon>
                            Update
                        </button>
                        <button class="cancel active-border" v-on:click="editData = null">
            				<o-icon icon="lucide:x" class="material-icons-outlined" ></o-icon>
                            Cancel
                        </button>
                    </div>
                </form>

                <button v-if="mutComment.isEdited" v-on:click="history" class="edit-data">Edited</button>

                <ol v-if="revisions.length > 0" class="history">
                    <li v-for="r in revisions">
                        <time :datetime="r.editTime">{{ date(r.editTime) }}</time>
                        <div class="body" v-html="r.body"></div>
                    </li>
                </ol>

            </div>
        </template>
        </div>
    `,
});
