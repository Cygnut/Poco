DROP TABLE IF EXISTS `configuration`;
CREATE TABLE `configuration` (
  `namespace` varchar(255) NOT NULL,
  `name` varchar(255) NOT NULL,
  `value` longtext,
  PRIMARY KEY (`namespace`,`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
