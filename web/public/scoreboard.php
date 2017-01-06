<!DOCTYPE html>
<html>
<head>
<link rel="stylesheet" type="text/css" href="css/common.css"/>
<link rel="stylesheet" type="text/css" href="css/entity.css"/>
<style>

.scoreboard {
	box-shadow: 8px 8px 4px #888888;
	border: 2px solid black;
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
	
	<?php include(realpath(dirname(__FILE__) . "./header.html")); ?>
	
	<article>
		
		<!-- Scoreboard section -->
		
		<section class="scoreboard" >
			<h3 class="scoreTitle">Rankings</h3>
			<table class="entityTable">
				<tr>
					<th>Rank</th>
					<th>Category</th>
					<th>Name</th> 
					<th>Score</th>
				</tr>
				
				<?php 
					require_once(realpath(dirname(__FILE__) . "/../library/include.php"));
					
					$DEFAULT_LIMIT = 10;
					
					$category = empty($_GET["category"]) ? null : getNonNegativeInt($_GET["category"], 0);
					echo $category;
					$offset = getNonNegativeInt($_GET["offset"], 0);
					$limit = getNonNegativeInt($_GET["limit"], $DEFAULT_LIMIT);
					$items = (new PocoDbClient())->getTopScoredEntities($category, $offset, $limit);
					
					foreach ($items as $item):
				?>
				
				<tr>
					<td><?php echo htmlspecialchars($item["rank"]); ?></td>
					<td><?php echo htmlspecialchars($item["category"]); ?></td>
					<td><?php echo htmlspecialchars($item["name"]); ?></td>
					<td><?php echo htmlspecialchars($item["score"]); ?></td>
				</tr>
				
				<?php endforeach; ?>
				
			</table>
			<div class="scoreButtons">
				<?php
					$url = "/scoreboard.php?";
					$prevUrl = $url . "offset=" . (($offset - $limit < 0) ? 0 : ($offset - $limit)) . "&limit=" . $limit;
					$nextUrl = $url . "offset=" . ($offset + $limit) . "&limit=" . $limit;
				?>
				
				<span><a href="<?php echo htmlspecialchars($prevUrl); ?>">&lt;</a></span>
				<span><a href="<?php echo htmlspecialchars($nextUrl); ?>">&gt;</a></span>
				<!--
				<span><form method="POST" action="<?php echo htmlspecialchars($prevUrl); ?>"><input type="submit" value="&lt;" /></form></span>
				<span><form method="POST" action="<?php echo htmlspecialchars($nextUrl); ?>"><input type="submit" value="&gt;" /></form></span>
				-->
			</div>
		</section>
		
	</article>
	
	<?php include(realpath(dirname(__FILE__) . "./footer.html")); ?>
	
</body>
</html>