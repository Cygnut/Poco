<?php
	function getNonNegativeInt($arg, $def)
	{
		$arg = $arg ?? $def;
		
		$arg = (int) $arg;
		
		if ($arg < 0)
			return $def;
		
		return $arg;
	}
	
	function getTwigEnvironment()
	{
		global $config;
		
		$loader = new Twig_Loader_Filesystem($config["paths"]["templates"]);
		return new Twig_Environment($loader, array(
			'cache' => $config["paths"]["compiled_templates"],
			'debug' => true
		));
	}
?>