DELIMITER $$
DROP PROCEDURE IF EXISTS `c_getTopScoredEntities`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `c_getTopScoredEntities`(
	in _category int,
	in _offset int, 
	in _limit int
	)
BEGIN
	
	select
		e.*,
		ec.name as category,
		esb.ranking as ranking,
        esb.score as score,
		esb.total_votes as total_votes
	from
		entity_scoreboard esb
		inner join entity e on e.id = esb.entity_id
		inner join entity_category ec on ec.id = e.category_id
	where
		(
            (_category is null and esb.scoreboard_category_id is null) 
            or
            (_category is not null and _category = esb.scoreboard_category_id)
        )
        and
        ranking between _offset and _offset+_limit-1
	order by ranking;
    
END$$
DELIMITER ;
