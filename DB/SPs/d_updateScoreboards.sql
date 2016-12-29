DELIMITER $$
DROP PROCEDURE IF EXISTS `d_updateScoreboards`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `d_updateScoreboards`()
BEGIN
	
    declare scoreboard_size integer default 10;

    declare category_id int;
    
    declare done int;
    declare category_cursor cursor for select id from entity_category;
    declare continue handler for not found set done = true;

    # Update the scoreboard tables atomically.
    start transaction;
        
        # Reset tables.
        delete from entity_scoreboard;
        delete from entity_scoreboard_info;
        
        # Get the scoreboard size:
        select cast(value as unsigned) from configuration
        where namespace = 'DB' and name = 'ScoreboardSize'
        into scoreboard_size;
        
        # Populate the overall scoreboard.
        insert into entity_scoreboard (
            scoreboard_category_id,
            entity_id,
            score
        ) 
        select 
            null, 
            e.id, 
            s.score
        from entity e
        inner join entity_score s
        on e.id = s.entity_id
        order by s.score desc
        limit 0, scoreboard_size;
        
        # Populate the per-category scoreboards.
        open category_cursor;
        
        # Ensure that the done flag is in its correct state, and not set by previous queries.
        set done = false;
        
        read_loop: loop
            fetch category_cursor into category_id;

            if done then leave read_loop;
            end if;
            
            insert into entity_scoreboard (
                scoreboard_category_id,
                entity_id,
                score
            ) 
            select 
                e.category_id, 
                e.id, 
                s.score
            from entity e
            inner join entity_score s
            on e.id = s.entity_id
            where e.category_id = category_id
            order by s.score desc
            limit 0, scoreboard_size;
            
        end loop;
        
        close category_cursor;
        
        # Update metadata table.
        insert into entity_scoreboard_info (
            when_updated
        ) 
        select 
            now();
        
    commit;
    
END$$
DELIMITER ;

