Vue.component("tag-search-select", {
	props: {
		min: {
			type: Number,
			default: 0,
		},
		label: {
			type: String,
			required: true,
		},
		desc: {
			type: String,
			required: false,
			default: null,
		},
		validateMsg: {
			type: String,
			default: null,
		},
		tagsApi: {
			type: String,
			required: true,
		},
		storyId: {
			type: Number,
			default: null,
		},
		preselected: {
			type: Array,
			default: null,
		},
		inline: {
			type: Boolean,
			default: false,
		},
		disableWhenEmpty: {
			type: Boolean,
			default: false,
		},
		hideLabels: {
			type: Boolean,
			default: false,
		},
	},
	data: function () {
		return {
			name: this.label.replace(/\s+/g, "").toLowerCase(),
			loading: true,

			// Tag search
			options: [],
			selected: [],
			search: "",
			highlighted: null,
			focused: false,

			disable: false,
		};
	},
	computed: {
		validate: function () {
			return this.selected.length >= this.min;
		},
		validationString: function () {
			return this.validateMsg.replace("{0}", `${this.min}`);
		},
		filtered() {
			return this.options.filter((x) => {
				const inName = x.name.toLowerCase().includes(this.search.toLowerCase());
				const inNamespace =
					x.namespace?.toLowerCase().includes(this.search.toLowerCase()) ??
					false;

				return (
					(inName || inNamespace) &&
					!this.selected.some((i) => i.id === x.id) &&
					this.search.length > 0
				);
			});
		},
	},
	methods: {
		pushUnique: (arr, el) => {
			if (arr.includes(el)) return;
			arr.push(el);
		},

		handleInputKeys: function (e) {
			switch (e.key) {
				case "Backspace":
					if (this.search.length <= 0) {
						this.selected.pop();
					}
					break;
				case "ArrowUp":
					if (this.highlighted !== null) e.preventDefault();
					if (this.highlighted > 0) {
						this.highlighted--;
					} else {
						this.highlighted = null;
					}
					break;
				case "ArrowDown":
					if (this.highlighted !== null) e.preventDefault();
					if (this.highlighted === null) {
						this.highlighted = 0;
					} else if (this.highlighted < this.filtered.length - 1) {
						this.highlighted++;
					}
					break;
				case " ":
				case "Enter":
					if (this.highlighted !== null) {
						e.preventDefault();
						this.highlighted = 0;
						this.pushUnique(
							this.selected,
							JSON.parse(JSON.stringify(this.filtered[this.highlighted])),
						);
					}
					break;
				default:
					break;
			}
		},
		checkDisabled: function () {
			this.disable = this.selected.length <= 0;
			return this.disable;
		},
		onClose: function () {
			this.focused = false;
		},
	},
	async created() {
		const { data } = await axios.get(`${this.tagsApi}/all`);
		this.options = data;
		this.loading = false;

		if (this.storyId) {
			const { data } = await axios.get(`${this.tagsApi}/story/${this.storyId}`);
			this.selected = data;
			for (const sel in this.selected) {
				this.options.find((e) => e.id === sel.id).hidden = true;
			}
		}

		if (this.preselected) {
			this.selected = this.options.filter(
				(o) => this.preselected.indexOf(o.id) !== -1,
			);
		}
	},
	template: `
        <div class="tag-search-select"
             v-on:focusin="focused = true">
        <select class="output"
                :name="name"
                multiple="multiple"
                :id="name"
                :disabled="disable">
            <option v-for="s in selected" :value="s.id" selected>{{ s.name }}</option>
        </select>

        <div class="o-form-group tag-search"
             :class="inline ? 'inline' : null"
             :style="{ marginTop: hideLabels ? 0 : null }"
             v-closable="{
                    exclude: ['search'],
                    handler: 'onClose'
                 }">
            <template v-if="!hideLabels">
                <label :for="name">{{ label.replace(/([A-Z])/g, " $1") }}</label>
                <p class="desc" v-if="desc && !inline">{{ desc }}</p>
            </template>

            <div class="searchbar" ref="search">
                <div class="tags">

                    <div class="tag" v-for="s in selected">
                        <div class="bg" v-bind:style="{background: s.namespaceColor}"></div>
                        <span class="name">
                        {{ s.namespaceName ? s.namespaceName + ':' : '' }}{{ s.name }}
                            <i class="material-icons-outlined" v-on:click="selected.remove(s)">clear</i>
                      </span>
                    </div>

                    <input type="text"
                           class="search"
                           v-model="search"
                           v-on:keydown="handleInputKeys"
                           placeholder="Search...">
                </div>

                <ol v-if="!loading && focused" class="search-results">
                    <li v-for="(o, idx) in filtered"
                        :style="{background: o.rgba}"
                        :class="highlighted === idx ? 'hl' : null"
                        v-on:click="selected.pushUnique(o)">
                        <span class="name">{{ o.namespace ? o.namespace + ':' : '' }}{{ o.name }}</span>
                    </li>
                </ol>
            </div>

        </div>

        <span v-if="!validate && validateMsg">{{ validationString }}</span>
        </div>
    `,
});
