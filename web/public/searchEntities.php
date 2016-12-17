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
		$fragment = $_GET["fragment"];
		$search_mode = getNonNegativeInt($_GET["search_mode"], 0);
		$offset = getNonNegativeInt($_GET["offset"], 0);
		$limit = getNonNegativeInt($_GET["limit"], 10);
		
		// Invoke query:
		$conn = new MySqlClient($config["db"]);
		// TODO: Limit characters in $fragmentEsc based on the SP parameter length.
		$fragmentEsc = $conn->escapeString($fragment);
		$fragmentEsc = substr($fragmentEsc, 0, 25);	// For now, just use the first 25.
		
		$rows = $conn->executeQuery(
			"CALL c_searchEntities(
				@_fragment := \"$fragmentEsc\", 
				@_search_mode := $search_mode, 
				@_offset := $offset, 
				@_limit := $limit);"
			);
		
		// Return output:
		$items = array();
		
		foreach ($rows as $row)
		{
			$items[] = array(
				"id" => $row["id"],
				"name" => $row["name"],
				"category" => $row["category"],
				"sub_category" => $row["sub_category"],
				"blurb" => $row["blurb"],
				"image_url" => $row["image_url"],
				"score" => $row["score"]
			);
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