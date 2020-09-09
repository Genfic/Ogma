Vue.component('tag-search-select', {
    props: {
        min: {
            type: Number,
            default: 0
        },
        label: {
            type: String,
            required: true
        },
        desc: {
            type: String,
            required: false,
            default: null
        },
        validateMsg: {
            type: String,
            default: null
        },
        tagsApi: {
            type: String,
            required: true
        },
        storyId: {
            type: Number,
            default: null
        },
        preselected: {
            type: Array,
            default: null
        },
        inline: {
            type: Boolean,
            default: false
        },
        disableWhenEmpty: {
            type: Boolean,
            default: false
        }
    },
    data: function () {
        return {
            name: this.label.replace(/\s+/g, '').toLowerCase(),
            loading: true,
            
            // Tag search
            options: [],
            selected: [],
            search: '',
            highlighted: null,
            focused: false,
            
            disable: false
        }
    },
    computed: {
        validate: function () {
            return this.selected.length >= this.min;
        },
        validationString: function () {
            return this.validateMsg
                .replace('{0}', `${this.min}`);
        },
        filtered() {
            return this.options.filter(x => {
                let inName = x.name.toLowerCase().includes(this.search.toLowerCase());
                let inNamespace = x.namespace && x.namespace.toLowerCase().includes(this.search.toLowerCase());
                
                return (inName || inNamespace)
                    && !this.selected.some(i => i.id === x.id)
                    && this.search.length > 0
            })
        }
    },
    methods: {
        handleInputKeys: function (e) {
            switch (e.key) {
                case 'Backspace':
                    if (this.search.length <= 0) {
                        this.selected.pop();
                    }
                    break;
                case 'ArrowUp':
                    if (this.highlighted !== null) e.preventDefault();
                    if (this.highlighted > 0) {
                        this.highlighted--;
                    } else {
                        this.highlighted = null;
                    }
                    break;
                case 'ArrowDown':
                    if (this.highlighted !== null) e.preventDefault();
                    if (this.highlighted === null) {
                        this.highlighted = 0;
                    } else if (this.highlighted < this.filtered.length - 1) {
                        this.highlighted++;
                    }
                    break;
                case ' ':
                case 'Enter':
                    if (this.highlighted !== null) {
                        e.preventDefault();
                        this.highlighted = 0;
                        this.selected.pushUnique(JSON.parse(JSON.stringify(this.filtered[this.highlighted])));
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
        }
    },
    created() {
        axios.get(this.tagsApi)
            .then(res => {
                this.options = res.data; 
                this.loading = false;
                
                if (this.storyId) {
                    axios.get(`${this.tagsApi}/story/${this.storyId}`)
                        .then(res => {
                            this.selected = res.data;
                            this.selected.forEach(x => this.options.find(e => e.id === x.id).hidden = true)
                        })
                        .catch(console.error);
                }
                
                if (this.preselected) {
                    this.selected = this.options.filter(o => this.preselected.indexOf(o.id) !== -1)
                }
                
            })
            .catch(console.error);
    },
    template: `
        <div class="tag-search-select"
             v-on:focusin="focused = true">
            <select class="output" 
                    :name="name" 
                    multiple="multiple" 
                    :id="name" 
                    :disabled="disable">
                <option v-for="s in selected" :value="s.id" selected>{{s.name}}</option>
            </select>
    
            <div class="o-form-group tag-search" 
                 :class="inline ? 'inline' : null"
                 v-closable="{
                    exclude: ['search'],
                    handler: 'onClose'
                 }">
                <label :for="name">{{label.replace( /([A-Z])/g, " $1" )}}</label>
                <p class="desc" v-if="desc && !inline">{{desc}}</p>
    
                <div class="searchbar" ref="search">
                  <div class="tags">
                    
                    <div class="tag" v-bind:style="{background: s.rgba}" v-for="s in selected">
                      {{s.namespace ? s.namespace+':' : ''}}{{s.name}}
                      <i class="material-icons-outlined" v-on:click="selected.remove(s)">clear</i>
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
                      <span class="name">{{o.namespace ? o.namespace+':' : ''}}{{o.name}}</span>
                    </li>
                  </ol>
                </div>
    
            </div>

            <span v-if="!validate && validateMsg">{{validationString}}</span>
        </div>
    `
});