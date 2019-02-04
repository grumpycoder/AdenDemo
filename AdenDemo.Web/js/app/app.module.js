
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
