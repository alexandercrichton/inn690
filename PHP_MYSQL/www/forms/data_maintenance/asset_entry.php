<script language="php">
require_once $_SERVER["DOCUMENT_ROOT"] . "forms/utility/sql_functions.php";
include_once $_SERVER["DOCUMENT_ROOT"] . "forms/sql_knowledge.php";

// This script searches and displays assets
// Author: Rune Rasmussen
// Date: 22/5/2011
$assetName = "";
$assetSubmitResult = "";
$existingAssets = "";

// If the asset name is set, attempt to insert this asset into the database
if (isset($_POST["asset_name"])) { 
	// Retrive all the assets with the given name from the database
	$assetName = $_POST["asset_name"];
	$sql_query = "SELECT * FROM `asset` WHERE name = \"".$assetName."\"";
	$result = doQuery($sql_query);
	if ($result != -1){
		if (sizeof($result) != 0){
			$assetSubmitResult .= '<p><font color="red">Asset: "'.$assetName.'" already exists in the database!</font></p>';
		}else{
			$sql_query = "INSERT INTO `asset` (`name`) VALUES (\"".$assetName."\")";
			if (doQuery($sql_query) == -1){
				$assetSubmitResult .= '<p><font color="red">Asset: "'.$assetName.'" failed to insert.</font></p>';
			}
		}
	}
}

// Get and display all existing assets for reference
$sql_query = "SELECT * FROM `asset`";
$result = doQuery($sql_query);
if ($result != -1){	
	for ($i = 0; $i < sizeof($result); $i++)
	{
		$existingAssets .= "<p>". $result[$i]["name"] ."</p>";
	}
}
</script>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html>
	<head>
		<meta http-equiv="content-type" content="text/html; charset=iso-8859-1" />
		<title>Asset Entry</title>
		<script type="text/javascript">
		<!--

		// prevents submit at the form level
		function testSubmit(){
			if(/[a-zA-Z_0-9]/.test(document.asset_insert_form.asset_name.value) ){
				return true;
			}
			alert('Asset names must not be empty can only contain letters, digits and underscore characters.');
			return false;
		}
		//-->
		</script>
	</head>
	<body>
		<h1>Knowledge-base Asset Setup</h1>
		<fieldset>
			<legend>
				<h2>Assets</h2>
			</legend>
			<?php echo $assetSubmitResult ?>
			<form name="asset_insert_form" action="asset_entry.php" method="post" onsubmit="return testSubmit()">
				<p>Note, the system will refuse asset names duplicated in the list below.</p>
				<p>Enter new asset name: <input type="text" size="30" name="asset_name"/></p>
				<p></p>
				<div align=center><input type="submit" value="OK" /></div>
				<div><a href="">Asset configuration page</a></div>
			</form>
		</fieldset>
		<fieldset>
			<?php echo $existingAssets; ?>
		</fieldset>
	</body>
</html>