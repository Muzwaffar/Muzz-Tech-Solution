/*
      Define the adapter so that it's reusable
*/
$.fn.select2.amd.define('select2/selectAllAdapter', [
    'select2/utils',
    'select2/dropdown',
    'select2/dropdown/attachBody'
], function (Utils, Dropdown, AttachBody) {

    function SelectAll() { }
    SelectAll.prototype.render = function (decorated) {
        var self = this;
        var $rendered = decorated.call(this),
            $selectAll = $(
                '<button class="btn-c-secondary-small m-2" id="' + this.$element[0].id +'_select" type="button" ><i class="fa fa-check-square-o"></i> Select All</button>'
            ),
            $unselectAll = $(
                '<button class="btn-c-secondary-small m-2" id="' + this.$element[0].id +'_unselect" type="button"><i class="fa fa-square-o"></i> Unselect All</button>'
            ),
            $btnContainer = $('<div class="d-flex">').append($selectAll).append($unselectAll);
        if (!this.$element.prop("multiple")) {
            // this isn't a multi-select -> don't add the buttons!
            return $rendered;
        }
        $rendered.find('.select2-dropdown').prepend($btnContainer);
        $selectAll.on('click', function (e) {
            var $results = $rendered.find('.select2-results__option[aria-selected=false]');
            $results.each(function () {
                var dat = {
                    id: $(this)[0].id.replace('select2-select-entity-result-yhcz-', ''),
                    text: $(this)[0].textContent
                }
                self.trigger('select', {
                    data: dat
                });
            });
            self.trigger('close');
        });

        $unselectAll.on('click', function (e) {
            var selectId = $(this)[0].id.replace("_unselect", "");
            $("#" + selectId).val(null).trigger("change");
            self.trigger('close');
        });

        return $rendered;
    };

    return Utils.Decorate(
        Utils.Decorate(
            Dropdown,
            AttachBody
        ),
        SelectAll
    );

});