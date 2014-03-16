<?php
	include "./database.php";
	
	$path = "logs/".date("d-m-Y H-i-s").".xes";
	
	$result = db_query("SELECT * FROM log_event ORDER BY UserName, time");
	$row = mysql_fetch_array($result);
	$currentName = $row['userName'];
	
	$f = fopen($path, "w");
		
	//xes head
	fwrite($f, 
'<?xml version="1.0" encoding="UTF-8" ?>
<log xes.version="1.0" xes.features="nested-attributes" openxes.version="1.0RC7" xmlns="http://www.xes-standard.org/">
	<extension name="Lifecycle" prefix="lifecycle" uri="http://www.xes-standard.org/lifecycle.xesext"/>
	<extension name="Organizational" prefix="org" uri="http://www.xes-standard.org/org.xesext"/>
	<extension name="Time" prefix="time" uri="http://www.xes-standard.org/time.xesext"/>
	<extension name="Concept" prefix="concept" uri="http://www.xes-standard.org/concept.xesext"/>
	<extension name="Semantic" prefix="semantic" uri="http://www.xes-standard.org/semantic.xesext"/>
	<global scope="trace">
		<string key="concept:name" value="__INVALID__"/>
	</global>
	<global scope="event">
		<string key="concept:name" value="__INVALID__"/>
		<string key="lifecycle:transition" value="complete"/>
	</global>
	<classifier name="MXML Legacy Classifier" keys="concept:name lifecycle:transition"/>
	<classifier name="Event Name" keys="concept:name"/>
	<classifier name="Resource" keys="org:resource"/>
	<string key="source" value="Open Simulator"/>
	<string key="concept:name" value="OpenSimLog.mxml"/>
	<string key="lifecycle:model" value="standard"/>
');
	
	//start first trace
	fwrite($f,
"	<trace>
");
	
	//======== begin main loop ===========
	do 
	{
		$resource = $row['resource'];
		$time = $row['time'];
		$action = $row['action'];
		$transition = $row['transition'];
		
		if($transition == "endTrace"){ //if end transaction end trace and start new
		fwrite($f,
"	</trace>
	<trace>
");
	continue;
		}
		else if($row['userName'] != $currentName) //if new name end trace and start new
		{
			fwrite($f,
"	</trace>
	<trace>
");
		//update currentName
		$currentName = $row['userName'];
		}//end if
		
		$resource = $row['resource'];
		$time = $row['time'];
		$action = $row['action'];
		$transition = $row['transition'];
				
		fwrite($f, 
"		<event>
			<string key=\"org:resource\" value=\"$resource\"/>
			<date key=\"time:timestamp\" value=\"".substr($time, 0, 10)."T".substr($time, 11, 8).".000+00:00\"/>
			<string key=\"concept:name\" value=\"$action\"/>
			<string key=\"lifecycle:transition\" value=\"$transition\"/>
		</event>
");

	} 
	while($row = mysql_fetch_array($result));
	
	//finish xes
	fwrite($f, 
"	</trace>
</log>
");

	fclose($f); 
	mysql_close($con);
	
function output_file($file, $name, $mime_type='')
{
 /*
 This function takes a path to a file to output ($file),
 the filename that the browser will see ($name) and
 the MIME type of the file ($mime_type, optional).
 
 If you want to do something on download abort/finish,
 register_shutdown_function('function_name');
 */
 if(!is_readable($file)) die('File not found or inaccessible!');
 
 $size = filesize($file);
 $name = rawurldecode($name);
 
 /* Figure out the MIME type (if not specified) */
 $known_mime_types=array(
    "pdf" => "application/pdf",
    "txt" => "text/plain",
    "html" => "text/html",
    "htm" => "text/html",
    "exe" => "application/octet-stream",
    "zip" => "application/zip",
    "doc" => "application/msword",
    "xls" => "application/vnd.ms-excel",
    "ppt" => "application/vnd.ms-powerpoint",
    "gif" => "image/gif",
    "png" => "image/png",
    "jpeg"=> "image/jpg",
    "jpg" =>  "image/jpg",
    "php" => "text/plain"
 );
 
 if($mime_type==''){
     $file_extension = strtolower(substr(strrchr($file,"."),1));
     if(array_key_exists($file_extension, $known_mime_types)){
        $mime_type=$known_mime_types[$file_extension];
     } else {
        $mime_type="application/force-download";
     };
 };
 
 @ob_end_clean(); //turn off output buffering to decrease cpu usage
 
 // required for IE, otherwise Content-Disposition may be ignored
 if(ini_get('zlib.output_compression'))
  ini_set('zlib.output_compression', 'Off');
 
 header('Content-Type: ' . $mime_type);
 header('Content-Disposition: attachment; filename="'.$name.'"');
 header("Content-Transfer-Encoding: binary");
 header('Accept-Ranges: bytes');
 
 /* The three lines below basically make the
    download non-cacheable */
 header("Cache-control: private");
 header('Pragma: private');
 header("Expires: Mon, 26 Jul 1997 05:00:00 GMT");
 
 // multipart-download and download resuming support
 if(isset($_SERVER['HTTP_RANGE']))
 {
    list($a, $range) = explode("=",$_SERVER['HTTP_RANGE'],2);
    list($range) = explode(",",$range,2);
    list($range, $range_end) = explode("-", $range);
    $range=intval($range);
    if(!$range_end) {
        $range_end=$size-1;
    } else {
        $range_end=intval($range_end);
    }
 
    $new_length = $range_end-$range+1;
    header("HTTP/1.1 206 Partial Content");
    header("Content-Length: $new_length");
    header("Content-Range: bytes $range-$range_end/$size");
 } else {
    $new_length=$size;
    header("Content-Length: ".$size);
 }
 
 /* output the file itself */
 $chunksize = 1*(1024*1024); //you may want to change this
 $bytes_send = 0;
 if ($file = fopen($file, 'r'))
 {
    if(isset($_SERVER['HTTP_RANGE']))
    fseek($file, $range);
 
    while(!feof($file) &&
        (!connection_aborted()) &&
        ($bytes_send<$new_length)
          )
    {
        $buffer = fread($file, $chunksize);
        print($buffer); //echo($buffer); // is also possible
        flush();
        $bytes_send += strlen($buffer);
    }
 fclose($file);
 } else die('Error - can not open file.');
 
die();
}  
 
/*
Make sure script execution doesn't time out.
Set maximum execution time in seconds (0 means no limit).
*/
set_time_limit(0);
output_file($path, 'log.xes', 'text/xes');
?> 