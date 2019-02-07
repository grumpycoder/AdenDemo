
window.$toggleWorkingButton = function (button, status) {

    if ($('.loadingoverlay').length) status = 'off';

    if (status === 'off') {
        button.LoadingOverlay('hide');
    } else {
        button.LoadingOverlay('show',
            { image: '', fontawesome: 'fa fa-cog fa-spin' });
    }
}

window.$showModalWorking = function () {
    $('.modal-content').LoadingOverlay('show',
        { image: '', fontawesome: 'fa fa-cog fa-spin' });
}

window.$hideModalWorking = function () {
    $('.modal-content').LoadingOverlay('hide');
}


$(document).ajaxError(function(e, xhr) {
    if (xhr.status === 401) {
        console.log('got 401 error');

        BootstrapDialog.show({
            
            draggable: true,
            title: 'Session Timeout',
            message: 'Your session has expired. Please login again. ',
            buttons: [
                {
                    label: 'Login',
                    action: function(dialogRef) {
                        dialogRef.close();
                        location.href = window.location.href;
                    }
                }
            ]
        });

    }
    //window.location = "/Account/Login";
    else if (xhr.status === 403)
        alert("You have no enough permissions to request this resource.");
});

// Source: http://pixelscommander.com/en/javascript/javascript-file-download-ignore-content-type/
window.downloadFile = function (sUrl) {

    //iOS devices do not support downloading. We have to inform user about this.
    if (/(iP)/g.test(navigator.userAgent)) {
        //alert('Your device does not support files downloading. Please try again in desktop browser.');
        window.open(sUrl, '_blank');
        return false;
    }

    //If in Chrome or Safari - download via virtual link click
    if (window.downloadFile.isChrome || window.downloadFile.isSafari) {
        //Creating new link node.
        var link = document.createElement('a');
        link.href = sUrl;
        link.setAttribute('target', '_blank');

        if (link.download !== undefined) {
            //Set HTML5 download attribute. This will prevent file from opening if supported.
            var fileName = sUrl.substring(sUrl.lastIndexOf('/') + 1, sUrl.length);
            link.download = fileName;
        }

        //Dispatching click event.
        if (document.createEvent) {
            var e = document.createEvent('MouseEvents');
            e.initEvent('click', true, true);
            link.dispatchEvent(e);
            return true;
        }
    }

    // Force file download (whether supported by server).
    if (sUrl.indexOf('?') === -1) {
        sUrl += '?download';
    }

    window.open(sUrl, '_blank');
    return true;
}

window.downloadFile.isChrome = navigator.userAgent.toLowerCase().indexOf('chrome') > -1;
window.downloadFile.isSafari = navigator.userAgent.toLowerCase().indexOf('safari') > -1;

