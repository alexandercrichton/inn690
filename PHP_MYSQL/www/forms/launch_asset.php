<!-- The purpose of this script is to launch interaction with an asset by the user.
The interaction is firstly registered with the database, as a "user asset event".
This only happens if the user is a valid member of the simulation (to prevent tampering)
If successful, the user is redirected to the asset's choose method page. -->
<script language="php">
require_once "utility/page_functions.php";
require_once "utility/sql_functions.php";
include_once "sql_world_state.php";

function redirectToError($heading, $message)
{
	error_page($heading, $message, "style.css");
}

// get the user key
$user_key = $_GET["user_key"];
// get the asset name
$asset_name = $_GET["asset_name"];
// get the asset key
$asset_key = $_GET["asset_key"];
// get the user's name
$user_name = $_GET["user_name"];

// if the values are set
if	(isset($user_key) && isset($asset_name) && isset($asset_key))
{
	$isValidUser = true;
	// TODO: Perform registered player validation
	if ($isValidUser) {	
		// insert the event
		$sql_query = makeInsQuery("user_events", array( array("user_key", $user_key),
														array( "asset", $asset_name),
														array("asset_key", $asset_key)));
		$result = doQuery($sql_query);
		
		// if successful, redirect to get_asset_methods
		if ($result != -1) {
			header("Location: get_asset_methods.php?user_key=".$user_key."&asset_name=".$asset_name."&asset_key=".$asset_key."&user_name=".$user_name);
		}
		else {
			redirectToError("Database query error", "Could not create user event.\n".$sql_query);
		}		
	} else {
		redirectToError("Simulation access error", "You are not part of the simulation");
	}	
}else{
	redirectToError("Something not set in URL", "Make sure user_key, asset_name, and asset_key are set");
}

</script>