<!DOCTYPE html>
<html>
<head>
<link rel="stylesheet" type="text/css" href="css/common.css"/>
<link rel="stylesheet" type="text/css" href="css/entity.css"/>
<style>

section {
	margin: auto;
}

a,h1,h2,h3 {
	text-align: center;
}

hr {
	width: 80%;
}

.entity-section {
	background-color: teal;
	width: 900px;
	height: 900px;
	
	border-radius: 450px;
	
	/* Centre the internal div. */
	display: flex;
	justify-content: center;
	align-items: center;
}

.entity-container {
	/* Space out content evenly. */
	width: 100%;
	display: flex;
	justify-content: space-around;
}

.links-container {
	text-align: center;
}

.links-container ul {
	/* No bullet points. */
	list-style-type: none;
	/* No margin where the bullet points were. */
	margin: 0;
	padding: 0;
}

.links-container ul li {
	display: inline;
}

form {
	text-align: center;
}

</style>
	
	<title>Poco - A popularity content on a heterogeneous database of media.</title>
	
	<meta charset="UTF-8">
	<meta name="description" content="Popularity contest on a heterogeneous database of media entities.">
	<meta name="keywords" content="Film, TV, Music, Food">
	<meta name="author" content="Cygnut">
	
	<meta name="viewport" content="width=device-width, initial-scale=1.0">
	
	<link rel="shortcut icon" href="img/content/PocoIcon-64x64-Transparent.png" />
	
</head>
<body>
	
	<?php //include(realpath(dirname(__FILE__) . "./header.html")); ?>
	<article>
	
	<section>
		<h1>Poco</h1>
		<hr/>
		<h2>It's a popularity contest!</h2>
	</section>
	
	<hr/>
	
	<!-- Voting section -->
	<section class="entity-section">
		
		<div class="entity-container">
		<?php
			require_once(realpath(dirname(__FILE__) . "/../library/include.php"));
			$items = (new PocoDbClient())->getVotableEntities();
			
			foreach ($items as $item): 
		?>
		
			<div style="background-image: url(<?php echo htmlspecialchars($item["category_background_img"]); ?>);" class="entity">
				<!-- Card layout: -->
				<div>
					<!-- <p>{{ e.id }}</p> -->
					<div class="entity-title">
						<strong><?php echo htmlspecialchars($item["name"]); ?></strong>
					</div>
					<div class="entity-img-container">
						<img 
							src="<?php echo htmlspecialchars(empty($item["image_url"]) ? "img/content/alt_entity_image.png" : $item["image_url"]); ?>" 
							alt="<?php echo htmlspecialchars($item["name"]); ?>" 
							height="128" width="128"/>
					</div>
					<div class="entity-blurb">
						<?php echo htmlspecialchars($item["blurb"]); ?>
					</div>
					<div class="entity-footer">
						<img 
							src="<?php echo htmlspecialchars(empty($item["category_img"]) ? "img/content/alt_entity_image.png" : $item["category_img"]); ?>" 
							alt="<?php echo htmlspecialchars($item["category"]); ?>" 
							height="24" width="24"/>
						<span class="entity-score-container">
							<strong class="entity-score"><?php echo htmlspecialchars($item["score"]); ?></strong>
						</span>
					</div>
				</div>
			</div>
		
		<?php endforeach; ?>
		
		</div>
		
	</section>
		
	<hr/>
		
	<section>
		<h3>Scoreboard</h3>
		
		<div class="links-container">
			
			<ul>
			<li><a href="/scoreboard.php">All</a></li>
			<?php
				$categories = (new PocoDbClient())->getCategories();
				foreach ($categories as $category):
			?>
			<li>
			<a href="/scoreboard.php?category=<?php echo $category["id"]; ?>"><?php echo $category["name"]; ?></a>
			</li>
			<?php endforeach; ?>
			</ul>
		
		<div>
		
	</section>
	
	<hr>
	
	<section>
		<h3>Search</h3>
		
		<form class="searchForm" action="search.php" method="GET">
			<input type="text" name="fragment" value="<?php echo htmlspecialchars($_GET["fragment"]); ?>"></input>
			<input type="submit" value="Search"></input>
		</form>
		
		<br/>
	</section>
	
	<hr/>
	
		<!--
			To be added
		<section id="attributions">Attributions</section>
		-->
		
	</article>
	
</body>
</html>