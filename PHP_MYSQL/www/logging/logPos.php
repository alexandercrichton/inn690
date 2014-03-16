<?php
	include "./database.php";
	
	$userName = $_POST['userName'];
    $x = $_POST['x'];
	$y = $_POST['y'];
	$z = $_POST['z'];
		
	db_query(	"INSERT INTO log_position (userName, x, y, z)
				VALUES ('$userName','$x','$y', '$z')");
	
	echo "$x, $y, $z";
?>