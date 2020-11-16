Vue.component('manage-ban', {
    props: {
        actionName: {
          type: String,
          required: true  
        },
        banDate: {
            type: String,
            required: true
        },
        duration: {
            type: Number,
            required: true
        }
    },
    data: function () {
        return {            
            newDuration: 0,
            visible: false
        }
    },
    methods: {
        hide: function () {
            this.visible = false;
        },
        execute: function () {
            
        },
        unban: function () {
            
        }
    },
    template: `
      <div class="my-modal" v-if="visible" @click.self="hide">
          <div class="content">
            <strong>Manage {{actionName.toLowerCase()}}</strong>
            <hr>
            
            <template v-if="banDate">
              <time :datetime="banDate">Lasts until {{banDate}}</time>
              <br>
              <span><strong>{{duration}}</strong> days left</span>
              <hr>
              <button @click="unban">Un{{actionName.toLowerCase()}}</button>
            </template>
            
            <template v-else>
              {{actionName}} user for <input type="number" min="1"> days
              <br>
              <button @click="execute">Save</button>
            </template>
            
          </div>
        </div>
    `
});