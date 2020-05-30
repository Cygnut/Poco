DELIMITER $$
DROP PROCEDURE IF EXISTS `c_getVotableEntities`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `c_getVotableEntities`()
BEGIN
    
    declare _offset int;
    declare _first_id int;
    declare _second_id int;
    
    # Pick a vote selection at random.
    # We ensure that the row index is in range using greatest and least.
    
    select getRandomInt(0, count(1)) from entity_vote_selection into _offset;
    
    select 
        e.first_entity_id,
        e.second_entity_id
    into 
        _first_id, 
        _second_id
    from entity_vote_selection e
    limit _offset, 1;
    
    # Return those entities.
    
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
