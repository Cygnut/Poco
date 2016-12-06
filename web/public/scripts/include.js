function include(file)
{
    document.write("<script type='text/javascript' src='" + file + "'></script>");
}

//include('http://ajax.googleapis.com/ajax/libs/angularjs/1.4.8/angular.min.js');
include('https://ajax.googleapis.com/ajax/libs/angularjs/1.5.7/angular.min.js');


include('scripts/BackendClient.js');

include('scripts/app.js');
include('scripts/controllers/vote.js');
include('scripts/controllers/scoreboard.js');
include('scripts/controllers/search.js');

