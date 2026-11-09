app.directive('contenteditable', function () {
  return {
    restrict: "A",
    require: 'ngModel',
    controller: function ($scope) {
      //$scope.username = 'Adan';
      //$scope.printValue = function () {
      //  console.log($scope.username);
      //}
    },
    link: function (scope, element, attrs, myController) {

      element.on('keyup', function () {
        scope.$apply(updateViewModel);
      });

      function updateViewModel() {
        var htmlValue = element.text()
        myController.$setViewValue(htmlValue);
        //scope.printValue();
      }
      myController.$render = updateHtml

      function updateHtml() {
        var viewModelValue = myController.$viewValue
        element.text(viewModelValue);

      }
    }
  };
});