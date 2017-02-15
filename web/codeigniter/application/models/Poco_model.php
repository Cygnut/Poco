<?php
class Poco_model extends CI_Model {
	
	/*
	Note: 
		To use stored procedures with mysqli in codeigniter, you must edit the system files:
		
		On line 262 of system/database/drivers/mysqli/mysqli_driver.php change
		protected function _execute($sql)
		{
			return $this->conn_id->query($this->_prep_query($sql));
		}
		to this
		protected function _execute($sql)
		{
			$results = $this->conn_id->query($this->_prep_query($sql));
			@mysqli_next_result($this->conn_id); // Fix 'command out of sync' error
			return $results;
		}
	*/
	
	public function __construct()
	{
		$this->load->database();
	}
	
	private static function toEntity($row)
	{
		return [
			'id' => $row['id'],
			'name' => $row['name'],
			'category' => $row['category'],
			'sub_category' => $row['sub_category'],
			'blurb' => $row['blurb'],
			'image_url' => $row['image_url'] ? $row['image_url'] : 'resources/img/content/alt_entity_image.png',
			'score' => $row['score'],
			'category_img' => 'resources/img/content/entity_category_' . $row['category'] . '.png',
			'category_background_img' => 'resources/img/content/entity_category_background_' . $row['category'] . '.png'
		];
	}
	
	public function getCategories()
	{
		$rows = $this->db
			->query('call c_getCategories()')
			->result_array();
		
		return $rows;
	}
	
	public function searchEntities($fragment, $searchMode, $offset, $limit)
	{
		$rows = $this->db
			->query('call c_searchEntities(?,?,?,?)', [
				'fragment' => $fragment,
				'search_mode' => $searchMode,
				'offset' => $offset,
				'limit' => $limit
			])
			->result_array();
		
		return array_map('self::toEntity', $rows);
	}
	
	public function voteEntity($id, $direction)
	{
		$this->db
			->query('call c_voteEntity(?,?,?)', [
				'id' => $id,
				'direction' => $direction,
				'by', 1
			]);
	}
	
	public function getTopScoredEntities($category, $offset, $limit)
	{
		$rows = $this->db
			->query('call c_getTopScoredEntities(?,?,?)', [
				'category' => $category,
				'offset' => $offset,
				'limit' => $limit
			])
			->result_array();
		
		return array_map('self::toEntity', $rows);
	}
	
	public function getVotableEntities()
	{
		$rows = $this->db
			->query('call c_getVotableEntities()')
			->result_array();
		
		return array_map('self::toEntity', $rows);
	}
}
?>