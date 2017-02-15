<?php
class Scoreboard extends CI_Controller {
	
	public function __construct()
	{
		parent::__construct();
		$this->load->model('poco_model');
		$this->load->helper('url_helper');
	}
	
	public function index($category = null)
	{
		$headerData['title'] = 'Poco | Scoreboard';
		
		/*
			TODO: Title page with category!!
			TODO: Fix pagination here!!
			
			$DEFAULT_LIMIT = 10;
			
			$category = empty($_GET["category"]) ? null : getNonNegativeInt($_GET["category"], 0);
			$offset = getNonNegativeInt($_GET["offset"], 0);
			$limit = getNonNegativeInt($_GET["limit"], $DEFAULT_LIMIT);
			$items = (new PocoDbClient())->getTopScoredEntities($category, $offset, $limit);
		
		*/
		
		$data['entities'] = $this->poco_model->getTopScoredEntities($category, 0, 10);
		
		$this->load->view('templates/header', $headerData);
		$this->load->view('scoreboard', $data);
		$this->load->view('templates/footer');
	}
}
?>