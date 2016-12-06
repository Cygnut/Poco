DELIMITER $$
DROP PROCEDURE IF EXISTS `c_voteEntity`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `c_voteEntity`(
	in _id int,
    in _direction int,	# 1 for up, -1 for down.
    in _by int	# Defaults to 1 - should be a positive int >= 1.
    )
BEGIN
	
	# Upvote or downvote an entity and return the new score.
    
    # Sigh - we have to use a session variable here instead of a local
    # in order to set it in the update - you can't do that with locals syntactically for no reason.
    # However, we are setting it each time so we should be fine.
	set @new_score = 0;
    
    update entity_score
    set		
        score = @new_score := score + if(_direction > 0,1,-1) * _by,
        total_votes = total_votes + _by
    where entity_id = _id;
    
    select @new_score as score;
END$$
DELIMITER ;
