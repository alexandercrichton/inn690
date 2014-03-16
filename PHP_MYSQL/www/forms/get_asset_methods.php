<script language="php">
require_once "utility/page_functions.php";
require_once "utility/form_functions.php";
require_once "utility/sql_functions.php";
require_once "world_config.php";
include_once "sql_world_state.php";

// get the user key
$user_key = $_GET["user_key"];
// get the asset name
$asset_name = $_GET["asset_name"];
// get the asset key
$asset_key = $_GET["asset_key"];
// get the user name
$user_name = $_GET["user_name"];

// make sure everything is set
if (isset($asset_key) && isset($asset_name) && isset($user_key)){
	
	// select every event for this user 
	$result = getPlayerAssetEvent($user_key, $asset_key);
	if ($result == FALSE || sizeof($result) == 0 ){
		error_page("Database query result", "You aren't registered against this asset.\n".$sql_query, "style.css");
	}
	// change the database
	include_once "sql_knowledge.php";

	// get methods for the asset
	$sql_query = makeQuery("method_name", "asset_methods", array ( array("asset_name", "=", $asset_name)));
	
	// perform a query and get the database result
	$result = doQuery($sql_query);
	// 
	// test success
	if ($result != FALSE) {
		// the asset list
		$assets = array();
		$assets[0] = array();
		$assets[0][0] = "NULL";
		$assets[0][1] = "SELECT METHOD";
		// the index
		$index = 1;
		// create selection 
		for($i = 0; $i < sizeof($result); $i++)
		{
			$assets[$index] = array();
			$assets[$index][0] = $index;
			$assets[$index][1] = $result[$i]["method_name"];
			$index = $index + 1;
		}
	}
	
	// the name of the kdatabase
	$d1 = $db_knowledge_name;
	// the name of the database
	$d2 = $db_world_state_name;
	// Give state information to user for reference
	$state_message = "";
	$sql_query = "";
	$sql_query = "SELECT DISTINCT ".$d2.".`world_states`.`predicate_label`, ".$d1.".`method_preconditions`";
	$sql_query .= ".`variable`, ".$d2.".`world_states`.`value` FROM ".$d1.".`method_preconditions`";	
	$sql_query .= " LEFT JOIN ".$d2.".`world_states` ON ".$d2.".`world_states`.`predicate_label` =";
	$sql_query .= " ".$d1.".`method_preconditions`.`predicate` WHERE ".$d2.".`world_states`.`world_key`=".$TheWorld;
	$sql_query .= " AND ".$d2.".`world_states`.`asset_name` = \"".$asset_name."\"";	

//echo "<h2>DEBUG LOG (SQL QUERY)</h2>";
//echo "$sql_query";

$result = doQuery($sql_query);
	$sql_query = "";
	// if query succeeds
	if ($result != FALSE){		
		// create selection 
		$variable = $value = $predicate_label = "";
		for ($i = 0; $i < sizeof($result); $i++)
		{
			$predicate_label = $result[$i]["predicate_label"];  // 0
			$variable = $result[$i]["variable"];				// 1
			$value = $result[$i]["value"];						// 2			
			
			if ($i > 0) {
				$sql_query .= ", ";
			}
			$state_message .= "<p>".($i+1).") ".$predicate_label." \"".$value."\"</p>\n";		
		}
	}
	
	
	// define java script functions
	$javaScrp = beginJavaScript();
	$javaScrp .= onSelectJS("doSelection","document.method_select_form.method_list", "document.method_select_form.method_name");
	$javaScrp .= endJavaScript();

	beginPage("Asset Methods", "style.css", $javaScrp);
	beginForm("method_select_form", $asset_name." - Asset Method List", "choose_method_parameters.php", "POST", "");
	message("Current state of \"". $asset_name."\"", $state_message);
	//onclick_button("Home", "- The asset home page", "window.location = 'show_asset_methods.php?user_key=".$user_key."';");
	dropdownBox("method_list", "Please select a method", $assets, "doSelection();");
	hiddenField("method_name", "");
	hiddenField("user_key", $user_key);
	hiddenField("asset_name", $asset_name);
	hiddenField("asset_key", $asset_key);
	hiddenField("user_name", $user_name);
	echo "<p></p>";
	echo "<div align=center>";
	button("Submit");
	echo "</div>";
	endForm();
}else{

	beginPage("Error: Malformed URL", "style.css", "");
	message("Error: Malformed URL", "Certain features are not present in the URL, please revise.");
}

endPage();

</script>