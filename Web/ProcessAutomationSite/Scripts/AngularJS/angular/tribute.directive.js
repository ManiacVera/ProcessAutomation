app.directive('tribute', function (tributeService) {
    return {
        restrict: 'A',

        link: function (scope, element) {
            if (!element[0]._tributeAttached) {
                tributeService.tribute.attach(element[0]);
                element[0]._tributeAttached = true;
            }

            scope.$on('$destroy', function () {
                tributeService.tribute.detach(element[0]);
                element[0]._tributeAttached = false;
            });
        }
    };
});