<!DOCTYPE html>
<html>
<head>
<link rel="stylesheet" type="text/css" href="css/common.css"/>
<link rel="stylesheet" type="text/css" href="css/entity.css"/>
<style>

a,p,h1,h2,h3,h4,h5,h6 {
	text-align: center;
}

.entity-table {
	display: flex;
	justify-content: space-around;
	flex-direction: row;
	flex-wrap: wrap;
}

h3.scoreTitle {
	text-align: center;
}

/* The following two are to spread out the buttons evenly horizontally. */
div.scoreButtons {
	display: table;
	width: 100%;
	table-layout: fixed;    /* For cells of equal size */
}

div.scoreButtons span {
	display: table-cell;
	text-align: center;
}

</style>
	
	<title>Poco | Scoreboard</title>
	
	<meta charset="UTF-8">
	<meta name="description" content="Poco | Scoreboard">
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
		<h2>Scoreboard</h2>
	</section>
	
	<section class="scoreboard" >
		<h3 class="scoreTitle">Rankings</h3>
		
		<div class="entity-table">
			<?php 
				require_once(realpath(dirname(__FILE__) . "/../library/include.php"));
				$twig = getTwigEnvironment();
				
				$DEFAULT_LIMIT = 10;
				
				$category = empty($_GET["category"]) ? null : getNonNegativeInt($_GET["category"], 0);
				$offset = getNonNegativeInt($_GET["offset"], 0);
				$limit = getNonNegativeInt($_GET["limit"], $DEFAULT_LIMIT);
				$items = (new PocoDbClient())->getTopScoredEntities($category, $offset, $limit);
				
				foreach ($items as $item)
					echo $twig->render('entity.twig', array('entity' => $item));
			?>
		</div>
		
		<div class="scoreButtons">
			<?php
				$url = "/scoreboard.php?";
				$prevUrl = $url . "offset=" . (($offset - $limit < 0) ? 0 : ($offset - $limit)) . "&limit=" . $limit;
				$nextUrl = $url . "offset=" . ($offset + $limit) . "&limit=" . $limit;
			?>
			
			<span><a href="<?php echo htmlspecialchars($prevUrl); ?>">&lt;</a></span>
			<span><a href="<?php echo htmlspecialchars($nextUrl); ?>">&gt;</a></span>
		</div>
	</section>
	
	<section>
		<p>Cygnut @ 2016</p>
	</section>
	
	</article>
	
</body>
</html>