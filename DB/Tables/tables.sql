
DROP TABLE IF EXISTS `configuration`;
CREATE TABLE `configuration` (
  `namespace` varchar(255) NOT NULL,
  `name` varchar(255) NOT NULL,
  `value` longtext,
  PRIMARY KEY (`namespace`,`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `entity_category`;
CREATE TABLE `entity_category` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(45) NOT NULL,
  `when_created` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=25 DEFAULT CHARSET=utf8;

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

DROP TABLE IF EXISTS `entity_score`;
CREATE TABLE `entity_score` (
  `entity_id` int(11) NOT NULL,
  `score` bigint(20) NOT NULL DEFAULT '0',
  `total_votes` bigint(20) NOT NULL DEFAULT '0',
  PRIMARY KEY (`entity_id`),
  CONSTRAINT `entity_entity_id` FOREIGN KEY (`entity_id`) REFERENCES `entity` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `entity_scoreboard` (
  `scoreboard_category_id` int(11) DEFAULT NULL,
  `entity_id` int(11) NOT NULL,
  `score` int(11) NOT NULL,
  `total_votes` int(11) NOT NULL,
  KEY `entity_scoreboard_entity_idx` (`entity_id`),
  KEY `entity_scoreboard_entity_category_idx` (`scoreboard_category_id`),
  KEY `entity_scoreboard_idx` (`scoreboard_category_id`),
  CONSTRAINT `entity_scoreboard_entity` FOREIGN KEY (`entity_id`) REFERENCES `entity` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `entity_scoreboard_entity_category` FOREIGN KEY (`scoreboard_category_id`) REFERENCES `entity_category` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `entity_scoreboard_info` (
  `when_updated` datetime NOT NULL,
  `size` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
