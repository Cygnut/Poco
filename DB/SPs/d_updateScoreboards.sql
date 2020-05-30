DELIMITER $$
DROP PROCEDURE IF EXISTS `d_updateScoreboards`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `d_updateScoreboards`()
BEGIN
    
    declare category_id int;
    
    declare done int;
    declare category_cursor cursor for 
        select null
        union
        (select id from entity_category);
    declare continue handler for not found set done = true;

    # Update the scoreboard tables atomically.
    start transaction;
        
        # Reset tables.
        delete from entity_scoreboard;
        delete from entity_scoreboard_info;
        
        # Populate the per-category scoreboards (the global one has category_id = null).
        open category_cursor;
        
        # Ensure that the done flag is in its correct state, and not set by previous queries.
        set done = false;
        
        read_loop: loop
            fetch category_cursor into category_id;

            if done then leave read_loop;
            end if;
            
            set @row = 0;
            insert into entity_scoreboard (
                scoreboard_category_id,
                ranking,
                entity_id,
                score,
                total_votes
            ) 
            select
                category_id,
                (@row:=@row+1),
                entity_id,
                score,
                total_votes
            from 
            (
                select 
                    category_id, 
                    e.id as entity_id,
                    s.score as score,
                    s.total_votes as total_votes
                from entity e
                inner join entity_score s on e.id = s.entity_id
                where (category_id is null) or (category_id is not null and category_id = e.category_id)
                order by s.score desc, e.name desc
            ) sub;
            
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

