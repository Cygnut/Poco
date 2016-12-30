SET GLOBAL event_scheduler = ON;

DROP EVENT IF EXISTS updateVoteSelections;
CREATE DEFINER=`root`@`localhost` EVENT IF NOT EXISTS updateVoteSelections
	ON SCHEDULE EVERY 15 MINUTE
	COMMENT 'Updates the vote selection tables.'
	DO 
        CALL d_updateVoteSelection();
