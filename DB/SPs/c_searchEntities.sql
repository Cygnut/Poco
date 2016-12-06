DELIMITER $$
DROP PROCEDURE IF EXISTS `c_searchEntities`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `c_searchEntities`(
	in _fragment varchar(255),
    in _search_mode int,
    in _offset int,
    in _limit int
	)
BEGIN
	
	#
	# Search for an entity like-ing on certain specified fields.
	# Order by popularity and then name.
	#
	#
	
	# _search_mode:
    #	0 - entity.name or entity.blurb (also the fallback)
    #	1 - just entity.name
    #	2 - just entity.blurb
    #
    
	declare _fragment_like varchar(511);
    select concat('%', _fragment, '%') into _fragment_like;

	select 
		e.*, 
        ec.name as category,
		es.score as score,
		es.total_votes as total_votes
    from entity e
    inner join entity_category ec on e.category_id = ec.id
	inner join entity_score es on e.id = es.entity_id
    where 
		case _search_mode
			when 1 then e.name like _fragment_like
			when 2 then e.blurb like _fragment_like
			else e.name like _fragment_like or e.blurb like _fragment_like
		end
    order by score desc, name desc
    limit _offset, _limit;
END$$
DELIMITER ;
