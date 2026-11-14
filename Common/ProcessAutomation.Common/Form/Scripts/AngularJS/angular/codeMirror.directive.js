
app.directive('codeMirrorEditor', ['codeMirrorService', function (codeMirrorService) {

    return {
        restrict: 'A',
        require: 'ngModel',

        link: function (scope, element, attrs, ngModel) {

            let destroyed = false;

            function renderValue() {

                if (destroyed) {
                    return;
                }

                const value = ngModel.$viewValue == null
                    ? ''
                    : ngModel.$viewValue;

                if (codeMirrorService.isActiveElement(element[0])) {

                    const instance = codeMirrorService.getActiveInstance();

                    if (instance && instance.getValue() !== value) {
                        instance.setValue(value);
                    } else if (!instance) {
                        element.val(value);
                    }

                } else {
                    element.val(value);
                }
            }

            function activateEditor() {

                if (destroyed) {
                    return;
                }

                codeMirrorService.attach(element, ngModel, scope);
            }

            ngModel.$render = renderValue;

            element.on('focus', activateEditor);

            scope.$evalAsync(function () {
                renderValue();
            });

            scope.$on('$destroy', function () {

                destroyed = true;

                element.off('focus', activateEditor);

                codeMirrorService.detach(element);
            });

        }
    };

}]);
