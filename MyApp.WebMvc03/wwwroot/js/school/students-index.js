'use strict';

var StudentList = (function ($) {
    return {
        submitForm: function () {
            $('#sort-filter-form').submit();
        },

        setFilterByInputType: function(filterElem) {
            var filterBy = Number.parseInt($(filterElem).find(":selected").val());
            var currentInputType = $('#FilterValue').attr('type');
            
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
        },

        filterByHasChanged: function (filterElem) {
            this.setFilterByInputType(filterElem);
        },

        submitSort: function (newSortBy) {
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
        },

        submitGoToPage: function (newPageIndex) {
            $('#PageIndex').val(newPageIndex);
            this.submitForm();
        },
    };

}(window.jQuery));
