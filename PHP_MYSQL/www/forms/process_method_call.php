<script language="php">
require_once "utility/page_functions.php";
require_once "utility/form_functions.php";
require_once "utility/sql_functions.php";
require_once "world_config.php";
require_once "sql_knowledge.php";
require_once "sql_world_state.php";
require_once "../logging/LogEventDirect.php";



// get the user key
$user_key = $_POST["user_key"];
// get the asset name
$asset_name = $_POST["asset_name"];
// get the asset key
$asset_key = $_POST["asset_key"];
// get the method name
$method_name = $_POST["method_name"];
// get the user's name
$user_name = $_POST["user_name"];

//echo "<b>[DEBUG]</b> ".$user_key." - ".$asset_name." - ".$asset_key." - ". $method_name." - ". $user_name;

// make sure everything is set
if (isset($asset_key) && isset($asset_name) && isset($user_key) && isset($method_name)) {
	
	// select every event for this user 
 	$result = getPlayerAssetEvent($user_key, $asset_key);
 	if ($result == FALSE || sizeof($result) == 0 ){
 		error_page("Database query result", "You aren't registered against this asset.\n".$sql_query, "style.css");
 		exit;
 	}
	
	
 	// the precondition label
 	$precon_label = "";
 	// the postcondition A label
 	$postA_label = "";
 	// the postcondition B label
 	$postB_label = "";
 	// the name of the database
 	$d1 = $db_knowledge_name;
 	// get the method variables
 	$sql_query = "SELECT `variable` FROM `".$d1."`.`method_parameters` WHERE `method_name` = \"".$method_name."\"";
 	// run query
 	$result = doQuery($sql_query);
 	// define a results array
 	$variables = array();
 	// if query succeeds
 	if ($result != -1){
		for ($i = 0; $i < sizeof($result); $i++) {
			$variables[$i] = $result[$i]["variable"];
		}
 	}
 	
 	// select the precondition predicates
 	$sql_query = "SELECT `predicate`, `variable` FROM ".$d1.".`method_preconditions` WHERE ";
 	$sql_query .= "`method_name` = \"".$method_name."\"";
 	// run query
 	$result = doQuery($sql_query);
 	// define a results array
 	$pred_values = array();
 	// if query succeeds
	//echo "<p>HERE0</p>";
	
	//echo $sql_query;
	
 	if ($result != -1){

		$predicate = $variable = "";
		//echo "<p>HERE1</p>";
		for ($i = 0; $i < sizeof($result); $i++) {
			$predicate = $result[$i]["predicate"];	// 0
			$variable = $result[$i]["variable"];	// 1
			
			// get the variable value
 			$val = $_POST[uscoreString($variable)];
			if ( isset($val) ){
 				$pred_values[$i] = array();
 				$pred_values[$i][0] = $predicate;
 				$pred_values[$i][1] = $variable;
 				$pred_values[$i][2] = $val;
 				if (testUnderscore($predicate) ) {
 					$precon_label .= $predicate.",".$val.";";
 				} else {
 					$precon_label .= $predicate.",".$val.",".$asset_name.";";
 				}
 			} else {
 				// generate a form to display data
				error_page("Critial Error",  "The variable \"".$variable."\" did not have a value.", "style.css");
 				$pred_values = array();
 				break;
 			}
		}
 	}
//echo "<p>HERE3</p>";
	//a test value
	$test = FALSE;
	//if all went well
	if (sizeof($pred_values) > 0) {
	
	//echo "<p>HERE4</p>";
		// variable Boolean
 		$var_test = TRUE;
 		$test = TRUE;
 		// the precondition message
 		$precon_msg = "";
 		// generate a form to display data
 		beginPage("Validations", "style.css", "");
 		beginForm("method_validation_form", "Validations", "kill_asset_work.php", "GET", "");
 		hiddenField("user_key", $user_key);
 		hiddenField("asset", $asset_key);
 		// test the precondition
		for ($j = 0; $j < sizeof($pred_values); $j++) {
 			// if the predicate has an underscore
 			if (testUnderscore($pred_values[$j][0])){
 				// check predicate
 				$var_test = testPredicate($TheWorld, $free_predicate, $pred_values[$j][0], $pred_values[$j][2]);
			}else{
 				// check predicate
 				$var_test = testPredicate($TheWorld, $asset_name, $pred_values[$j][0], $pred_values[$j][2]);
 			}
 			$test = $test && $var_test;
			// if the test is false
 			if (!$var_test) {
 				$precon_msg  .= "<p>Cannot proceed because ".$pred_values[$j][2]." is not ".deuscoreString($pred_values[$j][0])."</p>\n";
			}
		}
		
		
 		// if the precondition is satisfied
		if ($test) {
			$precon_msg  .= "<div align=center><p>Good!</p></div>\n";			

			// Update the world state

			// select the precondition predicates
			$sql_query = 
			"SELECT `predicate`, `variable`, `state` FROM ".$d1.".`method_post_conditions` WHERE ";
			$sql_query .= "`method_name` = \"".$method_name."\"";
			// run query
			$result = doQuery($sql_query);
			// define a results array
			$pr_val_states = array();
			// if query succeeds
			if ($result != -1){
				// define an index
				$predicate = $variable = $state = "";
				
				// for each row
				for ($i = 0; $i < sizeof($result); $i++) {
					$predicate = $result[$i]["predicate"];	// 0
					$variable = $result[$i]["variable"];	// 1
					$state = $result[$i]["state"];			// 2
					
					// Get the variable value
					$val = $_POST[uscoreString($variable)];
					if ( isset($val) ){
						$pr_val_states[$i] = array();
						$pr_val_states[$i][0] = $predicate;
						$pr_val_states[$i][1] = $variable;
						$pr_val_states[$i][2] = $val;
						$pr_val_states[$i][3] = $state;
						if ($state == 0){
							if (testUnderscore($predicate) ) {
								$postB_label .= $predicate.",".$val.";";
							} else {
								$postB_label .= $predicate.",".$val.",".$asset_name.";";
							}
						}else{
							if (testUnderscore($predicate) ) {
								$postA_label .= $predicate.",".$val.";";
							} else {
								$postA_label .= $predicate.",".$val.",".$asset_name.";";
							}
						}
					}
				}
			}
			// test the precondition
			for ($j = 0; $j < sizeof($pr_val_states); $j++) {
				if	($pr_val_states[$j][3] == 0){
					// if the predicate has an underscore
					if (testUnderscore($pr_val_states[$j][0])){
						//echo "Remove ".$pr_val_states[$j][0]." ".$pr_val_states[$j][2];
						// remove the predicate
						removePredicate($TheWorld, $free_predicate, $pr_val_states[$j][0], $pr_val_states[$j][2]);
					}else{
						//echo "Remove ".$pr_val_states[$j][0]." ".$pr_val_states[$j][2];
						// remove the predicate
						removePredicate($TheWorld, $asset_name, $pr_val_states[$j][0], $pr_val_states[$j][2]);
					}
				}
			}
			// array to collect post codition values for logging
			$loggingData = array();
			
			// test the precondition
			for ($j = 0; $j < sizeof($pr_val_states); $j++) {
				if	($pr_val_states[$j][3] == 1){
					// $loggingData["variable"] = val
					$loggingData[$pr_val_states[$j][1]] = $pr_val_states[$j][2];
					// if the predicate has an underscore
					if (testUnderscore($pr_val_states[$j][0])){
						//echo "Insert ".$pr_val_states[$j][0]." ".$pr_val_states[$j][2];
						// insert the predicate
						insertPredicate($TheWorld, $free_predicate, $pr_val_states[$j][0], $pr_val_states[$j][2]);
					}else{
						//echo "Insert ".$pr_val_states[$j][0]." ".$pr_val_states[$j][2];
						// insert the predicate
						insertPredicate($TheWorld, $asset_name, $pr_val_states[$j][0], $pr_val_states[$j][2]);
					}
				}
			}
			// Log action to logging table
			logEvent($user_name, $asset_name, $method_name, "complete", $loggingData); // GAH ADD PARAMETER SELECTIONS. 
			
			// get and send service routines
			$sql_query = 
			"SELECT * FROM ".$d1.".`method_service_calls` WHERE `method_name` = \"".$method_name."\"";
			// run query
			$result = doQuery($sql_query);
			// define a results array
			$serv_routines = array();
			// if query succeeds
			if ($result != -1){
				// for each row
				for ($i = 0; $i < sizeof($result); $i++) {	
					// get the variable value
					$val = $_POST[uscoreString($result[$i]["variable"])];
					// test if set
					if ( isset($val) ){
						// create array
						$serv_routines[$i] = array();
						$serv_routines[$i][0] = $result[$i]["service_call"];
						$serv_routines[$i][1] = $result[$i]["variable"];
						$serv_routines[$i][2] = $val;
					}
				}
			}
			// write service calls
			for ($j = 0; $j < sizeof($serv_routines); $j++) {
				// set routine
				$serv_rout = $serv_routines[$j][0].":".$serv_routines[$j][2];
				// write the service routine
				writeServiceCall($asset_key, $serv_rout, 1, $TheWorld);
			}
			
			// get the player state
			$player_state = getPlayerState($user_key);
			// are we recording actions
			if ($record == $player_state) {
				$label_id = "";
				// insert record of action
				$sqlQuery = "INSERT INTO ".$d1.".`kPN_templates` (`key`, `pre`, `a`, `neg_b`) ";
				$sqlQuery .= " VALUES (NULL, \"".$precon_label."\", \"".$postA_label."\", \"".$postB_label."\")";
				// run query
				$result = doQuery($sqlQuery);				
				if( $result != -1 ){

					$sqlQuery = "SELECT MAX(`key`) FROM ".$d1.".`kPN_templates`";
					// run query
					$result = doQuery($sqlQuery);
					// if query succeeds
					if ($result != -1){
						// test number of rows
						if (sizeof($result) > 0) {
							$label_id = $result[0]["MAX(`key`)"];
						}
					}
				}
				// create the name
				$label_name = $method_name." for ".$asset_name;
				$param_val_l = "";
				foreach ($variables as &$val) {
					$label_name .= " ".$val." ".$_POST[uscoreString($val)];
					$param_val_l .= $val.";".$_POST[uscoreString($val)].";";
				}
				// insert template label
				$sqlQuery = "INSERT INTO ".$d1.".`template_labels` (`key`, `name`, `method_name`";
				$sqlQuery .= ", `asset`, `asset_key`, `actor`, `actor_key`, `param_val_list`) ";
				$sqlQuery .= " VALUES (".$label_id.", \"".$label_name."\", \"".$method_name."\", ";
				$sqlQuery .= "\"".$asset_name."\", \"".$asset_key."\", \"\", \"".$user_key."\", \"".$param_val_l."\")";
				doQuery($sqlQuery);
				
				// get the count from the recorded activities
				$sqlQuery = "SELECT COUNT(*) FROM `recorded_activities`";
				$result = doQuery($sqlQuery);
				// if query succeeds
				if ($result != -1){
					// test number of rows
					if (sizeof($result) > 0)
					{
						$activity_count = $result[0]["COUNT(*)"];
						$sqlQuery = "INSERT INTO `recorded_activities` (`kpn_key`, `name`, `user_key`, `order`) ";
						$sqlQuery .= " VALUES (".$label_id.", \"".$method_name." on ".$asset_name."\", \"".$user_key."\", ".$activity_count.")";						
						doQuery($sqlQuery);
					}
				}
			}
 			// if replaying
 			if ($player_state == $play) {
 				
 			}
		}
		
		message("Precondition validation:", $precon_msg);
		echo "<div align=center>";
		button("OK");
		echo "</div>";
		endForm();
	}
} else {
	// generate a form to display data
	beginPage("Error: Malformed URL", "style.css", "");
	message("Error: Malformed URL", "Certain features were not present in the URL, please revise.");
}

endPage();

</script>