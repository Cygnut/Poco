DROP TABLE IF EXISTS `entity_score`;
CREATE TABLE `entity_score` (
  `entity_id` int(11) NOT NULL,
  `score` bigint(20) NOT NULL DEFAULT '0',
  `total_votes` bigint(20) NOT NULL DEFAULT '0',
  PRIMARY KEY (`entity_id`),
  CONSTRAINT `entity_entity_id` FOREIGN KEY (`entity_id`) REFERENCES `entity` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
