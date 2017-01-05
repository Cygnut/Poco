<!DOCTYPE html>
<html>
<head>
<script type="text/javascript" src="scripts/include.js"/></script>
<link rel="stylesheet" type="text/css" href="css/common.css"/>
<link rel="stylesheet" type="text/css" href="css/entity.css"/>
<style>
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
	
	<?php include(realpath(dirname(__FILE__) . "./header.html")); ?>
	
	<article>
		
		<!-- Voting section -->
		<section class="entity-container">
			
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
			
		</section>
		
		<!--
			To be added
		<section id="links">Links</section>
		<section id="attributions">Attributions</section>
		-->
		
	</article>
	
	<?php include(realpath(dirname(__FILE__) . "./footer.html")); ?>
	
</body>
</html>