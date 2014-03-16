<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html>
	<head>
		<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1">
		<title>Predicate Entry</title>
		<script type="text/javascript">
		<!--

		// prevents submit at the form level
		function testSubmit(){
			if(/[a-zA-Z_0-9]/.test(document.domain_insert_form.domain_name.value) ){
				return true;
			}
			alert("Predicate names must not be empty and can only contain letters, digits and underscore characters.");
			return false;
		}
		//-->
		</script>
	</head>
	<body>
		<h1>Knowledge-base Predicate Setup</h1>
		<fieldset>
			<legend>
				<h2>Predicate</h2>
			</legend>
			<script language="php">
				// This script searches and displays assets
				// Author: Rune Rasmussen
				// Date: 22/5/2011
				$predicateName = $_POST["predicate_name"]; 
				// check the submit button
				if (isset($predicateName)) { 
					// include functions to access the h-three database
					include_once "h_three_database.php";
					// define a query
					$sql_query = "SELECT * FROM `predicate_labels` WHERE predicate = \"".$predicateName."\"";
					// perform a query and get the database result
					$result = db_query($sql_query);
					// test success
					if ($result != FALSE){
						if (mysql_num_rows($result) != 0){
							echo "<p><font color=\"red\">Predicate: \"".$predicateName."\" already exists in the database!</font></p>";
						}else{
							// the insert statement
							$sql_query = "INSERT INTO `predicate_labels` (`predicate`) VALUES (\"".$predicateName."\")";
							// perform a query and test success
							if (db_query($sql_query) == FALSE){
								echo "<p><font color=\"red\">Predicate: \"".$predicateName."\" failed to insert.</font></p>";
							}
						}
					}
				}
				</script>
			<form name="predicate_insert_form" action="predicate_entry.php" method="post" onsubmit="return testSubmit()">
				<p>Note, the system will refuse predicate names duplicated in the list below.</p>
				<p>Enter new predicate name: <input type="text" size="30" name="predicate_name"/></p>
				<p></p>
				<div align=center><input type="submit" value="OK" /></div>
				<div><a href="">Predicate configuration page</a></div>
			</form>
		</fieldset>
		<fieldset>
			<legend>
				<h2>Existing Predicates</h2>
			</legend>
			<script language="php">
			// This script searches and displays assets
			// Author: Rune Rasmussen
			// Date: 22/5/2011
			// include functions to access the h-three database
			include_once "h_three_database.php";
			// define a query
			$sql_query = "SELECT * FROM `predicate_labels`";
			// perform a query and get the database result
			$result = db_query($sql_query);
			// test success
			if ($result != FALSE){	
				// create selection 
				while ($row = mysql_fetch_array($result, MYSQL_NUM)) {
					echo "<p>".$row[0]."</p>";
				}
			}
			</script>
		</fieldset>
	</body>
</html>
