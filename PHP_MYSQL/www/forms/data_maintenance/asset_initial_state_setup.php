<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html>
	<head>
		<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1">
		<title>Asset Initial State Setup</title>
	</head>
	<body>
		<fieldset>
			<legend>
				<h2>Asset: Initial State Setup</h2>
			</legend>
			<form name="asset_state_form" action="asset_initial_state_setup.php" method="post" onsubmit="return testSubmit()">
				<p></p>
				<div>
					Select Asset :
					<select name="asset_names" onchange="asset_state_form.submit();">
						<option value="-1">ASSETS</option>
						<script language="php">
						// This script searches and displays assets
						// Author: Rune Rasmussen
						// Date: 22/5/2011
						// include functions to access the h-three database
						include_once "h_three_database.php";
						// get asset names
						$assetNames = $_POST["asset_names"];
						// define a query
						$sql_query = "SELECT * FROM `asset`";
						// perform a query and get the database result
						$result = db_query($sql_query);
						// test success
						if ($result != FALSE){
							if (isset($assetNames) && $assetNames != -1){
								// create selection 
								while ($row = mysql_fetch_array($result, MYSQL_NUM)) {
									if ($assetNames == $row[0]){
										echo "<option value=\"".$row[0]."\" SELECTED>".$row[0]."</option>";
									}else{
										echo "<option value=\"".$row[0]."\">".$row[0]."</option>";
									}
								}
							}else{
								// create selection 
								while ($row = mysql_fetch_array($result, MYSQL_NUM)) {
									echo "<option value=\"".$row[0]."\">".$row[0]."</option>";
								}
							}
						}
						</script>
					</select>
				</div>
				<p></p>
				<div>
					<script language="php">
						// This script searches and displays assets
						// Author: Rune Rasmussen
						// Date: 22/5/2011
						// include functions to access the h-three database
						include_once "h_three_database.php";
						// get asset names
						$assetNames = $_POST["asset_names"];
						// get predicate labels
						$predicateLabel = $_POST["predicate_names"];
						// if asset chosen
						if (isset($assetNames) && $assetNames != -1 ){
							// define a query
							$sql_query = "SELECT * FROM `predicate_labels`";
							// perform a query and get the database result
							$result = db_query($sql_query);
							// test success
							if ($result != FALSE){
								echo "Add Predicate : ";
								echo "<select name=\"predicate_names\" onchange=\"asset_state_form.submit();\">";
								echo "<option value=\"-1\">PREDICATES</option>";
								// create selection 
								while ($row = mysql_fetch_array($result, MYSQL_NUM)) {
									if (isset($predicateLabel) && $predicateLabel != -1 && $predicateLabel == $row[0]){
										echo "<option value=\"".$row[0]."\" SELECTED>".$row[0]."</option>";
									}else{
										echo "<option value=\"".$row[0]."\">".$row[0]."</option>";
									}
								}
								echo "</select>";
								echo "<input type=\"hidden\" name=\"last_predicate\" value=\"".$predicateLabel."\" />";
							}
							// if predicate get predicate variables
							if	(isset($predicateLabel) && $predicateLabel != -1) {
								// query predicate variables
								$sql_query = "SELECT * FROM `predicate_parameters` WHERE `predicate` = \"".$predicateLabel."\"";
								// perform a query and get the database result
								$result = db_query($sql_query);
								// test success
								if ($result != FALSE) {
									// set a counter
									$count = 0;
									// create selections for each variable
									while ($row = mysql_fetch_array($result, MYSQL_NUM)) {
										echo "<p>Domain for ".$row[1]."</p>";
										// get domain values for the variable
										$sql_query = "SELECT `value` FROM `domain_values` WHERE `name` = ";
										$sql_query = $sql_query." (SELECT `domain_name` FROM `variables` WHERE `identifier` = \"".$row[1]."\")";
										// perform a query and get the database result
										$result2 = db_query($sql_query);
										// test success
										if ($result2 != FALSE) {
											// create a selection
											echo "<select name=\"dom_values_".$count."\">";
											echo "<option value =\"-1\">VALUE</option>";
											// create domain value selector
											while ($row2 = mysql_fetch_array($result2, MYSQL_NUM)) {
												echo "<option value =\"".$row2[0]."\">".$row2[0]."</option>";
											}
											echo "</select>";
											echo "<input type=\"hidden\" name=\"var_name_".$count."\" value=\"".$row[1]."\" />";
											// count variable
											$count = $count + 1;
										}
									}
									echo "<input type=\"hidden\" name=\"num_vars\" value=\"".$count."\" />";
								}
							}
						}
					</script>
				</div>
				<p></p>
				<div align=center><input type="submit" value="OK" /></div>
				<p></p>
				<script language="php">
					// This script searches and displays assets
					// Author: Rune Rasmussen
					// Date: 22/5/2011
					// include functions to access the h-three database
					include_once "h_three_database.php";
					// get asset names
					$assetNames = $_POST["asset_names"];
					// get predicate labels
					$predicateLabel = $_POST["predicate_names"];
					// the predicate check variable
					$predicateCheck = $_POST["last_predicate"];
					// check values
					if	(isset($assetNames) && $assetNames != -1 && 
					isset($predicateLabel) && $predicateLabel != -1 &&
					isset($predicateCheck) && $predicateCheck == $predicateLabel){
						// then we can get a count
						$count_val = $_POST[num_vars];
						// the variable identifiers
						$var_ids = array();
						// the domain values
						$domain_vals = array();
						// if this exists
						if (isset($count_val)){
							// for each count
							for ($i = 0; $i < $count_val; $i++){
								$domain_vals[] = $_POST["dom_values_".$i];
								$var_ids[] = $_POST["var_name_".$i];
							}
							// create a test
							$test = TRUE;
							for ($i = 0; $i < $count_val; $i++){
								if (!isset($domain_vals[$i]) || $domain_vals[$i] == -1 || 
								!isset($var_ids[$i]) || $var_ids[$i] == -1){
									$test = FALSE;
								}
							}
							// if the test is good
							if ($test == TRUE){
								for ($i = 0; $i < $count_val; $i++){
									// replace into 
									$sql_query = "REPLACE INTO `asset_initial_state` (`asset_name`,";
									$sql_query = $sql_query."`predicate`, `variable_name`, `value`) VALUES ";
									$sql_query = $sql_query."(\"".$assetNames."\", \"".$predicateLabel."\",";
									$sql_query = $sql_query."\"".$var_ids[$i]."\", \"".$domain_vals[$i]."\")";
									// commit query
									// perform a query and get the database result
									$result = db_query($sql_query);
									// test success
									if ($result == FALSE) {
										echo "ERROR: Insert failed!";
									}
								}
							}
						}
					}
					
				</script>
				<p>The initial state of the asset is :
				<script language="php">
					// This script searches and displays assets
					// Author: Rune Rasmussen
					// Date: 22/5/2011
					// include functions to access the h-three database
					include_once "h_three_database.php";
					
					// get asset names
					$assetNames = $_POST["asset_names"];
					
					if (isset($assetNames) && $assetNames != -1) {
						$sql_query = "SELECT * FROM `asset_initial_state` WHERE `asset_name` = \"".$assetNames."\"";
						// perform a query and get the database result
						$result = db_query($sql_query);
						// test success
						if ($result != FALSE) {
							while ($row = mysql_fetch_array($result, MYSQL_NUM)) {
								echo $row[1]."(".$row[2]." = ".$row[3].") & ";
							}
						}
					}
					
				</script>
				TRUE </p>
			</form>
		</fieldset>
	</body>
</html>
