// Get the poco module and add a controller to it.
angular.module('poco').controller("vote", function($scope, $http) {
    
    var c = new BackendClient($http);
    
    // This method allows us to reseed the $scope.votableEntities to two random entities.
    $scope.newVotableEntities = function()
    {
        return c.getVotableEntities(null, function(data) {
            $scope.votableEntities = data.data.items;
        });
    }
    
    // Button handler for when an item is voted up.
    $scope.onVoteUp = function(id)
    {
        // Send the upvote to the server.
        c.voteEntity(
        {
            id: id,
            direction: 1
        },
        function(data) {});
        
        // Reseed the votable entities.
        $scope.newVotableEntities();
    }
    
    // Initialise $scope.votableEntities;
    $scope.newVotableEntities();
});