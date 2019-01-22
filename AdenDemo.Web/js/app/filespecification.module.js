
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
                width: 70,
                alignment: 'center',
                cellTemplate: function (container, options) {

                    $('<a/>').addClass('btn btn-default btn-sm btn-grid')
                        .text('Edit')
                        .on('dxclick',
                            function (e) {
                                editFileSpecification($(this), options.data);
                            })
                        .appendTo(container);

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


    function editFileSpecification(container, data) {
        var title = 'Edit File Specification';
        var url = '/home/editfileSpecification/' + data.id;
        var postUrl = '/api/filespecification/' + data.id;

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
                            type: "PUT",
                            url: postUrl,
                            data: JSON.stringify({ 'dto': $('form').serialize() }),
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