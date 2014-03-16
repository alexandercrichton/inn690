<?php
// This script contains tool functions used in most of the other php scripts
// It exists to remove reused code and improve readability of the project
// Author: Joseph Hitzke
// Date: 22/11/2010

// This function tells if variables have been passed by a form with the method given
// $method - the name of the method in string form
// Pre: Need to check if a form has been submitted
// Post: Have checked if the form was submitted
function formPassed($method) {
	return ($_SERVER['REQUEST_METHOD'] == $method) ? true : false;
}

// This function gets all the variables listed for post data
// $array - list of keys wanted to get from the post data
// Pre: Data posted to page via post and wanted to be extracted
// Post: Data extracted and returned
function getPostData ($array) {
	$result = array();
	foreach($array as $str) {
		$result[$str] = getPostVar($str);
	}
	return $result;
}

// This function gets all the variables listed for url data
// $array - list of keys wanted to get from the url data
// Pre: Data posted to page via url and wanted to be extracted
// Post: Data extracted and returned
function getURLData ($array) {
	$result = array();
	foreach($array as $str) {
		$result[$str] = getURLVar($str);
	}
	return $result;
}

// This function takes a string for the variable supposedly passed by post
// $str - the name of the variable passed by post
// Pre: Need to check and get a post variable
// Post: Have a post variable if it exists, else it is null
// Note: Ternary condition :: boolean check ? return if true : return if false
// 		 Can be used in returns or $var =
function getPostVar($str) {
	return isset($_POST[$str]) ? $_POST[$str] : null;
}

// This function takes a string for the variable supposedly passed in the url
// $str - the name of the variable passed in the url
// Pre: Need to check and get a variable from the url
// Post: Have a variable from the url if it exists, else it is null
// Note: Refer to ternary note on getPostVar()
function getURLVar($str) {
	return isset($_GET[$str]) ? $_GET[$str] : null;
}

// This function checks all the variables given to it against null and returns
// false if any of then == null
// $array - a singular variable or an array of variables
// Pre: Need to check variables gotten by post or get
// Post: Have checked all variables and know if any aren't there
// Note: This will work with any variables gotten with getPostVar() or getURLVar()
function checkVars($array) {
	if(is_array($array)) {
		foreach($array as $obj) {
			if($obj == null) {
				return false;
			}
		}
		return true;
	}
	return $array == null;
}
	
?>