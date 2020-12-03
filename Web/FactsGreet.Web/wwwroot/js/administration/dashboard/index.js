(function () {
    let magicGrid = new MagicGrid({
        container: "#cards", // Required. Can be a class, id, or an HTMLElement.
        static: true, // Required for static content.
        animate: true, // Optional.
    });

    window.addEventListener('load', function () {
        magicGrid.listen();
    });
})()
