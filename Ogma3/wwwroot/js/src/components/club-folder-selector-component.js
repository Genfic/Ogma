Vue.component("club-folder-selector", {
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
                message: "",
                success: null
            },
            visible: false
        };
    },
    methods: {
        add: async function () {
            if (!this.$refs.folder.sel) {
                this.status = {
                    message: "You must select a folder!",
                    success: false
                };
                return;
            }

            try {
                await axios.post(`${this.foldersRoute}/add-story`, {
                    folderId: this.$refs.folder.sel,
                    storyId: this.storyId
                }, {
                    headers: {"RequestVerificationToken": this.csrf}
                });
                this.status = {
                    message: "Successfully added",
                    success: true
                };
            } catch (err) {
                this.status = {
                    message: err.response.data,
                    success: false
                };
            }
        },
        hide: function () {
            this.visible = false;
            this.selectedClub = null;
        }
    },
    async mounted() {
        const {data} = await axios.get(`${this.clubsRoute}/user`);
        this.clubs = data;

        this.csrf = document.querySelector("input[name=__RequestVerificationToken]").value;
    },
    template: `
        <div class="club-folder-selector my-modal" v-if="visible" @click.self="hide" v-cloak>
        <div class="content">
            <template v-if="selectedClub">

                <div class="header">
                    <img :src="cdn + (selectedClub.icon ?? 'ph-250.png')" :alt="selectedClub.name" width="32"
                         height="32">
                    <span>{{ selectedClub.name }}</span>
                </div>

                <div class="msg" :class="status.success ? 'success' : 'error'">
                    {{ status.message }}
                </div>

                <folder-tree ref="folder"
                             :show-none="false"
                             :club-id="selectedClub.id"
                             :route="foldersRoute">
                </folder-tree>

                <div class="buttons">
                    <button class="active-border add" @click="add">Add</button>
                    <button class="active-border cancel" @click="selectedClub = null">Go back</button>
                </div>

            </template>
            <template v-else>

                <div class="header">
                    <span>Your clubs</span>
                </div>

                <div class="clubs">
                    <div class="club" tabindex="0" v-for="c in clubs" @click="selectedClub = c">
                        <img :src="cdn + (c.icon ?? 'ph-250.png')" :alt="c.name" width="24" height="24">
                        <span>{{ c.name }}</span>
                    </div>
                </div>

            </template>
        </div>
        </div>
    `
});