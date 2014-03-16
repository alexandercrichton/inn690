<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
		<title>Stats</title>
		<link rel="stylesheet" type="text/css" href="./css/style.css" />
	</head>
	
	
	<?php
		include "./database.php";
		$arr = array();
	?>
	
	<?php
		//average time per process
		$result = db_query("SELECT * FROM log_event ORDER BY UserName, time");
		$row = mysql_fetch_array($result);
		$currentName = $row['userName'];
		$beginTime = strtotime($row['time']);
		$lastTime = $beginTime;
		$events = 1;
		$processes = 1;
		
		do
		{
			if($row['transition'] == "endTrace")
			{
				$processes += 1;
				continue;
			}
			else if($row['userName'] != $currentName)
			{
				$arr[$currentName]['timePerProcess'] = ($lastTime-$beginTime)/$processes;
				$arr[$currentName]['numProcesses'] = $processes;
				$arr[$currentName]['numEvents'] = $events;
				$arr[$currentName]['timeBetweenEvents'] = ($lastTime-$beginTime)/$events;
				$arr[$currentName]['totalTime'] = $lastTime - $beginTime;
				$currentName = $row['userName'];
				$beginTime = strtotime($row['time']);
				$processes = 1;
				$events = 1;
			}
			
			$events += 1;
			$lastTime = strtotime($row['time']);
		}
		while($row = mysql_fetch_array($result));
		
		$arr[$currentName]['timePerProcess'] = ($lastTime-$beginTime)/$processes;
		$arr[$currentName]['numProcesses'] = $processes;
		$arr[$currentName]['numEvents'] = $events;
		$arr[$currentName]['timeBetweenEvents'] = ($lastTime-$beginTime)/$events;
		$arr[$currentName]['totalTime'] = $lastTime - $beginTime;

		//average time between events
		/*$result = db_query("SELECT * FROM log_event ORDER BY UserName, time");
		$row = mysql_fetch_array($result);
		$currentName = $row['userName'];
		$beginTime = strtotime($row['time']);
		$lastTime = $beginTime;
		$count = 1;
		
		do
		{
			if($row['transition'] == "endTrace")
			{
				continue;
			}
			else if($row['userName'] != $currentName)
			{
				$arr[$currentName]['timeBetweenEvents'] = ($lastTime-$beginTime)/$count;
				$arr[$currentName]['numEvents'] = $count;
				$currentName = $row['userName'];
				$beginTime = strtotime($row['time']);
				$count = 1;
			}
			
			$lastTime = strtotime($row['time']);
			$count += 1;
		}
		while($row = mysql_fetch_array($result));
		
		$arr[$currentName]['timeBetweenEvents'] = ($lastTime-$beginTime)/$count;
		$arr[$currentName]['numEvents'] = $count;
		*/
		
		//total distance travelled
		$result = db_query("SELECT * FROM log_position ORDER BY UserName, time");
		$row = mysql_fetch_array($result);
		$currentName = $row['userName'];
		$distance = 0;
		$lastX = intval($row['x']);
		$lastY = intval($row['y']);
		$lastZ = intval($row['z']);
		$row = mysql_fetch_array($result);
		
		do
		{
			if($row['userName'] != $currentName)
			{
				$arr[$currentName]['totalDistance'] = $distance;
				$currentName = $row['userName'];
				$distance = 0;
				$lastX = intval($row['x']);
				$lastY = intval($row['y']);
				$lastZ = intval($row['z']);
				continue;
			}
			
			$distance += sqrt( pow(intval($row['x'])-$lastX, 2) 
							+ pow(intval($row['y'])-$lastY, 2) 
							+ pow(intval($row['z'])-$lastZ, 2));
			$lastX = intval($row['x']);
			$lastY = intval($row['y']);
			$lastZ = intval($row['z']);
		}
		while($row = mysql_fetch_array($result));
		
		$arr[$currentName]['totalDistance'] = $distance;

		//average distance per process
		foreach($arr as $key => $value)
		{
			if(!isset($value['numProcesses']) || !isset($value['totalDistance']) || $value['numProcesses'] == 0)
				$arr[$key]['distancePerProcess'] = 0;
			else
				$arr[$key]['distancePerProcess'] = $value['totalDistance'] / $value['numProcesses'];
		}
		
		
	?>
	
	<body>
		<div class="wrapper">
			<div id="header">
			<h1>Statistics</h1>
			</div>
			<div id="container">
			<table border="1">
				<thead>
					<tr>
						<th>User Name</th>
						<th>Num Events</th>
						<th>Total Time</th>
						<th>Num Processes</th>
						<th>Average Time per Process</th>
						<th>Average Time per Event</th>
						<th>Distance Travelled</th>
						<th>Average Distance per Process</th>
					</tr>
				</thead>
				<tbody>
			
				<?php
				$i = 0;
					foreach($arr as $key => $value)
					{
						if($i%2 == 0)
							echo "<tr class='light'>";
						else
							echo "<tr class='dark'>";
						$i += 1;
							
						echo sprintf("<td>%s</td>", $key);
						
						if(isset($value['numEvents']))
							echo sprintf("<td>%d</td>", $value['numEvents']);
						else
							echo sprintf("<td>%d</td>", 0);
							
						if(isset($value['totalTime']))
							echo sprintf("<td>%d</td>", $value['totalTime']);
						else
							echo sprintf("<td>%d</td>", 0);
							
						if(isset($value['numProcesses']))
							echo sprintf("<td>%d</td>", $value['numProcesses']);
						else
							echo sprintf("<td>%d</td>", 0);
							
						if(isset($value['timePerProcess']))
							echo sprintf("<td>%f</td>", $value['timePerProcess']);
						else
							echo sprintf("<td>%d</td>", 0);
							
						if(isset($value['timeBetweenEvents']))
							echo sprintf("<td>%f</td>", $value['timeBetweenEvents']);
						else
							echo sprintf("<td>%d</td>", 0);
							
						if(isset($value['totalDistance']))
							echo sprintf("<td>%d</td>", $value['totalDistance']);
						else
							echo sprintf("<td>%d</td>", 0);
							
						if(isset($value['distancePerProcess']))
							echo sprintf("<td>%f</td>", $value['distancePerProcess']);
						else
							echo sprintf("<td>%d</td>", 0);
							
						echo "</tr>";
					}
				?>
				</tbody>
			</table>
			</div>
		</div>
	</body>
</html>