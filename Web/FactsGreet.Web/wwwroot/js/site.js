// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
[...document.getElementsByClassName("utc-to-local")]
    .forEach(x => {
        if (!!x.value)
            x.value = new Date(x.value.trim()).toLocaleString()
        else x.textContent = new Date(x.textContent.trim()).toLocaleString()
    })
