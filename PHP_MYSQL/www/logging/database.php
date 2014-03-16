<script language="php">
// Functions in this script access the hospital processes database
// Author: Rune Rasmussen
// Date: 1/5/2010



//===== edit these ========================

// the set of database credentials
$username="root"; // The user name
$password=""; // The password
$database="veis_logging"; // the name of the database
$host = "localhost"; // the host

//=========================================


$rows_affected=0;// the number of row affected

// A database query function for SQL queries
function db_query($query)
{
	// get the global variables
	global $username, $password, $database, $host, $rows_affected; 
	
	// connect to the database
	$link = mysql_connect($host,$username,$password);
	// if not able to connect to the database
	if (!$link)
	{
		echo 'Could not connect: ' . mysql_error();
		return FALSE;
	}
	// otherwise select the hospital database
	$db_selected = mysql_select_db($database, $link);
	
	// database selection error
	if (!$db_selected) 
	{
		echo 'Can\'t use ' . $database . ': ' . mysql_error();
		return FALSE;
	}
	
	// insert the patient data
	$result = mysql_query($query);
	
	// check result
	if (!$result) {
		$message  = 'Invalid query: ' . mysql_error() . "\n";
		$message .= 'Whole query: ' . $query;
		echo $message;
		return FALSE;
	}
	// set the rows affected
	$rows_affected = mysql_affected_rows($link);
	// disconnect from the database
	mysql_close($link);
	// return the result
	return $result;
}



</script>
