DELIMITER $$
DROP PROCEDURE IF EXISTS `c_getVotableEntities`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `c_getVotableEntities`()
BEGIN

	#
    # Select two rows by random to compare, with the requirement that:
    #   - They both be of different category in such a way that we pick
    #     that category in an evenly random fashion.
    #   - They both have as close a score as possible.
    #
    
	
	
    declare _first_offset int;
    declare _first_id int;
    declare _first_category_id int;
    declare _first_score int;
    
    declare _second_category_offset int;
    declare _second_category_id int;
    
    declare _second_id int;

	# 1. Pick a row at random    
    # We ensure that the row index is in range using greatest and least.
	select getRandomInt(0, count(1)) from entity into _first_offset;

	select 
		e.id, 
        e.category_id, 
        es.score
    into 
		_first_id, 
        _first_category_id,
        _first_score
    from entity e
	inner join entity_score es on e.id = es.entity_id
    limit _first_offset, 1;

	# 2. Select a random different category to select from.    
	select getRandomInt(0, count(1) - 1) from entity_category into _second_category_offset;
    
    select id 
    from entity_category
    where id <> _first_category_id
    limit _second_category_offset, 1 
    into _second_category_id;

	# 3. Now find another row for the user to vote against.
    # It should have:
    #   The different randomly chosen category AND
    #   The next closest 'score'.    
    select id
    into _second_id
    from entity_score es    
	inner join entity e on e.id = es.entity_id
    where e.category_id = _second_category_id
    order by abs(_first_score - es.score)
    limit 1;
    
    # 4. Return both rows.
	select
		e.*, 
        ec.name as category,
		es.score as score,
		es.total_votes as total_votes
    from entity e
	inner join entity_score es on e.id = es.entity_id
    inner join entity_category ec on e.category_id = ec.id
    where e.id in (_first_id, _second_id);
        
END$$
DELIMITER ;
