DELIMITER $$
DROP PROCEDURE IF EXISTS `i_updateEntityCategory`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `i_updateEntityCategory`(in _category varchar(45))
BEGIN
    
    #
    # Ensure that an entity_category is present, and return its id.
    #
    
    declare _id int;
    
    select id into _id from entity_category where name = _category limit 1;
        
    if (_id is null) then
        insert into entity_category (name) values (_category);
        # use the equivalent of scope_identity() here.
        set _id = last_insert_id();    
    end if;
    
    select _id as id;
END$$
DELIMITER ;