function include(file)
{
    document.write("<script type='text/javascript' src='" + file + "'></script>");
}

include('vendor/angular.min-1.5.7.js');

include('scripts/BackendClient.js');

include('scripts/app.js');
include('scripts/controllers/vote.js');
include('scripts/controllers/scoreboard.js');
include('scripts/controllers/search.js');

