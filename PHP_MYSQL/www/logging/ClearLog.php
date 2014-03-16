<?php
	include "./database.php";

	db_query("TRUNCATE TABLE log_event;");
	db_query("TRUNCATE TABLE log_event_extra;");
	db_query("TRUNCATE TABLE log_position;");
	

?> 

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title>Log data cleared</title>
</head>

<body>
	<p>Log data cleared.</p>
    <a href="index.html">Back</a>
</body>
</html>