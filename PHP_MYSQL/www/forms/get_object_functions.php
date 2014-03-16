<?php
// The purposes of this script is to echo back the given objects functions and extra
// metadata. No form needs to be shown at this stage. 
// Author: Kathleen Nardella
// Data: 13/07/2012

// Includes
require_once("data_functions.php");
require_once("sql_knowledge.php");
require_once("sql_functions.php");

// Get object name 
$urlData = getURLData(array("case"));
if($urlData["case"] == null) {
	beginPage("Error", "style.css");
	message("Error", "Case needs to be stated in the url.");
	endPage();
	exit(0);
}

?>