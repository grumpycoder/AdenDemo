

(function () {
    console.log('review ready');

    $(document).on('click', 'a[data-doc-id]', function (e) {
        var id = $(this).data('doc-id');
        var $currentTarget = $(this);
        var title = $currentTarget.data('title');
        var url = $currentTarget.data('url');

        BootstrapDialog.show({
            size: BootstrapDialog.SIZE_WIDE,
            draggable: true,
            title: title,
            message: $('<div></div>').load(url, function (resp, status, xhr) {
                if (status === 'error') {
                    window.$log.error('Error showing history');
                }
            }),
            buttons: [
                {
                    label: 'Close',
                    action: function (dialogRef) {
                        dialogRef.close();
                    }
                },
                {
                    label: 'Download',
                    cssClass: 'btn-primary',
                    action: function (dialogRef) {
                        var downloadUrl = '/download/' + id;
                        window.downloadFile(downloadUrl);
                        window.$log.info('Your file is being downloaded');
                        dialogRef.close();
                    }
                }
            ]
        });
    });
})();


