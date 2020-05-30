<?php
class Scoreboard extends CI_Controller {
    
    public function __construct()
    {
        parent::__construct();
        $this->load->model('poco_model');
        $this->load->helper('url_helper');
        $this->load->library('pagination');
    }
    
    public function index($category)
    {
        $paging['base_url'] = base_url() . 'scoreboard' . ($category ? "/$category" : "");
        $paging['total_rows'] = 100;    // For now, just show top 100.
        $paging['per_page'] = 10;
        $paging['uri_segment'] = 3;
        
        $this->pagination->initialize($paging);
        $page = ($this->uri->segment(3)) ? $this->uri->segment(3) : 0;
        
        $entities = $this->poco_model->getTopScoredEntities(
            $this->poco_model->getCategoryId($category), 
            $page + 1, 
            $paging['per_page']);
        
        $data['title'] = 'Poco | Scoreboard';
        $data['filter'] = $category;
        $data['entities'] = $entities;
        $data['links'] = $this->pagination->create_links();
        
        $this->load->view('templates/header', $data);
        $this->load->view('scoreboard', $data);
        $this->load->view('templates/footer');
    }
}
?>