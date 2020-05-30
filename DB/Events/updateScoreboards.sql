SET GLOBAL event_scheduler = ON;

DROP EVENT IF EXISTS updateScoreboards;
CREATE DEFINER=`root`@`localhost` EVENT IF NOT EXISTS updateScoreboards
    ON SCHEDULE EVERY 15 MINUTE
    COMMENT 'Updates the scoreboard tables.'
    DO 
        CALL d_updateScoreboards();
