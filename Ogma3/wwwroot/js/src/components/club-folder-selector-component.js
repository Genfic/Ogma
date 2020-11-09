Vue.component('club-folder-selector', {
    props: {
        clubsRoute: {
            type: String,
            required: true
        },
        foldersRoute: {
            type: String,
            required: true
        },
        cdn: {
            type: String,
            required: true
        },
        storyId: {
            type: Number,
            required: true
        }
    },
    data: function () {
        return {
            clubs: [],
            folders: null,
            selectedClub: null,
            csrf: null,
            status: {
                message: '',
                success: null
            },
            visible: false
        }
    },
    methods: {
        add: function () {
            axios.post(`${this.foldersRoute}/add-story`, {
                folderId: this.$refs.folder.sel,
                storyId: this.storyId
            }, {
                headers: { "RequestVerificationToken" : this.csrf }
            })
            .then(_ => {
                this.status = {
                    message: 'Successfully added',
                    success: true
                }
            })
            .catch(err => {
                this.status = {
                    message: err.response.data,
                    success: false
                }
            });
        }
    },
    mounted() {
        axios.get(`${this.clubsRoute}/user`)
            .then(res => this.clubs = res.data)
            .catch(console.error);
        this.csrf = document.querySelector('input[name=__RequestVerificationToken]').value;
    },
    template: `<div class="club-folder-selector my-modal" v-if="visible" @click.self="visible = false">
      <div class="content">
        <template v-if="selectedClub">
          
            <div class="header">
              <img :src="cdn + (selectedClub.icon ?? 'ph-250.png')" :alt="selectedClub.name" width="16" height="16">
              <span>{{selectedClub.name}}</span>
            </div>
            
            <folder-tree ref="folder"
              :club-id="selectedClub.id"
              :route="foldersRoute"
              label="Select folder">
            </folder-tree>
            
            <div class="buttons">
              <button @click="add">Add</button>
              <button @click="selectedClub = null">Go back</button>
            </div>
          
          </template>
          <template v-else>
          
            <div class="clubs">
              <div class="club" v-for="c in clubs" @click="selectedClub = c">
                <img :src="cdn + (c.icon ?? 'ph-250.png')" :alt="c.name" width="16" height="16">
                <span>{{c.name}}</span>
              </div>
            </div>
            
          </template>
      </div>
    </div>`
});