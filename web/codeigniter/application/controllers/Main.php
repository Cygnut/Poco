<?php
class Main extends CI_Controller {
    
    public function __construct()
    {
        parent::__construct();
        $this->load->model('poco_model');
        $this->load->helper('url_helper');
    }
    
    public function index()
    {
        $headerData['title'] = 'Poco - A popularity content on a heterogeneous database of media.';
        
        $data['votableEntities'] = $this->poco_model->getVotableEntities();
        $data['categories'] = $this->poco_model->getCategories();
        
        $this->load->view('templates/header', $headerData);
        $this->load->view('index', $data);
        $this->load->view('templates/footer');
    }
    
    public function vote($id, $direction)
    {
        $this->poco_model->voteEntity($id, $direction);
    }
}
?>