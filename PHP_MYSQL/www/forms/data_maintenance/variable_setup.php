<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html>
	<head>
		<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1">
		<title>Variable Entry</title>
		<script type="text/javascript">
		<!--

		// prevents submit at the form level
		function testSubmit(){
			// get the selected value
			var nVal = document.variable_insert_form.domain_names.value;
			if (nVal == -1){
				alert("All variables must have domains, please select a domain from \"DOMAIN NAMES\".");
				return false;
			}
			// test name format
			if(/[a-zA-Z_0-9]/.test(document.variable_insert_form.variable_name.value)){
				return true;
			}
			alert("Predicate names must not be empty and can only contain letters, digits and underscore characters.");
			return false;
		}
		//-->
		</script>
	</head>
	<body>
		<h1>Knowledge-base Variable Setup</h1>
		<fieldset>
			<legend>
				<h2>Predicate</h2>
			</legend>
			<script language="php">
				// This script searches and displays assets
				// Author: Rune Rasmussen
				// Date: 22/5/2011
				$domain = $_POST["domain_names"];
				$variableName = $_POST["variable_name"]; 
				// check the submit button
				if (isset($variableName) && $domain != -1 && $variableName != "") { 
					// include functions to access the h-three database
					include_once "h_three_database.php";
					// define a query
					$sql_query = "SELECT * FROM `variables` WHERE identifier = \"".$variableName."\"";
					// perform a query and get the database result
					$result = db_query($sql_query);
					// test success
					if ($result != FALSE){
						if (mysql_num_rows($result) != 0){
							echo "<p><font color=\"red\">Variable: \"".$variableName."\" already exists in the database!</font></p>";
						}else{
							// the insert statement
							$sql_query = "INSERT INTO `variables` (`identifier`, `domain_name`) VALUES (\"".$variableName."\", \"".$domain."\")";
							// perform a query and test success
							if (db_query($sql_query) == FALSE){
								echo "<p><font color=\"red\">Variable: \"".$variableName."\" failed to insert.</font></p>";
							}
						}
					}
				}
				</script>
			<form name="variable_insert_form" action="variable_setup.php" method="post" onsubmit="return testSubmit()">
				<p>Note, the system will refuse variable names duplicated in the list below.</p>
				<p>Enter new variable name: <input type="text" size="30" name="variable_name"/></p>
				<p></p>
				<div> Then select a variable domain :
					<select name="domain_names">
						<option value="-1">DOMAIN NAMES</option>
						<script language="php">
						// This script searches and displays assets
						// Author: Rune Rasmussen
						// Date: 22/5/2011
						// include functions to access the h-three database
						include_once "h_three_database.php";
						// define a query
						$sql_query = "SELECT * FROM `domains`";
						// perform a query and get the database result
						$result = db_query($sql_query);
						// test success
						if ($result != FALSE){	
							// create selection 
							while ($row = mysql_fetch_array($result, MYSQL_NUM)) {
								echo "<option value=\"".$row[0]."\">".$row[0]."</option>";
							}
						}
						</script>
					</select>
				</div>
				<div align=center><input type="submit" value="OK" /></div>
			</form>
		</fieldset>
		<fieldset>
			<legend>
				<h2>Existing Variables</h2>
			</legend>
			<script language="php">
			// This script searches and displays assets
			// Author: Rune Rasmussen
			// Date: 22/5/2011
			// include functions to access the h-three database
			include_once "h_three_database.php";
			// define a query
			$sql_query = "SELECT * FROM `variables`";
			// perform a query and get the database result
			$result = db_query($sql_query);
			// test success
			if ($result != FALSE){	
				// create selection 
				while ($row = mysql_fetch_array($result, MYSQL_NUM)) {
					echo "<p>Variable \"".$row[0]."\" over domain \"".$row[1]."\".</p>";
				}
			}
			</script>
		</fieldset>
	</body>
</html>
