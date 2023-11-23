angular.module('myApp', [])
    .filter('numberFormatter', function () {
        return function (input) {
            return input.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        };
    });