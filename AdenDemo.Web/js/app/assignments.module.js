
(function () {

    var uriCurrent = "/api/workitem";
    var uriComplete = "/api/workitem/finished";

    var $gridCurrent = $('#gridCurrent').dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: 'id',
            loadUrl: uriCurrent
        }),
        remoteOperations: true,
        allowColumnResizing: true,
        showBorders: true,
        scrolling: { mode: "infinite" },
        paging: { pageSize: 20 },
        columnResizingMode: "nextColumn",
        columnMinWidth: 50,
        columnAutoWidth: true,
        wordWrapEnabled: true,
        height: 300,
        columns: [
            { dataField: 'displayFileName', caption: 'File Name' },
            { dataField: 'dataYear', caption: 'Data Year', width: 75 },
            { dataField: 'assignedDate', caption: 'Assigned', dataType: 'datetime', width: 150 },
            { dataField: 'dueDate', caption: 'Due', dataType: 'datetime', width: 150 },
            {
                width: 375,
                alignment: 'center',
                cellTemplate: function (container, options) {

                    if (!options.data.canGenerate) {
                        $('<a/>').addClass('btn btn-default  btn-sm btn-grid')
                            .text('Review File')
                            .attr('href', '/review/' + options.data.dataYear + '/' + options.data.fileNumber)
                            .attr('aria-label', 'Review generated file ' + options.data.fileName)
                            .attr('target', '_blank')
                            .appendTo(container);
                    }

                    var actionName = options.data.actionName;
                    if (options.data.isManualUpload && options.data.canGenerate) actionName = 'Manual';

                    $('<a/>').addClass('btn btn-primary btn-sm btn-grid')
                        .text(actionName)
                        .attr('aria-label', options.data.actionName + ' ' + options.data.fileName)
                        .on('dxclick',
                            function (e) {
                                complete($(this), options.data);
                            })
                        .appendTo(container);

                    if (options.data.canReject) {
                        $('<a/>').addClass('btn btn-danger btn-sm btn-grid')
                            .text('Reject File')
                            .attr('aria-label', 'Reject generated file ' + options.data.fileName)
                            .on('dxclick',
                                function (e) {
                                    reject($(this), options.data);
                                })
                            .appendTo(container);
                    }

                    if (options.data.canSubmit) {
                        $('<a/>').addClass('btn btn-danger btn-sm btn-grid')
                            .text('Report Errors')
                            .attr('aria-label', 'Confirm file submitted ' + options.data.fileName)
                            .on('dxclick',
                                function (e) {
                                    showReportErrors($(this), options.data);
                                })
                            .appendTo(container);
                    }

                    if (options.data.canReviewError) {
                        $('<a/>').addClass('btn btn-danger btn-sm btn-grid')
                            .text('View Errors')
                            .attr('aria-label', 'Review submission errors ' + options.data.fileName)
                            .on('dxclick',
                                function (e) {
                                    showErrorDetails($(this), options.data);
                                })
                            .appendTo(container);
                    }

                }
            }
        ],
        onToolbarPreparing: function (e) {
            var currentGrid = e.component;

            e.toolbarOptions.items.unshift(
                {
                    location: "after",
                    widget: "dxButton",

                    options: {
                        icon: "refresh",
                        hint: 'Refresh',
                        onClick: function () {
                            currentGrid.refresh();
                        }
                    }
                }
            );
        }
    }).dxDataGrid("instance");

    var $gridComplete = $('#gridComplete').dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: 'id',
            loadUrl: uriComplete
        }),
        remoteOperations: true,
        allowColumnResizing: true,
        showBorders: true,
        scrolling: { mode: "infinite" },
        paging: { pageSize: 20 },
        columnResizingMode: "nextColumn",
        columnMinWidth: 50,
        columnAutoWidth: true,
        wordWrapEnabled: true,
        height: 300,
        columns: [
            { dataField: 'displayFileName', caption: 'File Name' },
            { dataField: 'dataYear', caption: 'Data Year' },
            { dataField: 'assignedDate', caption: 'Assigned', dataType: 'datetime' },
            { dataField: 'completedDate', caption: 'Completed', dataType: 'datetime' },
            { dataField: 'actionName', caption: 'Action' },
            {
                width: 150,
                alignment: 'center',
                cellTemplate: function (container, options) {

                    if (options.data.canCancel) {
                        $('<a/>').addClass('btn btn-default btn-sm btn-grid')
                            .text('Cancel')
                            .on('dxclick',
                                function (e) {
                                    cancel($(this), options.data);
                                })
                            .appendTo(container);
                    }
                }
            }
        ],
        onToolbarPreparing: function (e) {
            var historyGrid = e.component;

            e.toolbarOptions.items.unshift(
                {
                    location: "after",
                    widget: "dxButton",

                    options: {
                        icon: "refresh",
                        hint: 'Refresh',
                        onClick: function () {
                            historyGrid.refresh();
                        }
                    }
                }
            );
        }
    }).dxDataGrid("instance");


    function complete(container, data) {
        var uri = '/api/workitem/complete/' + data.id;
        if (data.isManualUpload && data.canGenerate) {
            showUpload(container, data);
        }
        else {
            // Generate files from defined report action
            $toggleWorkingButton(container);

            $.ajax({
                url: uri,
                type: 'POST',
                success: function (data) {
                    $gridCurrent.refresh();
                    $gridComplete.refresh();
                    toastr.success('Completed task for ' + data.fileName + ' (' + data.fileNumber + ')');
                },
                error: function (err) {
                    toastr.error('Error completing task');
                }
            }).always(function () {
                $toggleWorkingButton(container);
            });

        }

    }

    function reject(container, data) {
        var uri = '/api/workitem/reject/' + data.id;

        BootstrapDialog.confirm('Reject File, are you sure?', function (result) {
            if (result) {
                window.$showModalWorking();
                $.ajax({
                    url: uri,
                    type: 'POST',
                    success: function (data) {
                        $gridCurrent.refresh();
                        $gridComplete.refresh();
                        toastr.warning('Rejected file for ' + data.fileName + ' (' + data.fileNumber + ')');

                    },
                    error: function (err) {
                        toastr.error('Error rejecting file');
                    }
                }).always(function () {
                    window.$hideModalWorking();
                });
            }
        });
    }

    function cancel(container, data) {
        var uri = '/api/workitem/cancel/' + data.id;
        $toggleWorkingButton(container);
        $.ajax({
            url: uri,
            type: 'POST',
            success: function (data) {
                $gridCurrent.refresh();
                $gridComplete.refresh();
                toastr.success('Cancelled task for ' + data.fileName + ' (' + data.fileNumber + ')');
            },
            error: function (err) {
                toastr.error('Error cancelling task');
            }
        }).always(function () {
            $toggleWorkingButton(container);
        });
    }

    function showReportErrors(container, data) {
        var title = 'Submission Errors';
        var url = '/home/errorreport/' + data.id;
        var postUrl = '/home/reporterror'; // + data.id;

        BootstrapDialog.show({
            size: window.BootstrapDialog.SIZE_WIDE,
            draggable: true,
            title: title,
            message: $('<div></div>').load(url, function (resp, status, xhr) {
                if (status === 'error') {
                    toastr.error('Error retrieving reporting errors form');
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
                    label: 'Save',
                    cssClass: 'btn-primary',
                    action: function (dialogRef) {
                        dialogRef.enableButtons(false);
                        dialogRef.setClosable(false);
                        $showModalWorking($('.panel-body'));

                        $('#errorMessage').text('');

                        var formData = new FormData($('form')[0]);

                        if (files.length > 0) {
                            for (var i = 0; i < files.length; i++) {
                                formData.append('file', files[i]);
                            }
                        }
                        $.ajax({
                            type: "POST",
                            url: postUrl,
                            data: formData,
                            contentType: false,
                            processData: false,
                            success: function (response) {
                                $gridCurrent.refresh();
                                $gridComplete.refresh();
                                dialogRef.close();
                                toastr.success('Saved errors and created task for ' + data.fileName + ' (' + data.fileNumber + ')');
                            },
                            error: function (error) {
                                toastr.error('Error saving reporting errors for ' + data.fileName + ' (' + data.fileNumber + ')');
                            },
                            complete: function () {
                                dialogRef.close();
                            }
                        });

                    }
                }
            ]
        });

    }

    function showErrorDetails(container, data) {
        var title = 'Error Details';
        var url = '/home/workitemimages/' + data.id;

        BootstrapDialog.show({
            size: window.BootstrapDialog.SIZE_WIDE,
            draggable: true,
            title: title,
            message: $('<div></div>').load(url, function (resp, status, xhr) {
                if (status === 'error') {
                    //TODO: toast error message
                }
            }),
            buttons: [
                {
                    label: 'Close',
                    action: function (dialogRef) {
                        dialogRef.close();
                    }
                }
            ]
        });

    }
    
    function showUpload(container, data) {
        var loadUrl = '/uploadreport/' + data.id;
        var postUrl = '/api/workitem/submitreport/' + data.id;
        var title = 'File Upload';

        BootstrapDialog.show({
            size: window.BootstrapDialog.SIZE_WIDE,
            draggable: true,
            title: title,
            message: $('<div></div>').load(loadUrl, function (resp, status, xhr) {
                if (status === 'error') {
                    toastr.error('Error retrieving reporting errors form');
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
                    label: 'Save',
                    cssClass: 'btn-primary',
                    action: function (dialogRef) {
                        dialogRef.enableButtons(false);
                        dialogRef.setClosable(false);
                        $showModalWorking($('.panel-body'));

                        $('#errorMessage').text('');

                        var formData = new FormData($('form')[0]);

                        if (files.length > 0) {
                            for (var i = 0; i < files.length; i++) {
                                formData.append('file', files[i]);
                            }
                        }
                        $.ajax({
                            type: "POST",
                            url: postUrl,
                            data: formData,
                            contentType: false,
                            processData: false,
                            success: function (response) {
                                $gridCurrent.refresh();
                                $gridComplete.refresh();
                                dialogRef.close();
                                toastr.success('Saved errors and created task for ' + data.fileName + ' (' + data.fileNumber + ')');
                            },
                            error: function (error) {
                                toastr.error('Error saving reporting errors for ' + data.fileName + ' (' + data.fileNumber + ')');
                            },
                            complete: function () {
                                dialogRef.close();
                            }
                        });

                    }
                }
            ]
        });

    }

})();




