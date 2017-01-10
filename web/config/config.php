<?php
 
$config = array(
	"db" => array(
		"dbname" => "poco",
		"username" => "root",
		"password" => "admin",
		"host" => "localhost"
	),
	"paths" => array(
		"images" => array(
			"content" => $_SERVER["DOCUMENT_ROOT"] . "/images/content",
			"layout" => $_SERVER["DOCUMENT_ROOT"] . "/images/layout"
		),
		"library" => realpath(dirname(__FILE__) . '/library'),
		"templates" => realpath(dirname(__FILE__) . '/../public/templates'),
		"compiled_templates" => realpath(dirname(__FILE__) . '/../public/templates/compiled')
	)
);
 
/* Error reporting. */
ini_set("error_reporting", "true");
error_reporting(E_ALL|E_STRCT);

?>