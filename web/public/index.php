<!DOCTYPE html>
<html>
<head>
<script src="vendor/jquery-3.1.1.min.js"></script>
<script src="vendor/jquery-ui.min-1.12.1.js"></script>
<link rel="stylesheet" type="text/css" href="css/common.css"/>
<link rel="stylesheet" type="text/css" href="css/entity.css"/>
<style>

section {
	margin: auto;
}

a,p,h1,h2,h3,h4,h5,h6 {
	text-align: center;
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

/* Initially hide entity cards. */
entity {
	display: none;
}

</style>
	
	<title>Poco - A popularity content on a heterogeneous database of media.</title>
	
	<meta charset="UTF-8">
	<meta name="description" content="Popularity contest on a heterogeneous database of media entities.">
	<meta name="keywords" content="Film, TV, Music, Food">
	<meta name="author" content="Cygnut">
	
	<meta name="viewport" content="width=device-width, initial-scale=1.0">
	
	<link rel="shortcut icon" href="img/content/PocoIcon-64x64-Transparent.png" />
	
<script>

$(function() {
	
	$(".entity")
		.hide()
		.show("drop", { direction: "up" }, 1000);
	
	$(".entity")
		.click(function() {
			id = $(this).attr('data-id');
			// TODO: Handle this!
		});
	
});

</script>
</head>
<body>
	
	<article>
	
		<section>
			<h1>Poco</h1>
			<hr/>
			<h2>It's a popularity contest!</h2>
		</section>
		
		<hr/>
		
		<br/>
		
		<!-- Voting section -->
		<section class="entity-section">
			
			<div class="entity-container">
			<?php
				require_once(realpath(dirname(__FILE__) . "/../library/include.php"));
				
				$twig = getTwigEnvironment();
				$items = (new PocoDbClient())->getVotableEntities();
				
				foreach ($items as $item)
					echo $twig->render('entity.twig', array('entity' => $item));
			?>
			</div>
			
		</section>
		
		<br/>
		
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
		
		<section>
			<p>Cygnut @ 2016</p>
		</section>
	
	</article>
	
</body>
</html>