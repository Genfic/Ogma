Vue.component('folder-item', {
    props: {
        folder: {
           type: Object,
           required: true
        },
        sel: {
           type: Number
        },
        current: {
            type: Number
        },
        disabled: {
            type: Boolean,
            default: false
        }
    },
    methods: {
        bus: function (data) {
            if (data !== this.current)
            {
                this.$emit('bus', data);
            }
        }
    },
    template: `
      <div class="folder" :class="folder.id === current || disabled ? 'disabled' : null">
        <span v-on:click.self="$emit('bus', folder.id)" 
              :class="sel === folder.id ? 'active' : null"
              :tabindex="folder.id === current || disabled ? -1 : 0">
          {{folder.name}}
        </span> 
        <template v-if="folder.children.length > 0">
            <folder-item v-for="f in folder.children" 
                         :folder="f" 
                         :key="f.id"
                         @bus="bus"
                         :sel="sel"
                         :current="current"
                         :disabled="folder.id === current">
            </folder-item>  
        </template>  
    </div>`
});

Vue.component('folder-tree', {
    props: {
        clubId: {
            type: Number,
            required: true
        },
        route: {
            type: String,
            required: true
        },
        label: {
            type: String,
            required: true
        },
        value: {
            type: Number,
            default: null
        },
        current: {
            type: Number,
            default: null,
        },
        desc: {
            type: String,
            default: null
        },
    },
    data: function () { 
        return {
            folders: [],
            tree: [],
            sel: this.value,
            name: this.label.replace(/\s+/g, '')
        }
    },
    methods: {
        unflatten: function () {
            let hashTable = Object.create(null);
            this.folders.forEach( aData => hashTable[aData.id] = { ...aData, children : [] } );
            this.folders.forEach( aData => {
                if( aData.parentFolderId ) {
                    hashTable[aData.parentFolderId].children.push(hashTable[aData.id]);
                }
                else {
                    this.tree.push(hashTable[aData.id]);
                }
            });
        },
        bus: function (data) {
            console.log(data);
            this.sel = data;
        }
    },
    mounted() {
        axios.get(`${this.route}/${this.clubId}`)
            .then(res => {
                this.folders = res.data;
                this.unflatten()
            })
            .catch(console.error);
    },
    template: `
        <div class="o-form-group">
            <label :for="name">{{label.replace( /([A-Z])/g, " $1" )}}</label>
            <p class="desc" v-if="desc">{{desc}}</p>
            <input type="hidden" :value="sel" name="parentId">
            
            <div class="folder-tree active-border">
              
                <span v-on:click.self="sel = null"
                    :class="sel === null ? 'active' : null"
                    tabindex="0">
                  None
                </span>

                <folder-item v-for="f in tree" 
                           :folder="f" 
                           :key="f.id"
                           @bus="bus"
                           :sel="sel"
                           :current="current">
                </folder-item>
            </div>
        </div>
    `
});