<?php
	function getNonNegativeInt($arg, $def)
	{
		$arg = $arg ?? $def;
		
		$arg = (int) $arg;
		
		if ($arg < 0)
			return $def;
		
		return $arg;
	}
?>