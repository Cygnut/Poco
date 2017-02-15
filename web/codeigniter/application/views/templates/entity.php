
<div 
	style="background-image: url(<?php echo base_url($entity['category_background_img']); ?>);" 
	class="entity" 
	data-id="<?php echo $entity['id']; ?>"
	>
	<div class="entity-title">
		<strong>
			<?php echo $entity['name']; ?>
		</strong>
	</div>
	<div class="entity-img-container">
		<img 
			src="<?php echo $entity['image_url']; ?>" 
			alt="<?php echo $entity['name']; ?>" 
			height="128" 
			width="128"
		/>
	</div>
	<div class="entity-blurb">
		<?php echo $entity['blurb']; ?>
	</div>
	<div class="entity-footer">
		<img 
			src="<?php echo base_url($entity['category_img']); ?>" 
			alt="<?php echo $entity['category']; ?>" 
			height="24" 
			width="24"
		/>
		<span class="entity-score-container">
			<strong>
				<?php echo $entity['score']; ?>
			</strong>
		</span>
	</div>
</div>
