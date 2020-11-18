(function () {
    const previewImage = document.getElementById("preview-img");
    const previewImageSrc = previewImage.src;

    const thumbnailLinkInput = document.getElementById("thumbnail-link-input");
    const thumbnailImageInput = document.getElementById('thumbnail-image-input');

    thumbnailLinkInput
        .addEventListener('change', function () {
            previewImage.src = !!this.value ? this.value : previewImageSrc;
            thumbnailImageInput.value = '';
        })

    thumbnailImageInput
        .addEventListener('change', function () {
            previewImage.src = !!this.files ? window.URL.createObjectURL(this.files[0]) : previewImageSrc;
            thumbnailLinkInput.value = '';
        })

    new SimpleMDE({element: document.getElementById('content-textarea')})

    const categoryRows = document.getElementById('category-rows');

    function repairNamesAndIdsOnCategoryRows() {
        let i = 0;
        [...categoryRows.children]
            .forEach(x =>
                (x.firstElementChild.id = `${window.categoriesRoute}[${i}].Name`) &&
                (x.firstElementChild.name = `${window.categoriesRoute}[${i}].Name`) &&
                ++i &&
                (x.lastElementChild.firstElementChild.addEventListener('click', repairNamesAndIdsOnCategoryRows)))
    }

    const categoryInput = document.getElementById('category-input');

    function addCategoryRow() {
        const dummyNumber = Math.random();
        categoryRows.innerHTML += `
                <div class="input-group my-2">
                    <input class="form-control"
                            id="${dummyNumber}" 
                            name="${dummyNumber}" 
                            value="${categoryInput.value}"
                            type="text"
                            readonly>
                    <div class="input-group-append">
                        <a class="btn btn-danger input-group-text"
                                onclick="this.parentElement.parentElement.remove()">
                            Remove
                        </a>
                    </div>
                </div>
                `;
        categoryInput.value = '';
        repairNamesAndIdsOnCategoryRows();
    }

    document.getElementById('add-category-btn')
        .addEventListener('click', addCategoryRow);
})()
