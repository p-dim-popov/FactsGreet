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

    const categoryInput = document.getElementById('category-input');
    function addCategoryRow(){
        if (window.categoryRows === undefined) window.categoryRows = 0;
        document.getElementById('category-rows').innerHTML += `
                <div class="input-group my-2">
                    <input class="form-control"
                            id="${window.categoriesRoute}[${window.categoryRows}].Name" 
                            name="${window.categoriesRoute}[${window.categoryRows}].Name" 
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
        window.categoryRows++;
    }
    
    document.getElementById('add-category-btn')
        .addEventListener('click', addCategoryRow);
})()
