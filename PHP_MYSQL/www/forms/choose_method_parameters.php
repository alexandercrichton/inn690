<script language="php">
require_once "utility/page_functions.php";
require_once "utility/form_functions.php";
require_once "utility/sql_functions.php";
require_once "world_config.php";
require_once "sql_knowledge.php";
require_once "sql_world_state.php";

//get the user key
$user_key = $_POST["user_key"];
//get the asset name
$asset_name = $_POST["asset_name"];
//get the asset key
$asset_key = $_POST["asset_key"];
//get the method name
$method_name = $_POST["method_name"];
// get the user's name
$user_name = $_POST["user_name"];


//make sure everything is set
if (isset($asset_key) && isset($asset_name) && isset($user_key) && isset($method_name)) {
	
	$result = getPlayerAssetEvent($user_key, $asset_key);
	if ($result == FALSE || sizeof($result) == 0 ){
		error_page("Database query result", "You aren't registered against this asset.\n".$sql_query, "style.css");
	}
	
	// the name of the kdatabase
	$d1 = $db_knowledge_name;
	// the name of the database
	$d2 = $db_world_state_name;
	
	// get the method asset based preconditions with current value
	$sql_query = "SELECT ".$d2.".`world_states`.`predicate_label`, ".$d1.".`method_preconditions`";
	$sql_query .= ".`variable`, ".$d2.".`world_states`.`value` FROM ".$d1.".`method_preconditions`";	
	$sql_query .= " LEFT JOIN ".$d2.".`world_states` ON ".$d2.".`world_states`.`predicate_label` =";
	$sql_query .= " ".$d1.".`method_preconditions`.`predicate` WHERE ".$d1.".`method_preconditions`.";
	$sql_query .= "`method_name` = \"".$method_name."\" AND ".$d2.".`world_states`.`world_key`=".$TheWorld;
	$sql_query .= " AND ".$d2.".`world_states`.`asset_name` = \"".$asset_name."\"";
	
	$result = doQuery($sql_query);
	$sql_query = "";
	// create java script
	$JS = beginJavaScript();
	// $JS .= 
	$JS .= endJavaScript();	
	
	// generate a form to display data
	beginPage("Methods Parameters", "style.css", $JS );
	beginForm("parameter_select_form", "Method Parameter Lists", "process_method_call.php", "POST", "");
	hiddenField("user_key", $user_key);
	hiddenField("asset_name", $asset_name);
	hiddenField("asset_key", $asset_key);
	hiddenField("method_name", $method_name);
	hiddenField("user_name", $user_name);
	
	$state_message = "";
	// if query succeeds
	if ($result != FALSE){		
		// query for free variables
		$sql_query .= "SELECT `variable` FROM ".$d1.".`method_parameters` WHERE `method_name` = \"".$method_name;
		$sql_query .= "\" AND `variable` NOT IN ("; 
		
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
			// known parameter values for next page
			hiddenField(uscoreString($variable), $value);
			$sql_query .= "\"".$variable."\"";
			$state_message .= "<p>".($i+1).") ".$predicate_label." \"".$value."\"</p>\n";		
		}

		$sql_query .= ")";
	}
	message("Current state of \"". $asset_name."\"", $state_message);
	// define an array
	$param_array = array();
	// if something to query
	if	($sql_query != ""){
		// run query
		$param_array = doQuery($sql_query);
	}
	// show options
	echo "<p>Options for action \"".$method_name."\":</p>\n";
	// if the list is empty
	if (sizeof($param_array) == 0) {
		echo "<p>There are currently no options for this method </p>";
	} else {
		// for each option
		for ($j = 0; $j < sizeof($param_array); $j++) {

			$sql_query = "SELECT `value` FROM ".$d1.".`domain_values` WHERE `name` = ";
			$sql_query .= "(SELECT `domain_name` FROM ".$d1.".`variables` WHERE `identifier` = \"";
			$sql_query .= $param_array[$j]["variable"]."\")";

			$result = doQuery($sql_query);
			if ($result != FALSE){
				echo "<label>".$param_array[$j]["variable"].": </label>\n<select name='".uscoreString($param_array[$j]["variable"])."' onchange=''>\n";
				for ($i = 0; $i < sizeof($result); $i++)
				{
					echo "<option value=\"".$result[$i]["value"]."\"/> ".$result[$i]["value"]."</option>\n";
				}
				echo "</select>\n<br>\n";
			}
		}
	}
	echo "<br>";
	echo "<div align=center>";
	button("Submit");
	echo "</div>";
	endForm();
}else{

	beginPage("Error: Malformed URL", "style.css", "");
	message("Error: Malformed URL", "Certain features were not present in the URL, please revise.");
}

endPage();

</script>