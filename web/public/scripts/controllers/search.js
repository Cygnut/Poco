// Get the poco module and add a controller to it.
angular.module('poco').controller("search", function($scope, $http) {
    
    var c = new BackendClient($http);
    
    $scope.runQuery = function()
    {
        c.searchEntities(
        {
            fragment: $scope.fragment,
            search_mode: $scope.searchMode
        },
        function(data) {
            $scope.items = data.data.items;
        });
    }
    
    // Don't bother initialising $scope.items to avoid an unnecessary 
    // server hit for the case that the search isn't used.
    $scope.fragment = "";
    $scope.searchMode = 1;  // Just search by name for now, as we're not showing the blurb.
    $scope.items = [];
});