import dayjs from 'https://cdn.skypack.dev/-/dayjs@v1.10.4-MoS2QVkxh1TZYPgJA5zq/dist=es2020,mode=imports/optimized/dayjs.js';

Vue.component('comment', {
    props: {
        cdn: {
            type: String,
            required: true
        },
        comment: {
            type: Object,
            required: true
        },
        idx: {
            type: Number,
            required: true
        },
        route: {
            type: String,
            required: true
        },
        csrf: {
            type: String,
            required: true
        },
        highlight: {
            type: Boolean,
            required: false,
            default: false
        },
        isAuthenticated: {
            type: Boolean,
            default: false
        }
    },

    data: function () {
        return {
            editData: null,
            mutComment: this.comment,
            revisions: [],
            revisionsCache: null,
            hide: this.comment.isBlocked
        }
    },

    methods: {
        del: function () {
            if (confirm("Are you sure you want to delete?")) {
                axios.delete(`${this.route}/${this.comment.id}`, {
                        headers: { "RequestVerificationToken": this.csrf }
                    })
                    .then(res => this.mutComment = res.data)
                    .catch(console.error);
            }
        },

        edit: function () {
            if (this.editData && this.editData.id === this.comment.id) return;

            this.editData = null;
            axios.get(`${this.route}/md`, {
                    params: {
                        id: this.comment.id
                    }
                })
                .then(res => {
                    this.editData = {
                        id: this.comment.id,
                        body: res.data
                    }
                })
                .catch(console.error);
        },

        update: function (e) {
            e.preventDefault();

            let data = {
                body: this.editData.body,
                id: Number(this.editData.id)
            };

            axios.patch(this.route, data, {
                    headers: { "RequestVerificationToken": this.csrf }
                })
                .then(res => {
                    Object.assign(this.mutComment, res.data);
                    this.editData = null;
                })
                .catch(console.error)
        },
        
        report: function() {
            this.$emit('report', this.comment.id)
        },

        // Handle Enter key input
        enter: function(e) {
            if (e.ctrlKey) this.update(e)
        },

        history: function () {
            if (this.revisions.length > 0) {
                this.revisions = [];
            } else if (this.revisionsCache !== null) {
                this.revisions = this.revisionsCache
            } else {
                axios.get(`${this.route}/revisions/${this.comment.id}`)
                    .then(res => this.revisionsCache = this.revisions = res.data)
                    .catch(console.error);
            }
        },

        // Highlights the selected comment and scrolls it into view
        changeHighlight: function (e) {
            e.preventDefault();
            this.$emit('change-hl', (this.idx + 1));
        },

        toggleShow: function () {
            if (this.comment.isBlocked) {
                this.hide = !this.hide;
            }
        },

        date: function (dt) {
            return dayjs(dt).format('DD MMM YYYY, HH:mm');
        },
        
        avatar: function (url) {
            return url.includes('gravatar')
                ? url
                : cdn + url;
        }
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
      <template v-else-if="!mutComment.author">
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

          <img :src="avatar(mutComment.author.avatar)" :alt="mutComment.author.userName + '\\'s avatar'" 
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

            <div v-if="isAuthenticated" class="actions">
              
              <button class="action-btn small red-hl" title="Report" v-on:click="report">
                <i class="icon material-icons-outlined">flag</i>
              </button>
              
              <template v-if="mutComment.owned">
                
                <button class="action-btn small" title="Delete" v-on:click="del">
                  <i class="icon material-icons-outlined">delete_forever</i>
                </button>
                
                <button class="action-btn small" title="Edit" v-on:click="edit">
                  <i class="icon material-icons-outlined">edit</i>
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
                <i class="material-icons-outlined">edit</i>
                Update
              </button>
              <button class="cancel active-border" v-on:click="editData = null">
                <i class="material-icons-outlined">cancel</i>
                Cancel
              </button>
            </div>
          </form>

          <span v-if="mutComment.lastEdit" class="edit-data">
            Edited <span class="link" v-on:click="history">{{ mutComment.editCount }} times</span>, last edit: <time
              :datetime="mutComment.lastEdit">{{ date(mutComment.lastEdit) }}</time>
          </span>

          <ol v-if="revisions.length > 0" class="history">
            <li v-for="r in revisions">
              <time :datetime="r.editTime">{{ date(r.editTime) }}</time>
              <div class="body" v-html="r.body"></div>
            </li>
          </ol>

        </div>
      </template>
      </div>
    `
});