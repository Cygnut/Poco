DELIMITER $$
DROP PROCEDURE IF EXISTS `i_getPostImportStats`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `i_getPostImportStats`()
BEGIN
    # Get the spread of entities over categories.
    select 
        ec.id as category_id,
        ec.name as category,
        count(1) as count
    from entity e
    inner join entity_category ec on e.category_id = ec.id
    group by e.category_id;
END$$
DELIMITER ;
