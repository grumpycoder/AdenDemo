(function () {

    var uriCurrent = "/api/workitem/mark@mail.com/";
    var uriComplete = "/api/workitem/finished/mark@mail.com/";

    var $gridCurrent = $('#gridCurrent').dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: 'id',
            loadUrl: uriCurrent,
        }),
        remoteOperations: true,
        allowColumnResizing: true,
        showBorders: true,
        columnResizingMode: "nextColumn",
        columnMinWidth: 50,
        columnAutoWidth: true,
        wordWrapEnabled: true,
        height: 300,
        filterRow: { visible: false },
        headerFilter: { visible: false },
        groupPanel: { visible: false },
        scrolling: { mode: "virtual", rowRenderingMode: "virtual", },
        paging: { pageSize: 20 },
        columns: [
            { dataField: 'displayFileName', caption: 'File Name' },
            { dataField: 'dataYear', caption: 'Data Year', width: 75, },
            { dataField: 'assignedDate', caption: 'Assigned', dataType: 'datetime', width: 150, },
            { dataField: 'dueDate', caption: 'Due', dataType: 'datetime', width: 150, },
            {
                //width: 150,

                alignment: 'center',
                cellTemplate: function (container, options) {

                    if (!options.data.canGenerate) {
                        $('<a/>').addClass('btn btn-primary  btn-sm btn-grid')
                            .text('Review File')
                            .attr('href', '/review/' + options.data.dataYear + '/' + options.data.fileNumber)
                            .attr('target', '_blank')
                            .appendTo(container);
                    }


                    $('<a/>').addClass('btn btn-success btn-sm btn-grid')
                        .text(options.data.actionName)
                        .on('dxclick',
                            function (e) {
                                complete($(this), options.data);
                            })
                        .appendTo(container);

                    if (options.data.canReject) {
                        $('<a/>').addClass('btn btn-danger btn-sm btn-grid')
                            .text('Reject')
                            .on('dxclick',
                                function (e) {
                                    reject($(this), options.data);
                                })
                            .appendTo(container);
                    }

                    if (options.data.canSubmit) {
                        $('<a/>').addClass('btn btn-danger btn-sm btn-grid')
                            .text('Report Errors')
                            .on('dxclick',
                                function (e) {
                                    showReportErrors($(this), options.data);
                                })
                            .appendTo(container);
                    }
                    if (options.data.canReviewError) {
                        $('<a/>').addClass('btn btn-danger btn-sm btn-grid')
                            .text('View Details')
                            .on('dxclick',
                                function (e) {
                                    showErrorDetails($(this), options.data);
                                })
                            .appendTo(container);
                    }

                }
            },
        ],
        onToolbarPreparing: function (e) {
            var dataGrid = e.component;

            e.toolbarOptions.items.unshift(
                {
                    location: "after",
                    widget: "dxButton",
                    options: {
                        icon: "refresh",
                        onClick: function () {
                            dataGrid.refresh();
                        }
                    }
                },
            );
        }
    }).dxDataGrid("instance");

    var $gridComplete = $('#gridComplete').dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: 'id',
            loadUrl: uriComplete,
        }),
        remoteOperations: true,
        allowColumnResizing: true,
        showBorders: true,
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
            },
        ],
        filterRow: {
            visible: false
        },
        headerFilter: {
            visible: false
        },
        groupPanel: {
            visible: false
        },
        scrolling: {
            mode: "virtual",
            rowRenderingMode: "virtual",
        },
        paging: {
            pageSize: 20
        },

        onToolbarPreparing: function (e) {
            var dataGrid = e.component;

            e.toolbarOptions.items.unshift(
                {
                    location: "after",
                    widget: "dxButton",
                    options: {
                        icon: "refresh",
                        onClick: function () {
                            dataGrid.refresh();
                        }
                    }
                },
            );
        }
    }).dxDataGrid("instance");


    function complete(container, data) {
        console.log('generate', data);
        var uri = '/api/workitem/complete/' + data.id;
        $.ajax({
            url: uri,
            type: 'POST',
            success: function (data) {
                $gridCurrent.refresh();
                $gridComplete.refresh();
                //TODO: Toast success completed action
            },
            error: function (err) {
                //TODO: Toast error completing action
            }
        }).always(function () {
        });
    }

    function reject(container, data) {
        console.log('generate', data);
        var uri = '/api/workitem/reject/' + data.id;
        $.ajax({
            url: uri,
            type: 'POST',
            success: function (data) {
                $gridCurrent.refresh();
                $gridComplete.refresh();
                //TODO: Toast success completed action
            },
            error: function (err) {
                //TODO: Toast error completing action
            }
        }).always(function () {
        });
    }

    function cancel(container, data) {
        console.log('generate', data);
        var uri = '/api/workitem/cancel/' + data.id;
        $.ajax({
            url: uri,
            type: 'POST',
            success: function (data) {
                $gridCurrent.refresh();
                $gridComplete.refresh();
                //TODO: Toast success completed action
            },
            error: function (err) {
                //TODO: Toast error completing action
            }
        }).always(function () {
        });
    }

    function showReportErrors(container, data) {
        console.log('show report errors');
        var title = 'History';
        var url = '/home/errorreport/' + data.id;
        var postUrl = '/home/reporterror'; // + data.id;

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
                },
                {
                    label: 'Save',
                    cssClass: 'btn-primary',
                    action: function (dialogRef) {
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
                                //TODO: Toast success completed action
                                dialogRef.close();
                            },
                            error: function (error) {
                                //TODO: Toast error completing action
                            },
                            complete: function () {

                            }
                        });

                    }
                }
            ]
        });

    }

    function showErrorDetails(container, data) {
        console.log('show report errors');
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


})();






(function () {
    var uri = "/api/filespecification";

    var $grid = $('#grid').dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: 'id',
            loadUrl: uri,
            updateUrl: uri,
        }),
        remoteOperations: true,
        allowColumnResizing: true,
        showBorders: true,
        columnResizingMode: "nextColumn",
        columnMinWidth: 50,
        columnAutoWidth: true,
        columns: [
            { dataField: 'fileNumber', caption: 'File Number' },
            { dataField: 'fileName', caption: 'File Name' },
            {
                dataField: 'isRetired',
                caption: 'Retired',
                dataType: 'boolean',
                visible: true,
                showEditorAlways: false,
                trueText: 'Yes',
                falseText: 'No',
                customizeText: function (cellInfo) {
                    if (cellInfo.value) return 'Yes';

                    return 'No';
                },
            },
            { dataField: 'section', caption: 'Section' },
            { dataField: 'supportGroup', caption: 'Support Group', },
            { dataField: 'application', caption: 'Application', },
            { dataField: 'collection', caption: 'Collection' },
            { dataField: 'generationUserGroup', caption: 'Generation Group' },
            { dataField: 'approvalUserGroup', caption: 'Approval Group' },
            { dataField: 'submissionUserGroup', caption: 'Submission Group' },
            {
                width: 120,
                alignment: 'center',
                cellTemplate: function (container, options) {

                    $('<a/>').addClass('btn btn-default btn-sm btn-grid')
                        .text('Edit')
                        .on('dxclick',
                            function (e) {
                                editFileSpecification($(this), options.data);
                            })
                        .appendTo(container);

                    if (options.data.canRetire) {
                        $('<a/>').addClass('btn btn-default btn-sm btn-grid')
                            .text('Retire')
                            .on('dxclick',
                                function(e) {
                                    retire($(this), options.data);
                                })
                            .appendTo(container);
                    }
                    if (options.data.canActivate) {
                        $('<a/>').addClass('btn btn-default btn-sm btn-grid')
                            .text('Activate')
                            .on('dxclick',
                                function (e) {
                                    activate($(this), options.data);
                                })
                            .appendTo(container);
                    }

                }
            },
        ],
        wordWrapEnabled: true,
        columnChooser: {
            enabled: true
        },
        stateStoring: {
            enabled: true,
            type: "localStorage",
            storageKey: "gridFileSpecificationFilterStorage"
        },
        filterRow: {
            visible: true
        },
        headerFilter: {
            visible: true
        },
        groupPanel: {
            visible: true
        },
        scrolling: {
            mode: "virtual",
            rowRenderingMode: "virtual",
        },
        paging: {
            pageSize: 20
        },
        height: 400,
        sortByGroupSummaryInfo: [
            {
                summaryItem: "count"
            }
        ],
        summary: {
            totalItems: [
                {
                    column: "id",
                    displayFormat: '{0} Total File Specifications',
                    summaryType: 'count',
                    showInGroupFooter: true,
                    showInColumn: 'FileNumber'
                },
            ],
            groupItems: [
                {
                    summaryType: "count",
                    displayFormat: '{0} File Specifications',
                },
            ]
        },
        onToolbarPreparing: function (e) {
            var dataGrid = e.component;

            e.toolbarOptions.items.unshift(
                {
                    location: "after",
                    widget: "dxButton",
                    options: {
                        text: "Collapse All",
                        width: 136,
                        onClick: function (e) {
                            var expanding = e.component.option("text") === "Expand All";
                            dataGrid.option("grouping.autoExpandAll", expanding);
                            e.component.option("text", expanding ? "Collapse All" : "Expand All");
                        }
                    }
                },
                {
                    location: "after",
                    widget: "dxButton",
                    options: {
                        icon: "refresh",
                        onClick: function () {
                            dataGrid.refresh();
                        }
                    }
                },
                {
                    location: "after",
                    widget: "dxButton",
                    options: {
                        icon: "fa fa-undo",
                        onClick: function () {
                            dataGrid.state({});
                        }
                    }
                },
            );
        }
    }).dxDataGrid("instance");


    function activate(container, data) {
        var id = data.id;
        $.ajax({
            url: '/api/filespecification/activate/' + id,
            type: 'POST',
            success: function (data) {
                $grid.refresh();
                //TODO: Toast success retire action
            },
            error: function (err) {
                console.log('err', err);
                //TODO: Toast error retire action
            }
        }).always(function () {

        });
    }

    function retire(container, data) {
        var id = data.id; 
        $.ajax({
            url: '/api/filespecification/retire/' + id,
            type: 'POST',
            success: function (data) {
                $grid.refresh();
                //TODO: Toast success retire action
            },
            error: function (err) {
                console.log('err', err);
                //TODO: Toast error retire action
            }
        }).always(function () {
            
        });
    }

    function editFileSpecification(container, data) {
        var title = 'Edit File Specification';
        var url = '/home/editfileSpecification/' + data.id;
        var postUrl = '/api/filespecification/' + data.id;

        BootstrapDialog.show({
            type: BootstrapDialog.TYPE_WARNING, 
            size: window.BootstrapDialog.SIZE_WIDE,
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
                    label: 'Save',
                    cssClass: 'btn-primary',
                    action: function (dialogRef) {
                        var data = $('form').serializeJSON(); 
                        $.ajax({
                            contentType: 'application/json; charset=utf-8',
                            type: "PUT",
                            url: postUrl,
                            data: JSON.stringify(data),
                            //data: $('form').serialize(),
                            dataType: 'json',
                            success: function (response) {
                                //TODO: toast success
                                dialogRef.close();
                                $grid.refresh();
                            },
                            error: function (error) {
                                //TODO: toast error
                                console.log('error', error);
                            },
                            complete: function (status) {
                                console.log('complete', status);
                            }
                        });
                    }
                }
            ]
        });

    }

})();


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



$(function () {
    console.log('submission view ready');
    var uri = "/api/submission";

    var $grid = $('#grid').dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: 'id',
            loadUrl: uri,
            updateUrl: uri,
        }),
        remoteOperations: true,
        allowColumnResizing: true,
        showBorders: true,
        columnResizingMode: "nextColumn",
        columnMinWidth: 50,
        columnAutoWidth: true,
        columns: [
            {
                width: 50,
                type: "buttons",
                buttons: ["edit", "delete", {
                    text: "History",
                    icon: "fa fa-history",
                    hint: "History",
                    onClick: function (e) {
                        // Execute your command here
                        showHistory(e);
                    }
                }]
            },
            { dataField: 'fileNumber', caption: 'File Number' },
            { dataField: 'fileName', caption: 'File Name' },
            { dataField: 'submissionStateDisplay', caption: 'Status' },
            { dataField: 'currentAssignee', caption: 'Assigned' },
            { dataField: 'lastUpdatedFriendly', caption: 'Last Update' },
            { dataField: 'deadlineDate', caption: 'Submission Deadline', dataType: 'date', },
            { dataField: 'submissionDate', caption: 'Date Submitted', dataType: 'date', },
            { dataField: 'displayDataYear', caption: 'Data Year' },
            { dataField: 'section', caption: 'Section' },
            {
                dataField: 'isSEA', caption: 'SEA',
                dataType: 'boolean',
                visible: true,
                showEditorAlways: false,
                trueText: 'Yes',
                falseText: 'No',
                customizeText: function (cellInfo) {
                    if (cellInfo.value) return 'Yes';

                    return 'No';
                },
            },
            {
                dataField: 'isLEA', caption: 'LEA', dataType: 'boolean',
                visible: true,
                showEditorAlways: false,
                trueText: 'Yes',
                falseText: 'No',
                customizeText: function (cellInfo) {
                    if (cellInfo.value) return 'Yes';

                    return 'No';
                },
            },
            {
                dataField: 'isSCH', caption: 'SCH',
                dataType: 'boolean',
                visible: true,
                showEditorAlways: false,
                trueText: 'Yes',
                falseText: 'No',
                customizeText: function (cellInfo) {
                    if (cellInfo.value) return 'Yes';

                    return 'No';
                },
            },
            {
                width: 200,
                alignment: 'center',
                cellTemplate: function (container, options) {
                    if (options.data.canStart) {
                        $('<a/>').addClass('btn btn-default btn-sm btn-grid')
                            .text('Start')
                            .on('dxclick',
                                function (e) {
                                    startWorkFlow($(this), options.data);
                                })
                            .appendTo(container);
                    }

                    if (options.data.canCancel) {
                        $('<a/>').addClass('btn btn-default btn-sm btn-grid')
                            .text('Cancel')
                            .on('dxclick',
                                function () {
                                    cancelWorkFlow($(this), options.data);
                                })
                            .appendTo(container);
                    }

                    if (options.data.canReview) {
                        $('<a/>').addClass('btn btn-primary btn-sm btn-grid')
                            .text('Review File')
                            .attr('href', '/review/' + options.data.dataYear + '/' + options.data.fileNumber)
                            .attr('target', '_blank')
                            .appendTo(container);
                    }

                    if (options.data.canReopen) {
                        $('<a/>').addClass('btn btn-primary btn-sm btn-grid')
                            .text('Reopen')
                            .on('dxclick',
                                function (e) {
                                    reopenSubmission($(this), options.data);
                                })
                            .appendTo(container);
                    }

                    if (options.data.canWaiver) {
                        $('<a/>').addClass('btn btn-default btn-sm btn-grid')
                            .text('Waiver')
                            .on('dxclick',
                                function (e) {
                                    waiverWorkFlow($(this), options.data);
                                })
                            .appendTo(container);
                    }



                }
            },
        ],
        wordWrapEnabled: true,
        columnChooser: {
            enabled: true
        },
        stateStoring: {
            enabled: true,
            type: "localStorage",
            storageKey: "gridSubmissionFilterStorage"
        },
        filterRow: {
            visible: true
        },
        headerFilter: {
            visible: true
        },
        groupPanel: {
            visible: true
        },
        scrolling: {
            mode: "virtual",
            rowRenderingMode: "virtual",
        },
        paging: {
            pageSize: 20
        },
        height: 400,
        sortByGroupSummaryInfo: [{
            summaryItem: "count"
        }],
        summary: {
            totalItems: [
                {
                    column: "id",
                    displayFormat: '{0} Total Submissions',
                    summaryType: 'count',
                    showInGroupFooter: true,
                    showInColumn: 'FileNumber'
                },
            ],
            groupItems: [
                {
                    summaryType: "count",
                    displayFormat: '{0} Submissions',
                },

            ]
        },
        onToolbarPreparing: function (e) {
            var dataGrid = e.component;

            e.toolbarOptions.items.unshift(
                {
                    location: "after",
                    widget: "dxButton",
                    options: {
                        text: "Collapse All",
                        
                        width: 136,
                        onClick: function (e) {
                            var expanding = e.component.option("text") === "Expand All";
                            dataGrid.option("grouping.autoExpandAll", expanding);
                            e.component.option("text", expanding ? "Collapse All" : "Expand All");
                        }
                    }
                },
                {
                    location: "after",
                    widget: "dxButton",
                  
                    options: {
                        icon: "refresh",
                        hint: 'Refresh Data', 
                        onClick: function () {
                            dataGrid.refresh();
                        }
                    }
                },
                {
                    location: "after",
                    widget: "dxButton",
                    options: {
                        icon: "fa fa-undo",
                        hint: 'Reset Grid',
                        onClick: function () {
                            dataGrid.state({});
                        }
                    }
                },
            );
        }
    }).dxDataGrid("instance");

    function showHistory(e) {
        var title = 'History';
        var url = '/history/' + e.row.data.id;
        console.log(e.row.data);
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

    function startWorkFlow(container, data) {
        var id = data.id;
        console.log('id', id);
        $.ajax({
            url: '/api/submission/start/' + id,
            type: 'POST',
            success: function (response) {
                //TODO: toast success
                console.log('success', response);
                $grid.refresh();
            },
            error: function (error) {
                //TODO: toast error
                console.log('error', error);
            },
            complete: function (status) {
                console.log('complete', status);
            }
        });


    }

    function cancelWorkFlow(container, data) {
        var id = data.id;
        
        $.ajax({
            url: '/api/submission/cancel/' + id,
            type: 'POST',
            success: function (response) {
                //TODO: toast success
                console.log('success', response);
                $grid.refresh();
            },
            error: function (error) {
                //TODO: toast error
                console.log('error', error);
            },
            complete: function (status) {
                console.log('complete', status);
            }
        });

    }

    function reopenSubmission(container, data) {

        var url = '/home/reopen/' + data.id;
        var title = 'Reopen Reason';
        var postUrl = '/api/submission/reopen/' + data.id;

        BootstrapDialog.show({
            size: window.BootstrapDialog.SIZE_WIDE,
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
                    label: 'Save',
                    cssClass: 'btn-primary',
                    action: function (dialogRef) {
                        var data = $('form').serializeJSON(); 

                        $.ajax({
                            contentType: 'application/json;',
                            type: "POST",
                            url: postUrl,
                            data: JSON.stringify(data),
                            dataType: 'json',
                            success: function (response) {
                                console.log('success', response);
                                //TODO: toast success
                                dialogRef.close();
                                $grid.refresh();
                            },
                            error: function (error) {
                                //TODO: toast error
                                console.log('error', error);
                            },
                            complete: function (status) {
                                console.log('complete', status);
                            }
                        });
                    }
                }
            ]
        });

    }

    function waiverWorkFlow(container, data) {
        var title = 'Waiver Reason';
        var url = '/home/waiver/' + data.id;
        var postUrl = '/api/submission/waive/' + data.id;

        BootstrapDialog.show({
            size: window.BootstrapDialog.SIZE_WIDE,
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
                    label: 'Save',
                    cssClass: 'btn-primary',
                    action: function(dialogRef) {
                        var data = $('form').serializeJSON(); 

                        $.ajax({
                            contentType: 'application/json; charset=utf-8',
                            type: "POST", 
                            url: postUrl, 
                            data: JSON.stringify(data), 
                            success: function (response) {
                                //TODO: toast success
                                dialogRef.close();
                                $grid.refresh();
                            },
                            error: function (error) {
                                //TODO: toast error
                                console.log('error', error);
                            },
                            complete: function (status) {
                                console.log('complete', status);
                            }
                        });
                    }
                }
            ]
        });

    }
    
});