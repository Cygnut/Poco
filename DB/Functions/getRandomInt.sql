DELIMITER $$
DROP FUNCTION IF EXISTS `getRandomInt`$$
CREATE DEFINER=`root`@`localhost` FUNCTION `getRandomInt`(
	_offset int,
    _count int
	) RETURNS int(11)
begin

	# Gives a random element in the set 
    # { _offset, _offset + 1, ... , _offset + _count - 1 }

	return
		greatest(
			least(
				# Note: rand() gives a random number in [0,1]
				_offset + floor(rand() * _count),
				(_offset + _count - 1)
			),
			_offset);
end$$
DELIMITER ;
