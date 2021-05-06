// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$('.cpfcnpj').inputmask({
    mask: ['999.999.999-99', '99.999.999/9999-99'],
    keepStatic: true
});

$(function () {
    $("#btnAdd").on('click', function () {
        $.ajax({
            async: true,
            data: $("#form").serialize(),
            type: "GET",
            url: '/Invoice/AddProduct',
            success: function (partialView) {
                $('#productsContainer').html(partialView);
            }
        });
    });
})