DELIMITER $$
DROP PROCEDURE IF EXISTS `v_getEntities`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `v_getEntities`()
BEGIN
	select 
		e.*, 
        ec.name as category,
		es.score as score,
		es.total_votes as total_votes
    from entity e
    inner join entity_category ec on e.category_id = ec.id
	inner join entity_score es on e.id = es.entity_id;
END$$
DELIMITER ;
