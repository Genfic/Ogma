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
        }
    },
    data: function () {
        return {
            name: this.label.replace(/\s+/g, ''),
            loading: true,
            
            // Tag search
            options: [],
            selected: [],
            search: '',
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
                return (
                    x.name.toLowerCase().includes(this.search.toLowerCase())
                    || x.namespace.toLowerCase().includes(this.search.toLowerCase())
                ) && this.search.length > 0
            })
        }
    },
    methods: {
        addUnique(x) {
            if (this.selected.includes(x)) return;
            this.selected.push(x);
            this.options.find(e => e.id === x.id).hidden = true;
        },
        remove(x) {
            this.selected = this.selected.filter(e => e.id !== x.id);
            this.options.find(e => e.id === x.id).hidden = false;
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
                
            })
            .catch(console.error);
    },
    template: `
        <div>
            <select style="visibility: collapse; height: 0" :name="name" multiple="multiple" :id="name">
                <option v-for="s in selected" :value="s.id" selected>{{s.name}}</option>
            </select>
    
            <div class="o-form-group tag-search">
                <label :for="name">{{label.replace( /([A-Z])/g, " $1" )}}</label>
                <p class="desc" v-if="desc">{{desc}}</p>
    
                <div class="tags">
                    <div class="tag" v-bind:style="{background: s.rgba}" v-for="s in selected">
                        {{s.namespace ? s.namespace+':' : ''}}{{s.name}}
                        <i class="material-icons-outlined" v-on:click="remove(s)">clear</i>
                    </div>
                </div>
    
                <input class="search" type="text" placeholder="Search..." v-model="search">
    
                <ol v-if="!loading" class="search-results">
                    <li v-for="o in filtered" :style="{background: o.rgba}" :class="o.hidden ? 'hidden' : null" v-on:click="addUnique(o)">
                        <span class="name">{{o.namespace ? o.namespace+':' : ''}}{{o.name}}</span>
                        <span class="desc">{{o.description}}</span>
                    </li>
                </ol>
                
                <span v-if="!validate && validateMsg">{{validationString}}</span>
    
            </div>
        </div>
    `
});