
(function () {

    var uriCurrent = "/api/workitem/mark";
    var uriComplete = "/api/workitem/finished/mark";

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
            { dataField: 'assignedDate', caption: 'Assigned Date', dataType: 'datetime', width: 150, },
            { dataField: 'dueDate', caption: 'Due Date', dataType: 'datetime', width: 150, },
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
            { dataField: 'assignedDate', caption: 'Assigned Date', dataType: 'datetime' },
            { dataField: 'dueDate', caption: 'Due Date', dataType: 'datetime' },
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
        var postUrl = '/api/workitem/reporterror/'; // + data.id;

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

                        $('form').submit();


                        //var data = $('form').serializeJSON();
                        //var formData = new FormData($('form')[0]);
                        ////formData.append('id', data.id);
                        ////formData.append('description', data.description);

                        ////var files = document.getElementById('files').files;
                        //debugger;
                        ////data.files = []; 

                        ////if (files.length > 0) {
                        ////    for (var i = 0; i < files.length; i++) {
                        ////        formData.append('files', files[i]);
                        ////        data.files.push(files[i]);
                        ////    }
                        ////}
                        //postUrl = '/api/workitem/reporterror';
                        //$.ajax({
                        //    type: "POST",
                        //    url: postUrl,
                        //    //data: JSON.stringify(formData),
                        //    data: data,
                        //    contentType: false,
                        //    processData: false,
                        //    success: function (response) {

                        //    },
                        //    error: function (error) {

                        //    },
                        //    complete: function () {

                        //    }
                        //});


                        //WORKING 
                        //var formData = new FormData();
                        //formData.append('id', data.id);
                        //formData.append('description', data.description);

                        //var files = document.getElementById('files').files;
                        //if (files.length > 0) {
                        //    for (var i = 0; i < files.length; i++) {
                        //        formData.append('files', files[i]);
                        //    }
                        //}
                        //postUrl = '/api/workitem/reporterror2';
                        //$.ajax({
                        //    type: "POST",
                        //    url: postUrl,
                        //    data: formData,
                        //    contentType: false,
                        //    processData: false,
                        //    success: function (response) {

                        //    },
                        //    error: function (error) {

                        //    },
                        //    complete: function () {

                        //    }
                        //});


                    }
                }
            ]
        });

    }

    //$('form').submit(function (e) {
    //    e.preventDefault();
    //    console.log('submit form');
    //}); 

})();

$(document).on('submit',
    function (e) {
        e.preventDefault();
        console.log('submit form');

        var data = $('form').serializeJSON();
        var formData = new FormData($('form')[0]);

        debugger;
        //data.files = []; 

        if (files.length > 0) {
            for (var i = 0; i < files.length; i++) {
                formData.append('file', files[i]);
                //data.files.push(files[i]);
            }
        }
        var postUrl = '/home/reporterror';
        $.ajax({
            type: "POST",
            url: postUrl,
            //data: JSON.stringify(formData),
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {

            },
            error: function (error) {

            },
            complete: function () {

            }
        });

    });


