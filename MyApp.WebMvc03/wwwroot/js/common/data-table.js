'use strict';

var CommonDataTable = (function ($) {
    var TempObj = function () { };

    TempObj.prototype.submitForm = function () {
        $('#DataTableForm').submit();
    };

    TempObj.prototype.setFilterByInputType = function (filterElem) {
        var filterBy = Number.parseInt($(filterElem).find(":selected").val());
        var currentInputType = $('#FilterValue').attr('type');
        
        // TODO: Override this function in the child object and change input type as required
        //if ((filterBy == 3 || filterBy == 4)
        //    && currentInputType != 'date') {
        //    $('#FilterValue').attr('type', 'date');
        //    $('#FilterValue').val('');
        //} else if (!(filterBy == 3 || filterBy == 4)
        //    && currentInputType != 'text') {
        //    $('#FilterValue').attr('type', 'text');
        //    $('#FilterValue').val('');
        //}

        if (filterBy == 0) {
            $('#FilterValue').attr('readonly', true);
            $('#FilterValue').val('');
        } else {
            $('#FilterValue').attr('readonly', false);
        }
    };

    TempObj.prototype.filterByHasChanged = function (filterElem) {
        this.setFilterByInputType(filterElem);
    };

    TempObj.prototype.submitSort = function (newSortBy) {
        var sortBy = Number.parseInt($('#SortBy').val());
        var sortAscending = $('#SortAscending').val().toLowerCase() == 'true';

        if (sortBy === newSortBy) {
            $('#SortAscending').val(!sortAscending);
        }
        else {
            $('#SortAscending').val(true);
        }

        $('#SortBy').val(newSortBy);

        this.submitForm();
    };

    TempObj.prototype.submitGoToPage = function (newPageIndex) {
        $('#PageIndex').val(newPageIndex);
        this.submitForm();
    };

    return TempObj;

}(window.jQuery));
