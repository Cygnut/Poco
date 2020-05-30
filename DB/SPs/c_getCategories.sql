DELIMITER $$
DROP PROCEDURE IF EXISTS `c_getCategories`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `c_getCategories`()
BEGIN
    
    select * from entity_category;
    
END$$
DELIMITER ;
