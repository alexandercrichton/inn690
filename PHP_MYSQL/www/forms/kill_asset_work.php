<script language="php">
require_once "utility/page_functions.php";
require_once "utility/sql_functions.php";
require_once "sql_world_state.php";
require_once "world_config.php";

// get the user key
$user_key = $_GET["user_key"];
// get the asset key
$asset_key = $_GET["asset"];
// if these are set
if (isset($user_key) && isset($asset_key) && $asset_key != "NULL"){
	// select every event for this user 
	$sql_query = "SELECT * FROM `asset_service_routines` WHERE `asset_key` = \"".$asset_key."\" AND `service_routine` = \"ABORT\"";
	// run query
	$result = doQuery($sql_query);
	// if query succeeds
	if ($result != -1 && sizeof($result) > 0 ){
		//header("Location: show_asset_methods.php?user_key=".$user_key."");
	}
	// remove the work items
	$sql_query = "DELETE FROM `user_events` WHERE `user_key` = \"".$user_key."\" AND `asset_key`= \"".$asset_key."\"";
	// delete
	doQuery($sql_query);
	// add the abort service routine
	$sql_query = "INSERT INTO `asset_service_routines` (`priority`, `asset_key`, `service_routine`, `world_key`) VALUES (0, \"".$asset_key."\", \"ABORT\",".$TheWorld.")";
	// insert
	doQuery($sql_query);
}
// go back to show assets
// header("Location: show_asset_methods.php?user_key=".$user_key."");
beginPage("Completed", "style.css", "");
message("Completed", "You can now close this window");


endPage();
exit();

</script>