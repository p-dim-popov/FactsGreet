(async function () {
    await (async () => {
        const templateResponse = await fetch('/views/home/feed.js')
        const template = 'return ' + await templateResponse.text();

        class ArticleCard extends HTMLElement {
            constructor() {
                super();
                this.categoriesDisplayCount = 3;
                this.attachShadow({mode: "open"});
            }

            connectedCallback() {
                this.styleLinks = `<link href="/lib/bootstrap/dist/css/bootstrap.min.css" rel="stylesheet"/>`;
                this.title = this.getAttribute('title');
                this.author = this.getAttribute('author');
                this.thumbnailLink = this.getAttribute('thumbnail-Link');
                this.starsCount = +this.getAttribute('fans-count');
                this.isCreated = !!this.getAttribute('is-created');
                this.shortContent = this.getAttribute('short-content')
                this.categories = []//JSON.parse(this.getAttribute('categories'));

                this.shadowRoot.innerHTML = new Function(template).call(this);
            }
        }

        customElements.define("article-card", ArticleCard);
    })();

    // region Global declarations
    const main = document.getElementById('app');
    console.log(window.sessionStorage.getItem('lastLocation'))
    console.log(window.location.href);
    if (!window.sessionStorage.getItem('lastLocation').includes(window.location.origin + '/Articles/'))
        window.sessionStorage.setItem('page', '1');
    let page = +window.sessionStorage.getItem('page') || 1;
    let requestIsPending = false;

    // endregion

    async function loadMoreArticles() {
        if (requestIsPending) return;
        requestIsPending = true;
        const articlesResponse = await fetch(`/Home/GetFeedActivities?page=${page++}`);
        requestIsPending = false;
        
        if (articlesResponse.status !== 200) return;
        const editViewModelCollection = await articlesResponse.json();
        if (editViewModelCollection.length === 0) return;
        main.innerHTML += editViewModelCollection.reduce((acc, {editorUserName, isCreation, article}) => 
            //TODO: after user profile view add links to each editor's profile using editorUserName
            [...acc, `
<article-card title="${article.title}" 
            author="${editorUserName}" 
            short-content="${!!article.description
                ? article.description
                : article.shortContent}"
            thumbnail-link="${article.thumbnailLink}"
            stars-count="${article.starsCount}"
            is-created=${isCreation}
            categories="${JSON.stringify(article.categories)}">
</article-card>
`
            ], []).join('')
        window.sessionStorage.setItem('page', Math.max(page - 1).toString());
        console.log(page - 1)
    }

    loadMoreArticles();

    // TODO: correct pages, not correct
    document.getElementById('load-more-btn').addEventListener('click', loadMoreArticles)

    setInterval(() => {
        const scrollHeight = $(document).height();
        const scrollPos = $(window).height() + $(window).scrollTop();
        if (scrollPos === scrollHeight) {
            window.scrollTo({top: scrollHeight - 500})
        }
    }, 500)

    function loadMoreIfBottom() {
        const scrollHeight = $(document).height();
        const scrollPos = $(window).height() + $(window).scrollTop();

        if (((scrollHeight - 300) >= scrollPos) / scrollHeight === 0) {
            loadMoreArticles();
        }
    }

    window.addEventListener('scroll', loadMoreIfBottom);
})()