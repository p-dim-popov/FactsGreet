(function (){
    let magicGrid = new MagicGrid({
        container: "#files", // Required. Can be a class, id, or an HTMLElement.
        static: true, // Required for static content.
        animate: true, // Optional.
    });

    window.addEventListener('load', function (){
        magicGrid.listen();
        
        [...document.getElementsByClassName('rename-btn')]
            .forEach(el => el.addEventListener('click', function (){
                this.setAttribute('hidden', true);
                this.previousElementSibling.removeAttribute('hidden');
                this.previousElementSibling.removeAttribute('disabled');
                this.nextElementSibling.removeAttribute('hidden');
            }))
    })
})()
