<?php
// This script contains tool functions used to create forms on the webpage
// Author: Joseph Hitkze
// Date: 20/01/2011

// Begins the form
// $name   - Name of the form
// $desc   - Description of the form, ex: "Driver Registration"
// $action - The action taken by the form, ex: "driver_registration.php?case=id"
// $method - The method in which the form will be processed, ex: POST / GET
function beginForm($name, $desc, $action, $method) {
	echo "\n<table border='0'>\n<td>\n<form name='".$name."' action='".$action."' method='".$method."'>",
	"\n<fieldset>\n<legend>".$desc."</legend>";
}

// Ends the form
function endForm() {
	echo "\n</form>\n</fieldset>\n</td>\n</table>";
}

// Dropdown box [Values should be in pairs array(array("value", "desc"),)
// $name   - Name of the dropdown box field
// $desc   - Description of the dropdown box field
// $values - The value pairs of the dropdown box
// $onselect - The function to call on selection
function dropdownBox($name, $desc, $values, $onselect = "") {
	echo "\n<label>".$desc.": </label>\n<select name='".$name."' onchange='".$onselect."'>";
	for($i = 0; $i < sizeof($values); $i++) {
		if(is_int($values[$i][0])) {
			echo "\n<option value=".$values[$i][0]." /> ".$values[$i][1]."</option>";
		} else {
			echo "\n<option value='".$values[$i][0]."' /> ".$values[$i][1]."</option>";
		}
	}
	echo "\n</select>\n<br>";
}

// Checkboxes [Values should be in pairs array(array("value", "desc"),)
// $name   - Name of the check box field
// $desc   - Description of the check box field
// $values - The value pairs of the check box list
function checkBoxes($name, $desc, $values) {
	echo "\n<fieldset>\n<legend>".$desc."</legend>";
	for($i = 0; $i < sizeof($values); $i++) {
		echo "\n<input type='checkbox' name='".$name."' value='".$values[$i][0]."' /> ".$values[$i][1]."<br>";
	}
	echo "\n</fieldset>";
}

// Radio button list [Values should be in pairs array(array("value", "desc"),)
// $name   - Name of the radio button field
// $desc   - Description of the radio button field
// $values - The value pairs of the radio button field
function radioButtons($name, $desc, $values) {
	echo "\n<fieldset>\n<legend>".$desc."</legend>";
	for($i = 0; $i < sizeof($values); $i++) {
		echo "\n<input type='radio' name='".$name."' value='".$values[$i][0]."' /> ".$values[$i][1]."<br>";
	}
	echo "\n</fieldset>";
}

// Generates a text field portion of the form
// $desc   - Description of the text field
// $values - Value pairs of the text fields
function textFields($desc, $values) {
	echo "\n<fieldset>\n<legend>".$desc."</legend>\n<table border='0'>";
	for($i = 0; $i < sizeof($values); $i++) {
		echo "\n<tr>\n<td>\n<label>".$values[$i][0].": </label>\n</td>\n<td>\n<input type='text' name='".
		$values[$i][1]."' class='textfield' max='".$values[$i][2]."' value=' '/>\n</td>";
	}
	echo "\n</table>\n</fieldset>";
}

// Generates a button on the form
// $text - The text displayed on a form
function button($text) {
	echo "\n<input type='submit' value='".$text."' />";
}

// Generates a button with a label for
// redirection purposes and such
// $text - The text displayed on a form
// $label - The text to display next to the button?
// $addr - The function to call when clicked
function onclick_button($text, $label, $addr) {
	echo "\n<p><input type=\"button\" value=\"".$text."\" onclick=\"".$addr."\">".$label."</p>";
}

// Generates a hiddenField on the form
// $name - the name of the field
// $value - the value of the field
function hiddenField($name, $value){
    echo "\n<input type='hidden' name='".$name."' value='".$value."' />";
}
?>