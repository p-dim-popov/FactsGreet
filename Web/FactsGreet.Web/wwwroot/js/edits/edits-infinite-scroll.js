//view dependent string constants must be specified in window.globalConstants in corresponding view
(async function () {
    const main = document.getElementById('app');
    // console.log(window.sessionStorage.getItem('lastLocation'))
    // console.log(window.location.href);
    // if (!window.sessionStorage.getItem('lastLocation').includes(window.location.origin + '/Article/'))
    //     window.sessionStorage.setItem('page', '1');
    // let page = (+window.sessionStorage.getItem('page') - 1) || 1;
    // let requestIsPending = false;

    async function loadMoreArticles() {
        if (requestIsPending) return;
        requestIsPending = true;
        const articlesResponse =
            await fetch(
                `/Edits/GetEditsWithArticleCards?${window?.globalConstants?.queryParameters ?? ''}&page=${page}`,
                {redirect: 'follow'});
        requestIsPending = false;

        if (articlesResponse.status !== 200) return;
        const articles = (await articlesResponse.text()).trim();
        if (articles.length === 0) return;
        main.innerHTML += articles;

        window.sessionStorage.setItem('page', Math.max(page).toString());
        console.log(page);
        page++;
    }

    document.getElementById('load-more-btn').addEventListener('click', loadMoreArticles)

    setInterval(() => {
        const scrollHeight = $(document).height();
        const scrollPos = $(window).height() + $(window).scrollTop();
        if (!requestIsPending && ((scrollHeight - 300) >= scrollPos) / scrollHeight === 0) {
            loadMoreArticles();
        }
        if (scrollPos === scrollHeight) {
            window.scrollTo({top: scrollHeight - 500})
        }
    }, 500)
})()
