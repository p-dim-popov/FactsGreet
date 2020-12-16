(async function () {
    const main = document.getElementById('app');
    const userEmail = document.getElementById('Email');
    let requestIsPending = false;

    new QRCode(document.getElementById("qrcode"), window.location.href);
    document.getElementById('qrcode').children[1].style.width = '30vw';

    new QRCode(document.getElementById("qrcode-md"), window.location.href);
    document.getElementById('qrcode-md').children[1].style.width = '10vw';

    async function loadMoreArticles() {
        if (requestIsPending) return;
        requestIsPending = true;
        const articlesResponse =
            await fetch(`/Edits/GetEditsWithArticleCards?` +
                `editReferenceId=${main.lastElementChild?.id || ''}&` +
                `email=${userEmail.value}`);
        requestIsPending = false;

        if (articlesResponse.status !== 200) return;
        const edits = (await articlesResponse.text()).trim();
        if (edits.length === 0) return;
        main.innerHTML += edits;
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
