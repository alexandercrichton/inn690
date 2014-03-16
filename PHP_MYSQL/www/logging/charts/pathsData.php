<?php

include 'php-ofc-library/open-flash-chart.php';
include "../database.php";

function random_color(){
    mt_srand((double)microtime()*1000000);
    $c = '#';
    while(strlen($c)<7){
        $c .= sprintf("%02X", mt_rand(0, 255));
    }
    return $c;
}

$chart = new open_flash_chart();

$minX = 0;
$minY = 0;
$maxX = 0;
$maxY = 0;

$title = new title("" );
$chart->set_title( $title );
$chart->set_bg_colour('#ffffff');

$s = new scatter_line( random_color(), 3 );
$def = new hollow_dot();
$def->size(3)->halo_size(2);
$s->set_default_dot_style( $def );
$v = array();

$result = db_query("SELECT * FROM log_position ORDER BY userName, time");

$row = mysql_fetch_array($result);
$prevUserName = $row['userName'];
$x = floatval($row['x']);
$y = floatval($row['y']);
	
$minX = $x; 
$maxX = $x;
$minY = $y;
$maxY = $y;

/*$x = floatval($row['x']);
$y = floatval($row['y']);

$min = $x;
$max = $x;
if($y < $min)
	$min = $y;
if($y > $max)
	$max = $y;*/

do
{
	$userName = $row['userName'];
	if($userName != $prevUserName)
	{
		$s->set_values( $v );
		$chart->add_element( $s );
		$s = new scatter_line( random_color(), 3 );
		$def = new hollow_dot();
		$def->size(3)->halo_size(2);
		$s->set_default_dot_style( $def );
		$v = array();
		$prevUserName = $userName;
	}
	$x = floatval($row['x']);
	$y = floatval($row['y']);
	
	if($x < $minX)
		$minX = $x;
	else if($x > $maxX)
		$maxX = $x;
	
	if($y < $minY)
		$minY = $y;
	else if($y > $maxY)
		$maxY = $y;
	/*if($x < $min)
		$min = $x;
	if($x > $max)
		$max = $x;
	if($y < $min)
		$min = $y;
	if($y > $max)
		$max = $y;*/
	
	$v[] = new scatter_value($x, $y);
	
}while($row = mysql_fetch_array($result));

if(($maxX - $minX) > ($maxY - $minY))
	$maxY = $minY + ($maxX - $minX);
else
	$maxX = $minX + ($maxY - $minY);

$s->set_values( $v );
$chart->add_element( $s );

$x = new x_axis();
$x->set_range( intval($minX), intval($maxX)+1);
$chart->set_x_axis( $x );

$y = new x_axis();
$y->set_range( intval($minY), intval($maxY)+1, 10 );
$chart->add_y_axis( $y );


echo $chart->toPrettyString();