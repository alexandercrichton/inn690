<?php

$db_world_state_name = "veis_world_states";

// Database connection variables
$db = array(	"username" 	=> "root",
				"password" 	=> "",
				"server" 	=> "127.0.0.1",
				"database" 	=> $db_world_state_name);

// Create new connection
$mysqli = new mysqli($db["server"], $db["username"], $db["password"], $db["database"]);

// Unset the database connection variable for security measures
unset($db);

function world_query($query)
{
	// Do the query
	$result = $GLOBALS['mysqli']->query($query);
	printf("%s\n", mysqli_error($GLOBALS['mysqli']));
	if($result) {
		// If it was a select sql statement
		if(isset($result->num_rows)) {
			if($result->num_rows > 0) {
				// Get the results
				$array = array();
				while($row = $result->fetch_array(MYSQLI_ASSOC)) {
					array_push($array, $row);
				}
				return $array;
			}
		}
		// If it was any other sql statement
		if(isset($result->affected_rows)) {
			if($result->affected_rows > 0) {
				return 0;
			}
		}
		// If result is 1 it was success
		if(is_bool($result)) {
			return 0;
		}
	}
	// Default return, something went wrong
	return -1;
}

// The following functions encapsulate commonly used functionality relating to
// the world state database.
// Author: Rune Rasmussen
// Date: 6/5/2011
function vacateWorld($world)
{
	// delete query
	$sqlQuery = "DELETE FROM `world_states` WHERE `world_key` =".$world;
	// run query
	return world_query($sqlQuery);
}

function getAssetStates($world, $asset)
{
	// select query
	$sqlQuery = "SELECT `predicate_label`, `value` FROM `world_states` WHERE `world_key` =";
	$sqlQuery .= $world." AND `asset_name` = \"".$asset."\"";
	// run the query
	$result = world_query($sqlQuery);
	// run query
	return $result;
}

function testPredicate($world, $asset, $pred, $val)
{
	// select query
	$sqlQuery = "SELECT * FROM `world_states` WHERE `world_key` =".$world." AND `asset_name` = \"";
	$sqlQuery .= $asset."\" AND `predicate_label` =\"".$pred."\" AND `value` = \"".$val."\"";
	
	// run query
	$result = world_query($sqlQuery);
	
	// if query succeeds
	if ($result != -1){
		//echo mysql_num_rows($result);
		// test number of rows
		return (sizeof($result) > 0);
	}
	// otherwise false
	return FALSE;
}

function insertPredicate($world, $asset, $pred, $val)
{
	// insert query
	$sqlQuery = "INSERT INTO `world_states` (`world_key`, `asset_name`, `predicate_label`, `value`) ";
	$sqlQuery .= " VALUES (".$world.", \"".$asset."\", \"".$pred."\", \"".$val."\")";
	// run query
	return world_query($sqlQuery);
}

function removePredicate($world, $asset, $pred, $val)
{
	// delete query
	$sqlQuery = "DELETE FROM `world_states` WHERE `world_key` =".$world." AND `asset_name` = \"";
	$sqlQuery .= $asset."\" AND `predicate_label` =\"".$pred."\" AND `value` = \"".$val."\"";
	// run query
	return world_query($sqlQuery);
}

function writeServiceCall($ass_key, $serv_rout, $priority, $world_key)
{
	// insert query
	$sqlQuery = "INSERT INTO `asset_service_routines` (`priority`, `asset_key`, `service_routine`, `world_key`) ";
	$sqlQuery .= " VALUES (".$priority.", \"".$ass_key."\", \"".$serv_rout."\",".$world_key.")";
	// run query

	//file_put_contents("C:\\Users\\n8376476\\Documents\\Asset_Service_rountine.txt", $sqlQuery , FILE_APPEND | LOCK_EX);
	//file_put_contents("C:\\wamp\\www\\Asset_Service_rountine.txt", $sqlQuery."\r\n" , FILE_APPEND | LOCK_EX);

	return world_query($sqlQuery);
}

function setPlayerState($user_key, $state)
{
	// delete query
	$sqlQuery = "INSERT INTO `player_states` (`user_key`, `state`)";
	$sqlQuery .= " VALUES (\"".$user_key."\", ".$state.") ON DUPLICATE KEY UPDATE `state` = ".$state;
	// if the state deleted
	return world_query($sqlQuery);
}

function getPlayerState($user_key)
{
	// get the state
	$sqlQuery = "SELECT `state` FROM `player_states` WHERE `user_key` =\"".$user_key."\"";
	// run query
	$result = world_query($sqlQuery);
	// if query succeeds
	if ($result != -1){
		// test number of rows
		if (sizeof($result) > 0) {
			return $result[0]["state"];
		}
	}
	return -1;
}

function getPlayerAssetEvent($user_key, $asset_key)
{
	// select every event for this user 
	$sql_query = "SELECT * FROM `user_events` WHERE `user_key` = \"".$user_key."\" AND `asset_key` = \"".$asset_key."\"";
	// run query
	return world_query($sql_query);
	
}

?>