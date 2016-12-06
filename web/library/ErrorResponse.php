<?php
	function createErrorResponse()
	{
		return json_encode(
			array("error" => 
				array(
					"uri" => $_SERVER['REQUEST_URI'],
					"qs" => $_SERVER['QUERY_STRING']
				)
			)
		);
	}
?>