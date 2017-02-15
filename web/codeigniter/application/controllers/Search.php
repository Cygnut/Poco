<?php
class Search extends CI_Controller {
	
	public function __construct()
	{
		parent::__construct();
		$this->load->model('poco_model');
		$this->load->helper('url_helper');
	}
	
	public function index($fragment = '')
	{
		$headerData['title'] = 'Poco | Search';
		
		/*
			TODO: Fix this! Show fragment.
			TODO: Fix this! Pagination.
		
		$DEFAULT_LIMIT = 10;
		
		$fragment = $_GET["fragment"];
		$offset = getNonNegativeInt($_GET["offset"], 0);
		$limit = getNonNegativeInt($_GET["limit"], $DEFAULT_LIMIT);
		
		$items = (new PocoDbClient())->search($fragment, 0, $offset, $limit);
		
		*/
		
		$data['entities'] = $this->poco_model->searchEntities($fragment, 0, 0, 10);
		
		$this->load->view('templates/header', $headerData);
		$this->load->view('search', $data);
		$this->load->view('templates/footer');
	}
}
?>