'use strict';

var DataTable = (function ($) {

    var TempObj = function () { };

    // Inherit from CommonDataTable defined in js/common/data-table.js
    TempObj.prototype = new CommonDataTable();

    // Override parent function
    TempObj.prototype.setFilterByInputType = function (filterElem) {
        var filterBy = Number.parseInt($(filterElem).find(":selected").val());
        var currentInputType = $('#FilterValue').attr('type');
        
        // TODO: Override this function in the child object and change input type as required
        if ((filterBy == 3 || filterBy == 4)
            && currentInputType != 'date') {
            $('#FilterValue').attr('type', 'date');
            $('#FilterValue').val('');
        } else if (!(filterBy == 3 || filterBy == 4)
            && currentInputType != 'text') {
            $('#FilterValue').attr('type', 'text');
            $('#FilterValue').val('');
        }

        if (filterBy == 0) {
            $('#FilterValue').attr('readonly', true);
            $('#FilterValue').val('');
        } else {
            $('#FilterValue').attr('readonly', false);
        }
    };

    return TempObj;

}(window.jQuery));

(function ($) {
    $(document).ready(function () {

        var dataTable = new DataTable();

        $('#FilterBy').change(function () {
            dataTable.filterByHasChanged(this);
        });

        $('#BtnSubmitSearch').click(function () {
            dataTable.submitForm();
        });

        $('#SortField1').click(function () {
            dataTable.submitSort(sortFields.SortField1);
        });

        $('#SortField2').click(function () {
            dataTable.submitSort(sortFields.SortField2);
        });

        $('#SortField3').click(function () {
            dataTable.submitSort(sortFields.SortField3);
        });

        $('#BtnPreviouPage').click(function () {
            dataTable.submitGoToPage(prevPageNo);
            return false;
        });

        $('#BtnNextPage').click(function () {
            dataTable.submitGoToPage(nextPageNo);
            return false;
        });

        $('#PageSize').change(function () {
            dataTable.submitForm();
        });

    });
}(window.jQuery));
