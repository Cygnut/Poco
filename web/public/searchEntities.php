<?php
	require_once(realpath(dirname(__FILE__) . "/../library/include.php"));
	
	function getNonNegativeInt($arg, $def)
	{
		$arg = $arg ?? $def;
		
		$arg = (int) $arg;
		
		if ($arg < 0)
			return $def;
		
		return $arg;
	}
	
	function searchEntities($fragment, $offset, $limit)
	{
		try
		{
			$search_mode = 0;
			
			// Invoke query:
			global $config;
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
					"score" => $row["score"],
					"category_img" => "img/content/entity_category_" . $row["category"] . ".png"
				);
			}
			
			return $items;
		}
		catch (Exception $e)
		{
			error_log($e->getMessage());
			return array();
		}
	}
?>