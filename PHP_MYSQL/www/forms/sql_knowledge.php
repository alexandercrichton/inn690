<?php

$db_knowledge_name = "veis_knowledge_base";

// Database connection variables
$db = array(	"username" 	=> "root",
				"password" 	=> "",
				"server" 	=> "127.0.0.1",
				"database" 	=> $db_knowledge_name);

// Create new connection
$mysqli = new mysqli($db["server"], $db["username"], $db["password"], $db["database"]);

// Unset the database connection variable for security measures
unset($db);

?>