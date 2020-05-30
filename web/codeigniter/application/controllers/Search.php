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
        $data['title'] = 'Poco | Search';
        $data['fragment'] = $fragment;
        $data['entities'] = $this->poco_model->searchEntities($fragment, 0, 0, 10);
        
        $this->load->view('templates/header', $data);
        $this->load->view('search', $data);
        $this->load->view('templates/footer');
    }
}
?>