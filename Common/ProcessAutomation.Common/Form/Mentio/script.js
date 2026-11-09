if (!String.prototype.format) {
  String.prototype.format = function() {
    var args = arguments;
    return this.replace(/{(\d+)}/g, function(match, number) { 
      return typeof args[number] != 'undefined'
        ? args[number]
        : match
      ;
    });
  };
}

var app = angular.module('app', ['mentio']);

app.controller('ctrl', function ($scope) {

    $scope.people = [
		 { label: 'Joe' },
         { label: 'Mike' },
         { label: 'Diane' }
        // { label: '[@Joe]' },
        // { label: '[@Mike]' },
        // { label: '[@Diane]' }
    ]

    $scope.myval = '';
});