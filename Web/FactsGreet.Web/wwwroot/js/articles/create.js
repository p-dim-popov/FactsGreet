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
})()