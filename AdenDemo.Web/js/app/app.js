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
                            .text('Reject File')
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
                            .text('View Errors')
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
        $toggleWorkingButton(container);

        $.ajax({
            url: uri,
            type: 'POST',
            success: function (data) {
                $gridCurrent.refresh();
                $gridComplete.refresh();
                console.log('data', data);
                toastr.success('Completed task for ' + data.fileName + ' (' + data.fileNumber + ')');
            },
            error: function (err) {
                toastr.error('Error completing task');
            }
        }).always(function () {
            $toggleWorkingButton(container);
        });
    }

    function reject(container, data) {
        var uri = '/api/workitem/reject/' + data.id;
        $toggleWorkingButton(container);

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
            $toggleWorkingButton(container);
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
        console.log('show report errors');
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
        allowColumnReordering: true,
        showBorders: true,
        wordWrapEnabled: true,
        'export': {
            enabled: true,
            fileName: "FileSpecifications",
            allowExportSelectedData: false, 
            icon: 'fa fa-trash'
        },
        stateStoring: {
            enabled: true,
            type: "localStorage",
            storageKey: "gridFileSpecificationFilterStorage"
        },
        filterRow: { visible: true },
        headerFilter: { visible: true },
        groupPanel: { visible: true },
        scrolling: { mode: "virtual", rowRenderingMode: "virtual" },
        paging: { pageSize: 20 },
        height: 650,
        columnChooser: { enabled: true },
        columnResizingMode: "nextColumn",
        columnMinWidth: 50,
        columnAutoWidth: true,
        columns: [
            { dataField: 'fileNumber', caption: 'File Number', dataType: 'string' },
            { dataField: 'fileName', caption: 'File Name', dataType: 'string' },
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
            { dataField: 'section', caption: 'Section', dataType: 'string' },
            { dataField: 'supportGroup', caption: 'Support Group', dataType: 'string' },
            { dataField: 'application', caption: 'Application', dataType: 'string'},
            { dataField: 'collection', caption: 'Collection', dataType: 'string' },
            { dataField: 'generationGroup', caption: 'Generation Group', dataType: 'string' },
            { dataField: 'approvalGroup', caption: 'Approval Group', dataType: 'string' },
            { dataField: 'submissionGroup', caption: 'Submission Group', dataType: 'string' },
            {
                dataField: 'generators', caption: 'Generators', dataType: 'string', 
                cellTemplate: function(container, options) {
                    options.data.generators.forEach(function(item) { $('<span>' + item + '</span><br />').appendTo(container) });
                }, 
                allowFiltering:false, 
                calculateDisplayValue: function(rowData) {
                    return rowData.generators.join(", ");
                }
            },
            {
                dataField: 'approvers', caption: 'Approvers', dataType: 'string', 
                cellTemplate: function(container, options) {
                    options.data.approvers.forEach(function(item) { $('<span>' + item + '</span><br />').appendTo(container) });
                }, 
                allowFiltering:false, 
                calculateDisplayValue: function(rowData) {
                    return rowData.approvers.join(", ");
                }
            },
            {
                dataField: 'submitters', caption: 'Submitters', dataType: 'string',
                cellTemplate: function(container, options) {
                    options.data.submitters.forEach(function(item) { $('<span>' + item + '</span><br />').appendTo(container) });
                }, 
                allowFiltering:false, 
                calculateDisplayValue: function(rowData) {
                    return rowData.submitters.join(", ");
                }
            },
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
                        hint: 'Refresh', 
                        onClick: function () {
                            dataGrid.refresh();
                        }
                    }
                },
                {
                    location: "after",
                    widget: "dxButton",
                    options: {
                        icon: "clearformat",
                        hint: 'Clear filters', 
                        onClick: function () {
                            dataGrid.clearFilter();
                        }
                    }
                },
                {
                    location: "after",
                    widget: "dxButton",
                    options: {
                        icon: "clearsquare",
                        hint: 'Reset grid to default', 
                        onClick: function () {
                            dataGrid.state({});
                        }
                    }
                }
                
            );
        }
    }).dxDataGrid("instance");


    function activate(container, data) {
        var id = data.id;
        $toggleWorkingButton(container);

        $.ajax({
            url: '/api/filespecification/activate/' + id,
            type: 'POST',
            success: function (data) {
                $grid.refresh();
                toastr.success('Activated file for ' + data.fileName + ' (' + data.fileNumber + ')');
            },
            error: function (err) {
                toastr.error('Error activating file');
            }
        }).always(function () {
            $toggleWorkingButton(container);
        });
    }

    function retire(container, data) {
        var id = data.id; 
        $toggleWorkingButton(container);

        $.ajax({
            url: '/api/filespecification/retire/' + id,
            type: 'POST',
            success: function (data) {
                toastr.warning('Retired file for ' + data.fileName + ' (' + data.fileNumber + ')');
                $grid.refresh();
            },
            error: function (err) {
                toastr.error('Error retiring file');
            }
        }).always(function () {
            $toggleWorkingButton(container);
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
                        dialogRef.enableButtons(false);
                        dialogRef.setClosable(false);
                        $showModalWorking($('.panel-body'));

                        var data = $('form').serializeJSON(); 
                        $.ajax({
                            contentType: 'application/json; charset=utf-8',
                            type: "PUT",
                            url: postUrl,
                            data: JSON.stringify(data),
                            dataType: 'json',
                            success: function (data) {
                                toastr.success('Saved changes for ' + data.fileName + ' (' + data.fileNumber + ')');
                                dialogRef.close();
                                $grid.refresh();
                            },
                            error: function (error) {
                                toastr.error('Error saving file changes');
                            },
                            complete: function (status) {
                                
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
        wordWrapEnabled: true,
        'export': {
            enabled: true,
            fileName: "Submissions",
            allowExportSelectedData: false, 
            icon: 'fa fa-trash'
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
        height: 650,
        columnResizingMode: "nextColumn",
        columnMinWidth: 50,
        columnAutoWidth: true,
        columnChooser: {
            enabled: true
        },
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
                visible: false,
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
                visible: false,
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
                visible: false,
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
                        $('<a/>').addClass('btn btn-default btn-sm btn-grid')
                            .text('Review File')
                            .attr('href', '/review/' + options.data.dataYear + '/' + options.data.fileNumber)
                            .attr('target', '_blank')
                            .appendTo(container);
                    }

                    if (options.data.canReopen) {
                        $('<a/>').addClass('btn btn-default btn-sm btn-grid')
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
                        hint: 'Refresh', 
                        onClick: function () {
                            dataGrid.refresh();
                        }
                    }
                },
                {
                    location: "after",
                    widget: "dxButton",
                    options: {
                        icon: "clearformat",
                        hint: 'Clear filters', 
                        onClick: function () {
                            dataGrid.clearFilter();
                        }
                    }
                },
                {
                    location: "after",
                    widget: "dxButton",
                    options: {
                        icon: "clearsquare",
                        hint: 'Reset grid to default', 
                        onClick: function () {
                            dataGrid.state({});
                        }
                    }
                }
                
            );
        }
    }).dxDataGrid("instance");

    function showHistory(e) {
        var title = 'History';
        var url = '/history/' + e.row.data.id;

        BootstrapDialog.show({
            size: window.BootstrapDialog.SIZE_WIDE,
            draggable: true,
            title: title,
            message: $('<div></div>').load(url, function (resp, status, xhr) {
                if (status === 'error') {
                    toastr.error('Error retrieving history');
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
        $toggleWorkingButton(container);
        $.ajax({
            url: '/api/submission/start/' + id,
            type: 'POST',
            success: function (data) {
                toastr.success('Started submission process for ' + data.fileName + ' (' + data.fileNumber + ')');
                $grid.refresh();
            },
            error: function (error) {
                toastr.error('Error starting submission process');
            },
            complete: function (status) {
                
                $toggleWorkingButton(container);
            }
        });


    }

    function cancelWorkFlow(container, data) {
        var id = data.id;
        $toggleWorkingButton(container);
        $.ajax({
            url: '/api/submission/cancel/' + id,
            type: 'POST',
            success: function (response) {
                toastr.warning('Cancelled submission process for ' + response.fileName + ' (' + response.fileNumber + ')');
                $grid.refresh();
            },
            error: function (error) {
                toastr.error('Error cancelling submission process');
            },
            complete: function (status) {
                $toggleWorkingButton(container);
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
                        dialogRef.enableButtons(false);
                        dialogRef.setClosable(false);
                        $showModalWorking($('.panel-body'));

                        var data = $('form').serializeJSON();

                        $.ajax({
                            contentType: 'application/json;',
                            type: "POST",
                            url: postUrl,
                            data: JSON.stringify(data),
                            dataType: 'json',
                            success: function (response) {
                                toastr.success('Reopened submission process for ' + response.fileName + ' (' + response.fileNumber + ')');
                                dialogRef.close();
                                $grid.refresh();
                            },
                            error: function (error) {
                                toastr.error('Error reopening submission process');
                            },
                            complete: function (status) {
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
                        dialogRef.enableButtons(false);
                        dialogRef.setClosable(false);
                        $showModalWorking($('.panel-body'));

                        var data = $('form').serializeJSON(); 

                        $.ajax({
                            contentType: 'application/json; charset=utf-8',
                            type: "POST", 
                            url: postUrl, 
                            data: JSON.stringify(data), 
                            success: function (response) {
                                toastr.success('Waived submission process for ' + response.fileName + ' (' + response.fileNumber + ')');
                                dialogRef.close();
                                $grid.refresh();
                            },
                            error: function (error) {
                                toastr.error('Error waiving submission process');
                            },
                            complete: function (status) {
                            }
                        });
                    }
                }
            ]
        });

    }
    
});