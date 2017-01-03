<?php
	try
	{
		require_once(realpath(dirname(__FILE__) . "/../library/include.php"));
		
		// Invoke query:
		$conn = new MySqlClient($config["db"]);
		$rows = $conn->executeQuery(
			"CALL c_getVotableEntities();"
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
				"category_img" => "img/content/entity_category_" . $row["category"] . ".png",
				"category_background_img" => "img/content/entity_category_background_" . $row["category"] . ".png"
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