<?php
	
	function getNonNegativeInt($arg, $def)
	{
		$arg = $arg ?? $def;
		
		$arg = (int) $arg;
		
		if ($arg < 0)
			return $def;
		
		return $arg;
	}
	
	try
	{
		require_once(realpath(dirname(__FILE__) . "/../library/include.php"));
		
		// Parameter validation:
		$offset = getNonNegativeInt($_GET["offset"], 0);
		$limit = getNonNegativeInt($_GET["limit"], 10);
		
		// Invoke query:
		$conn = new MySqlClient($config["db"]);
		$rows = $conn->executeQuery(
			"CALL c_getTopScoredEntities(@_category := null, @_offset := $offset, @_limit := $limit);"
			);
		
		// Return output:
		$items = array();
		
		foreach ($rows as $row)
		{
			$items[] = array(
				"rank" => $row["ranking"],
				"id" => $row["id"],
				"name" => $row["name"],
				"category" => $row["category"],
				"sub_category" => $row["sub_category"],
				"blurb" => $row["blurb"],
				"image_url" => $row["image_url"],
				"score" => $row["score"]
			);
			++$rank;
		}
		
		echo json_encode(
			array("items" => $items)
		);
	}
	catch (Exception $e)
	{
		error_log($e->getMessage());
		echo createErrorResponse();
	}
?>