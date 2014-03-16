<?php
// This script contains tool functions used to generate the html for pages
// Author: Joseph Hitzke
// Date: 20/01/2011
// Modified by: Rune Rasmussen
// Date: 14/06/2011

// This function places the necessary header on a page
// $title - Title of the page
// $css   - filename / path of the .css
// $other - Any other text java scripts or functions that should sit in the header
function beginPage($title, $css, $other) {
	echo "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\">";
	echo "<html>\n<head>\n<title>".$title."</title>";
	includeCss($css);
	echo $other;
	echo "\n</head>\n<body>";
}

// This function places the necessary footer on a page
function endPage() {
	echo "\n</body>\n</html>";
}

// Include the CSS file
// $file - Filename of the css file
function includeCss($file) {
	echo "\n<style type='text/css'>\n";
	include($file);
	echo "\n</style>\n";
}

// Generates a message on the webpage
// $title - Title of the message
// $msg   - Message to be shown
function message($title, $msg) {
	echo "\n<table border='0'>\n<td>\n<fieldset>\n<legend>".$title."</legend>\n".$msg."\n</fieldset>\n</td>\n</table>";
}

// Begins a fieldset [ie: outline]
// $title - title of the fieldset
function fieldsetStart($title) {
	echo "\n<table border='0'>\n<td>\n<fieldset>\n<legend><h2>".$title."</h2></legend>\n";
}

// Ends the fieldset
function fieldsetEnd() {
	echo "\n</fieldset>\n</td>\n</table>";
}

// This function returns the begining of a javascript section
function beginJavaScript(){
	return "<script type=\"text/javascript\">\n<!--\n"; 
}

function paragraph($pretag, $msg, $endtag)
{
	echo "\n<p>".$pretag.$msg.$endtag."</p>";
}

// This function returns the end of a javascript section
function endJavaScript(){
	return "//-->\n</script>\n";
}

// This function returns a string of javascript that checks
// the selection in a form item does not have the value "NULL"
// $func : The function name (only)
// $item : The full document path of the item
function isValidSelectionJS($func, $item){
	// make sure the form fields are completed correctly
	return "function ".$func."() {\n var nVal = ".$item.".value;\n return (nVal != 'NULL');\n}\n";
}

// This function returns a string of javascript that checks
// the selection in a form item does not have the value "NULL"
// $func : The function name (only)
// $itemA : The full document path of the selection item
// $itemB : The full document path of the filemane item
function isValidSelectionAndFilenameJS($func, $itemA, $itemB) {
	// make sure the form fields are completed correctly
	return "function ".$func."() {\n
	var myRegExp = /[^a-zA-Z0-9_]/;\n
	if (".$itemB.".value == \"\" || myRegExp.test(".$itemB.".value)) {\n
		alert(\"YAWL Filename can only contain letters, numbers and underscore.\");\n
		return false;
	}\n
	var nVal = ".$itemA.".value;\n 
	return (nVal != 'NULL');\n}\n";
}

// Given a document path to a selection name and a document path
// to a field name, this function tests if the selection was "NULL"
// and if not then transfers the selected text to the field
// $func : The function name (only)
// $itemA : the full document path of the selection name
// $itemB : the full document path of the field name
function onSelectJS($func, $itemA, $itemB){
	return "function ".$func."() {\n 
	var nVal = ".$itemA.".value;\n
	if (nVal != 'NULL'){\n
		// get the selected index\n
		var w = ".$itemA.".selectedIndex;\n
		// set the device name with the selected name\n
		".$itemB.".value = ".$itemA.".options[w].text;\n
	}\n
}\n";
}

// Given a document path to a selection name
// $func : The function name (only)
// $loc : The redirection address
// $val : The get value
function locWithValJS($func, $loc, $val){
	return "function ".$func."() {\n
	var gVal = ".$val.".value;\n
	if (gVal != 'NULL'){\n
		var str = \"".$loc."\"\n;
		window.location = str + gVal;\n
	}\n
}\n";
}

// Given a string, this function tests
// if the first character is an underscore
function testUnderscore($str)
{
	return ($str[0] == '_');
}

// Given a string, this function
// creates a new string that is the
// copy if the given string but with
// every space replaced with an underscore
function uscoreString($str)
{
	return str_replace(" ", "_", $str);
}

// Given a string, this function
// creates a new string that is the
// copy if the given string but with
// every underscore replaced with a space
function deuscoreString($str)
{
	return str_replace("_", " ", $str);
}

// This function calculate and returns the
// union between two arrays
function array_union($left, $right) {
	// create the return array
	$retVal = array();
	// for each element add to the array
	foreach ($right as &$valueB) {
		$retVal[] = $valueB; 
	}
	// for every element in the left array
	foreach ($left as &$valueA) {
		// the can add varaible
		$canAdd = 1;
		// for each element in right
		foreach ($right as &$valueC) {
			// if equal
			if (strcmp($valueA, $valueC) == 0) {
				$canAdd = 0;
			}
		}
		// if can add
		if ($canAdd == 1) {
			$retVal[] = $valueA;
		}
	}
	// return the new array
	return $retVal;
}

// This function calculate and returns the
// the left array minus the right array
function array_minus($left, $right) {
	// create the return array
	$retVal = array();
	// for every element in the left array
	foreach ($left as &$valueA) {
		// the can add varaible
		$canAdd = 1;
		// for each element in right
		foreach ($right as &$valueB) {
			// if equal
			if (strcmp($valueA, $valueB) == 0) {
				$canAdd = 0;
			}
		}
		// if can add
		if ($canAdd == 1) {
			$retVal[] = $valueA;
		}
	}
	// return the new array
	return $retVal;
}

function echo_array($arr) {
	// for every element in the left array
	foreach ($arr as &$value) {
		echo $value." ";
	}
	echo "<p></p>";
}

function error_page($heading, $message, $css)
{
	beginPage("Simulation message", $css, "");
	message("Error: ".$heading, $message);
	endPage();	
}

?>