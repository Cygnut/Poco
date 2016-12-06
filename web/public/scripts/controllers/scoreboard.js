// Get the poco module and add a controller to it.
angular.module('poco').controller("scoreboard", function($scope, $http) {
    
    var c = new BackendClient($http);
    
    $scope.setRange = function()
    {
        c.getTopScoredEntities(
        { 
            offset: $scope.offset,
            limit: $scope.limit
        },
        function(data) {
            $scope.items = data.data.items;
        });
    }
    
    $scope.onMoveDown = function()
    {
        $scope.offset += $scope.limit;
        
        $scope.setRange();
    }
    
    $scope.onMoveUp = function()
    {
        $scope.offset -= $scope.limit;
        if ($scope.offset < 0) $scope.offset = 0;
        
        $scope.setRange();
    }
    
    // Initialise $scope.items to be the top 10 items:
    $scope.offset = 0;
    $scope.limit = 10;
    $scope.setRange();
});