DELIMITER $$
DROP PROCEDURE IF EXISTS `_getConfiguration`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `_getConfiguration`(
	in _namespace varchar(255)
)
BEGIN
	select *
    from configuration
    where namespace = _namespace;
END$$
DELIMITER ;

