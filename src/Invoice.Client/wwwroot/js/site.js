// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$('.cpfcnpj').inputmask({
    mask: ['999.999.999-99', '99.999.999/9999-99'],
    keepStatic: true
});

$('.money').inputmask('currency', {
    'groupSeparator': '.',
    'radixPoint': ",",
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

function DisableForm() {
    $("#form input").prop("disabled", true);
    $("#form input").prop("disabled", true);
    $("#form select").prop("disabled", true);
    $("#form textarea").prop("disabled", true);
    $("#form button").prop("hidden", true);
    $("#form input:button").prop("hidden", true);
    $("#form input:submit").prop("hidden", true);
    $('h1').text('Detalhamento de Fatura');
}

function EditMode() {
    $('h1').text('Editar Fatura');
}