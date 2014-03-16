<?php
    include "./database.php";

	$userName = $_POST['userName'];
    $resource = $_POST['resource'];
	$action = $_POST['action'];
	$transition = $_POST['transition'];

$con = mysql_connect($host,$username,$password);
if (!$con)
  {
  die('Could not connect: ' . mysql_error());
  }

mysql_select_db($database, $con);

if(!mysql_query("LOCK TABLES log_event WRITE, log_event_extra WRITE;", $con))
{
	die('Lock error: ' . mysql_error());
}	

if(!mysql_query("INSERT INTO log_event (recordNum, userName, resource, action, transition)
			VALUES (NULL, '$userName','$resource', '$action', '$transition');", $con))
{
	die('Insert error: ' . mysql_error());
}

foreach ( $_POST as $key => $value )
{
	if(!in_array($key, array("userName", "resource", "action", "transition")))
	{
		if(!mysql_query("INSERT INTO `".$database."`.`log_event_extra` (`recordNum`, `key`, `value`) 
						VALUES (last_insert_id(), '$key', '$value');", $con))
		{
			die('Insert2 error: ' . mysql_error());
		}
	}
}

if(!mysql_query("UNLOCK TABLES;"))
{
	die('Unlock error: ' . mysql_error());
}

mysql_close($con);

echo "added record.";
?>