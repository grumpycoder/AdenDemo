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
        wordWrapEnabled: true,
        columnChooser: {
            enabled: true
        },
        stateStoring: {
            enabled: true,
            type: "localStorage",
            storageKey: "gridFilterStorage"
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

    function showHistory(e) {
        console.log('showHistory', e);
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

        window.$toggleWorkingButton(container);

        $.ajax({
            url: '/api/submission/cancel/' + id,
            type: 'POST'
        })
            .done(function () {
                $grid.refresh().done(function (e) { console.log('done', e) });
                window.$log.success('Cancelled assignments');
            })
            .fail(function (err) {
                window.$log.error('Something went wrong: ' + err.responseJSON.message);
            })
            .always(function () {
                window.$toggleWorkingButton(container, 'off');
            });
    }

    function reopenSubmission(container, data) {

        var url = '/reopenaudit/' + data.id;
        var title = 'Reopen Reason';
        var postUrl = '/api/submission/reopen/' + data.id;

        $.ajax({
            url: url,
            type: 'GET',
            success: function (data) {
                window.BootstrapDialog.show({
                    size: window.BootstrapDialog.SIZE_WIDE,
                    draggable: true,
                    title: title,
                    message: $('<div></div>').load(url,
                        function (resp, status, xhr) {
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
                                window.$showModalWorking();
                                $.ajax({
                                    type: "POST",
                                    url: postUrl,
                                    data: $('form').serialize(),
                                    dataType: 'json',
                                    success: function (response) {
                                        $grid.refresh().done(function (e) { console.log('done', e) });
                                        window.$log.success('ReOpened submission');
                                    },
                                    error: function (error) {
                                        console.log('error', error);
                                        window.$log.error('Error: ' + error.responseJSON.message);
                                    },
                                    complete: function () {
                                        dialogRef.close();
                                        window.$hideModalWorking();
                                    }
                                });

                            }
                        }
                    ]
                });
            },
            error: function (err) {
                window.$log.error('Error showing audit entry');
            }
        })

    }

    function waiverWorkFlow(container, data) {
        var title = 'Waiver Reason';
        var url = '/home/audit/' + data.id;
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
                    action: function (dialogRef) {
                        $.ajax({
                            contentType: 'application/json; charset=utf-8',
                            type: "POST",
                            url: postUrl,
                            data: JSON.stringify({ 'model': $('form').serialize() }),
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

}); 