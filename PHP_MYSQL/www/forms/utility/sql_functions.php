<?php
// This script contains functions used to interface via SQL to the database
// Author: Joseph Hitzke
// Date: 15/01/2011

// Includes the SQL file which connects it to the database
// require_once("sql.php"); --> this should be done per form to connect to the correct database

// This function returns an sql query
// $select - which fields to be selected
// $from   - which table to use
// $where  - the where conditionals for the sql statement
//			 takes the format of { {"variable", "operand", "condition" }, {"variable", "operand", "condition" }, etc. }
function makeQuery($select, $from, $where) {
	$query = "SELECT ".$select." FROM ".$from;
	// Add in conditionals
	if($where != -1)
		$query = $query.whereQuery($where);
	return $query;
}

// Returns an update sql query
// $table - which table to update in the database
// $set   - which variables you want to set, and their values
// $where - the where conditional for the SQL query
// $set & $where takes the same format as $where in makeQuery
function makeModQuery($table, $set, $where) {
	$query = "UPDATE $table SET ";
	$query = $query.valueQuery($set);
	// Add in the where
	if($where != -1)
		$query = $query.whereQuery($where);
	return $query;
}

// Returns an insert into sql query
// $table  - the table which you wish you insert your values into
// $values - paired two dimensional array of the values you wish to insert
function makeInsQuery($table, $values) {
	// Create the base $query string
	$query = "INSERT INTO $table ";
	
	// Use specific collumns
	$query = $query."(";
	for($i = 0; $i < sizeof($values); $i++) {
		// Add in commas
		if($i > 0)
			$query = $query.", ";
		$query = $query.$values[$i][0];
	}
	$query = $query.") ";
		
	// Insert the values
	$query = $query."VALUES ";
	$query = $query."(";
	
	for($i = 0; $i < sizeof($values); $i++) {
	
		// Add in commas
		if($i > 0)
			$query = $query.", ";
			
		// Add in proper syntax
		if(is_int($values[$i][1])) {
			$query = $query.$values[$i][1];
		} else {
			$query = $query."'".$values[$i][1]."'";
		}
	}
	$query = $query.")";
	return $query;
}	

// Returns a value assigning part of an sql query
// $values - array of pairs of values array(array("name", $value))
function valueQuery($values) {
	$query = "";
	// Add in the variables being set
	for($i = 0; $i < sizeof($values); $i++) {
		// Add in the commas
		if($i > 0) {
			$query = $query.", ";
		}
		// Add in the var name
		$query = $query."`".$values[$i][0]."`=";
		// Add in the value
		if(is_int($values[$i][1])) {
			$query = $query.$values[$i][1];
		} else {
			$query = $query."'".$values[$i][1]."'";
		}
	}
	return $query;
}

// Returns the where part of an sql query
// $where - same format as the $where in makeQuery
function whereQuery($where) {
	$wQuery = ' WHERE ';
	// For all the conditionals in the array
	for($i = 0; $i < sizeof($where); $i++) {
		// If this is a trailing conditional add AND
		if($i > 0)
			$wQuery = $wQuery." AND ";
			
		// If the variable is an integer address it as so
		if(is_int($where[$i][1])) {
			$wQuery = $wQuery."`".$where[$i][0]."`".$where[$i][1].$where[$i][2];
		} else {
			$wQuery = $wQuery."`".$where[$i][0]."`".$where[$i][1]."'".$where[$i][2]."'";
		}
	}
	return $wQuery;
}

// Does a query and returns results as an array
// $query  - is the sql query
function doQuery($query) {
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

// Closes the current mysqli connection
function closeConnection() {
	mysqli_close($_GLOBALS['mysqli']);
}

// Sends a form complete signal to lsl
function formComplete($form) {
	$req = new HttpRequest("http://127.0.0.1:8081/forms/update_world.php?form=".$form, HttpRequest::METH_GET);
	try {
		$req->send();
	} catch (HttpException $ex) {
		echo $ex;
		exit(0);
	}
}

?>