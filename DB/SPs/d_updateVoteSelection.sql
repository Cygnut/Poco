DELIMITER $$
DROP PROCEDURE IF EXISTS `d_updateVoteSelection`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `d_updateVoteSelection`()
BEGIN
    
    declare _entity_id int;
    declare _category_id int;
    declare _score int;
    
    declare _second_category_offset int;
    declare _second_category_id int;
    declare _second_id int;
    
    declare done int;
    declare entity_cursor cursor for 
        select id, category_id, es.score 
        from entity
        inner join entity_score es on es.entity_id = id;
    
    declare continue handler for not found set done = true;
    
    # Update the scoreboard tables atomically.
    start transaction;
        
        # Reset tables.
        delete from entity_vote_selection;
        delete from entity_vote_selection_info;
        
        open entity_cursor;
        
        # Ensure that the done flag is in its correct state, and not set by previous queries.
        set done = false;
        
        read_loop: loop
            fetch entity_cursor into _entity_id, _category_id, _score;
            
            if done then leave read_loop;
            end if;
            
            # Find a random category that is not the current entity's one.
            # Find the one in that category with closest score, then add it to entity_vote_selection.
            
            # Get a random other category to use.
            select getRandomInt(0, count(1) - 1) from entity_category into _second_category_offset;
            
            select id
            from entity_category
            where id <> _category_id
            limit _second_category_offset, 1 
            into _second_category_id;
            
            # Find the entity in this second category with closest possible score.
            select id
            into _second_id
            from entity_score es    
            inner join entity e on e.id = es.entity_id
            where e.category_id = _second_category_id
            order by abs(_score - es.score)
            limit 1;
            
            # Insert the pair of entities into entity_vote_selection
            insert into entity_vote_selection (
                first_entity_id,
                second_entity_id
            )
            values (
                _entity_id,
                _second_id
            );
            
        end loop;
        
        close entity_cursor;
        
        # Update metadata table.
        insert into entity_vote_selection_info (
            when_updated
        ) 
        select 
            now();
        
    commit;
    
END$$
DELIMITER ;

