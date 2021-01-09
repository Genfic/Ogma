import Litedom from '//unpkg.com/litedom'

Litedom({
    tagName: 'o-quote-box',
    data: {
        quote: null,
        author: null,
        loading: false
    },
    async fetch() {
        this.loading = true;
        
        try {
            const data = await axios.get(this.prop.route)
            this.quote = data.body;
            this.author = data.author;
        } catch (e) {
            console.error(e)
        } finally {
            this.loading = false;
        }
        
        // axios.get(this.route)
        //     .then(res => {
        //         this.quote = res.data.body;
        //         this.author = res.data.author;
        //     })
        //     .catch(console.error)
        //     .finally(() => this.loading = false)
    },
    template: `
    <div id="quote" class="quote active-border">
        <div class="refresh" @click="fetch">
            <icon icon="refresh" :class="spin: this.loading"></icon>
        </div>
        <em class="body">{this.quote}</em>
        <span class="author">~ {this.author}</span>
    </div>`
})