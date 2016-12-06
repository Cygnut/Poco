DROP TABLE IF EXISTS `entity`;
CREATE TABLE `entity` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `category_id` int(11) NOT NULL,
  `sub_category` varchar(255) NOT NULL DEFAULT '',
  `blurb` varchar(511) NOT NULL DEFAULT '',
  `image_url` varchar(511) NOT NULL,
  `when_created` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `when_updated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `native_popularity` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `entity_entity_category_idx` (`category_id`),
  CONSTRAINT `entity_entity_category` FOREIGN KEY (`category_id`) REFERENCES `entity_category` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=11466 DEFAULT CHARSET=utf8;
