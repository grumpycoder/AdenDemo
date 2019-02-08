// mail.module.js

(function () {
    console.log('mail viewer ready');

    $('[data-message-id]').tooltip();

    $(document).on('click', '[data-message-id]', function (e) {
        e.preventDefault();
        var btn = $(this);
        console.log('btn', btn);
        var id = $(this).data('message-id');
        var url = '/api/mail/delete/' + id;
        $.ajax({
            url: url,
            type: 'POST',
            success: function (data) {
                toastr.success('Deleted message');
                btn.parent().closest('div').parent().remove();
            },
            error: function (err) {
                toastr.error('Something went wrong: ' + err.responseJSON.message);
            }
        }).always(function () {

        });
    });
})();