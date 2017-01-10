<!DOCTYPE html>
<html>
<head>
<link rel="stylesheet" type="text/css" href="css/common.css"/>
<link rel="stylesheet" type="text/css" href="css/entity.css"/>
<style>

a,p,h1,h2,h3,h4,h5,h6 {
	text-align: center;
}

.search {
	/*
	box-shadow: 8px 8px 4px #888888;
	border: 2px solid black;
	*/
}

.entity-table {
	display: flex;
	justify-content: space-around;
	flex-direction: row;
	flex-wrap: wrap;
}

.searchTitle {
	text-align: center;
}

.searchForm {
	margin-left: 10px;
	margin-bottom: 20px;
}

</style>
	
	<title>Poco | Search</title>
	
	<meta charset="UTF-8">
	<meta name="description" content="Popularity | Search">
	<meta name="keywords" content="Film, TV, Music, Food">
	<meta name="author" content="Cygnut">
	
	<meta name="viewport" content="width=device-width, initial-scale=1.0">
	
	<link rel="shortcut icon" href="img/content/PocoIcon-64x64-Transparent.png" />
	
</head>
<body>
	
	<article>
		
	<section>
		<h1>Poco</h1>
		<hr/>
		<h2>Search</h2>
	</section>
		
	<section class="search">
		<div class="entity-table">
		<?php 
			require_once(realpath(dirname(__FILE__) . "/../library/include.php"));
			$twig = getTwigEnvironment();
			
			$DEFAULT_LIMIT = 10;
			
			$fragment = $_GET["fragment"];
			$offset = getNonNegativeInt($_GET["offset"], 0);
			$limit = getNonNegativeInt($_GET["limit"], $DEFAULT_LIMIT);
			
			$items = (new PocoDbClient())->search($fragment, 0, $offset, $limit);
			
			foreach ($items as $item)
				echo $twig->render('entity.twig', array('entity' => $item));
		?>
		</div>
	</section>
	
	<section>
		<p>Cygnut @ 2016</p>
	</section>
	
	</article>
	
</body>
</html>