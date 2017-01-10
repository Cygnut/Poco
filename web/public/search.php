<!DOCTYPE html>
<html>
<head>
<link rel="stylesheet" type="text/css" href="css/common.css"/>
<link rel="stylesheet" type="text/css" href="css/entity.css"/>
<style>

.search {
	box-shadow: 8px 8px 4px #888888;
	border: 2px solid black;
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
	
	<?php include(realpath(dirname(__FILE__) . "./header.html")); ?>
	
	<article>
		
		<section class="search">
			<div>
				<h3 class="searchTitle">Search</h3>
				<form class="searchForm" action="search.php" method="GET">
					<input type="text" name="fragment" value="<?php echo htmlspecialchars($_GET["fragment"]); ?>"></input>
					<input type="submit" value="Search"></input>
				</form>
				
				<table class="entityTable">
					<tr>
						<th>Category</th>
						<th>Name</th> 
						<th>Score</th>
					</tr>
					
					<?php 
						require_once(realpath(dirname(__FILE__) . "/../library/include.php"));
						
						$DEFAULT_LIMIT = 10;
						
						$fragment = $_GET["fragment"];
						$offset = getNonNegativeInt($_GET["offset"], 0);
						$limit = getNonNegativeInt($_GET["limit"], $DEFAULT_LIMIT);
						
						$items = (new PocoDbClient())->search($fragment, 0, $offset, $limit);
						
						foreach ($items as $item):
					?>
					
					<tr>
						<td><?php echo htmlspecialchars($item["category"]); ?></td>
						<td><?php echo htmlspecialchars($item["name"]); ?></td>
						<td><?php echo htmlspecialchars($item["score"]); ?></td>
					</tr>
					
					<?php endforeach; ?>
				</table>
				
			</div>
		</section>
		
	</article>
	
	<?php include(realpath(dirname(__FILE__) . "./footer.html")); ?>
	
</body>
</html>