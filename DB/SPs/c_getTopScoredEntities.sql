DELIMITER $$
DROP PROCEDURE IF EXISTS `c_getTopScoredEntities`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `c_getTopScoredEntities`(
	in _offset int, 
    in _limit int
    )
BEGIN
	select 
		e.*, 
        ec.name as category,
		es.score as score,
		es.total_votes as total_votes
    from entity e
    inner join entity_category ec on e.category_id = ec.id
	inner join entity_score es on e.id = es.entity_id
    order by es.score desc, e.name desc
    limit _offset, _limit;
END$$
DELIMITER ;
