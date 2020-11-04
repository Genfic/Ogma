Vue.component('folder-item', {
    props: {
        folder: {
           type: Object,
           required: true
        },
        sel: {
           type: Number
        }
    },
    methods: {
        bus: function (data) {
            this.$emit('bus', data)
        }
    },
    template: `<div style="margin-left: 1rem">
        <span v-on:click.self="$emit('bus', folder.id)" :style="{ backgroundColor: this.sel === folder.id ? 'red' : null }">
          {{folder.id}}: {{folder.name}}
        </span> 
        <template v-if="folder.children.length > 0">
            <folder-item v-for="f in folder.children" 
                         :folder="f" 
                         :key="f.id"
                         @bus="bus"
                         :sel="sel">
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
    },
    data: function () {
        return {
            folders: [],
            tree: [],
            sel: null,
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
        <div class="folder-tree">
          <input type="hidden" :value="sel" name="parentId">
          <folder-item v-for="f in tree" 
                       :folder="f" 
                       :key="f.id"
                       @bus="bus"
                       :sel="sel">
          </folder-item>
        </div>
    `
});