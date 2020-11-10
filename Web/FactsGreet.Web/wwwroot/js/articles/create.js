(function(){
    const previewImage = document.getElementById("preview-img");
    const previewImageSrc = previewImage.src;
    document.getElementById("thumbnail-link-input")
        .addEventListener('change', function () {
            previewImage.src = !!this.value ? this.value : previewImageSrc;
        })   
})()