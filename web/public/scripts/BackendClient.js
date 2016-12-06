
function BackendClient($http)
{
    this.$http = $http;
}

BackendClient.prototype = 
{
    // public:
    
    // params = { offset, limit }
    getTopScoredEntities: function(params, callback, errorCallback)
    {
        return this.getData("getTopScoredEntities.php", params, callback, errorCallback);
    },
    
    // params = {}
    getVotableEntities: function(params, callback, errorCallback)
    {
        return this.getData("getVotableEntities.php", params, callback, errorCallback);
    },
    
    // params = { fragment, search_mode, offset, limit }
    searchEntities: function(params, callback, errorCallback)
    {
        return this.getData("searchEntities.php", params, callback, errorCallback);
    },
    
    // params = { id, direction }
    voteEntity: function(params, callback, errorCallback)
    {
        return this.postData("voteEntity.php", params, callback, errorCallback);
    },
    
    // private:
    postData: function(path, data, callback, errorCallback = this.defaultErrorCallback)
    {
        return this.$http({
            method: "POST",
            url: path,
            data: data
        }).then(
        function(response) { callback(response); },
        function(response) { errorCallback(response); });
    },
    
    getData: function(path, params, callback, errorCallback = this.defaultErrorCallback)
    {
        return this.$http({
            method: "GET",
            url: path,
            params: params
        }).then(
        function(response) { callback(response); },
        function(response) { errorCallback(response); });
    },
    
    defaultErrorCallback: function(reason)
    {
        console.log("Call to failed with reason " + reason);
    }
}