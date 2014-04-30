-- phpMyAdmin SQL Dump
-- version 4.0.4
-- http://www.phpmyadmin.net
--
-- Host: localhost
-- Generation Time: Jan 25, 2014 at 05:32 PM
-- Server version: 5.6.12-log
-- PHP Version: 5.4.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `veis_world_states`
--

DROP DATABASE IF EXISTS `veis_world_states`;
CREATE DATABASE IF NOT EXISTS `veis_world_states` DEFAULT CHARACTER SET latin1 COLLATE latin1_swedish_ci;
USE `veis_world_states`;

DELIMITER $$
--
-- Procedures
--
CREATE DEFINER=`root`@`localhost` PROCEDURE `reset_world_state`()
    NO SQL
BEGIN
DELETE FROM world_states;

INSERT INTO veis_world_states.world_states(world_key, asset_name, predicate_label, value)
	SELECT 1, asset_name, predicate, value
    FROM veis_knowledge_base.asset_initial_state 
    ORDER BY asset_name;

SELECT count(*) 'Rows affected' FROM veis_world_states.world_states ;
END$$

DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `access_records`
--

CREATE TABLE IF NOT EXISTS `access_records` (
  `world_key` int(10) unsigned NOT NULL,
  `last_updated` datetime NOT NULL,
  `TIMESTAMP` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`world_key`),
  KEY `world_key` (`world_key`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `access_records`
--

INSERT INTO `access_records` (`world_key`, `last_updated`, `TIMESTAMP`) VALUES
(1, '2014-01-26 03:29:49', '2014-01-25 17:29:49');

-- --------------------------------------------------------

--
-- Table structure for table `asset_service_routines`
--

CREATE TABLE IF NOT EXISTS `asset_service_routines` (
  `key` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `priority` int(11) NOT NULL,
  `asset_key` varchar(128) NOT NULL,
  `service_routine` varchar(64) NOT NULL,
  `world_key` int(10) unsigned NOT NULL,
  `TIMESTAMP` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`key`),
  KEY `asset_service_routines_ibfk_1` (`world_key`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1 AUTO_INCREMENT=93 ;

--
-- Dumping data for table `asset_service_routines`
--

INSERT INTO `asset_service_routines` (`key`, `priority`, `asset_key`, `service_routine`, `world_key`, `TIMESTAMP`) VALUES
(25, 1, 'd967cdb9-9f47-4602-90c8-70e326c96919', 'Move Goods:Truck from=Entrance', 1, '2013-12-14 22:34:32'),
(89, 1, 'f5637d0b-8904-4741-a16b-553965423b92', 'Move:Bed to=ICUBed02', 1, '2014-01-25 17:10:40'),
(90, 0, 'f5637d0b-8904-4741-a16b-553965423b92', 'ABORT', 1, '2014-01-25 17:10:42'),
(91, 1, 'f5637d0b-8904-4741-a16b-553965423b92', 'Move:Bed to=ICUBed02', 1, '2014-01-25 17:15:05'),
(92, 0, 'f5637d0b-8904-4741-a16b-553965423b92', 'ABORT', 1, '2014-01-25 17:15:07');

--
-- Triggers `asset_service_routines`
--
DROP TRIGGER IF EXISTS `ins_asset_service_routines`;
DELIMITER //
CREATE TRIGGER `ins_asset_service_routines` AFTER INSERT ON `asset_service_routines`
 FOR EACH ROW UPDATE `access_records`
SET `last_updated` = NOW()
WHERE `world_key` = NEW.world_key
//
DELIMITER ;
DROP TRIGGER IF EXISTS `upd_asset_service_routines`;
DELIMITER //
CREATE TRIGGER `upd_asset_service_routines` AFTER UPDATE ON `asset_service_routines`
 FOR EACH ROW UPDATE `access_records`
SET `last_updated` = NOW()
WHERE `world_key` = NEW.world_key
//
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `player_states`
--

CREATE TABLE IF NOT EXISTS `player_states` (
  `user_key` varchar(128) NOT NULL,
  `state` varchar(10) NOT NULL,
  `TIMESTAMP` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`user_key`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `player_states`
--

INSERT INTO `player_states` (`user_key`, `state`, `TIMESTAMP`) VALUES
('f9209d10-3c2a-4678-b6c0-8d063e0e0238', '0', '2013-09-01 03:10:23');

-- --------------------------------------------------------

--
-- Table structure for table `play_activities`
--

CREATE TABLE IF NOT EXISTS `play_activities` (
  `kpn_key` int(10) unsigned NOT NULL,
  `user_key` varchar(128) NOT NULL,
  `TIMESTAMP` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Stand-in structure for view `possible_goals`
--
CREATE TABLE IF NOT EXISTS `possible_goals` (
`Goals` varchar(194)
);
-- --------------------------------------------------------

--
-- Table structure for table `recorded_activities`
--

CREATE TABLE IF NOT EXISTS `recorded_activities` (
  `kpn_key` int(10) unsigned NOT NULL,
  `name` varchar(128) NOT NULL,
  `user_key` varchar(128) NOT NULL,
  `order` int(10) unsigned NOT NULL,
  `TIMESTAMP` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`kpn_key`,`user_key`,`order`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `user_events`
--

CREATE TABLE IF NOT EXISTS `user_events` (
  `key` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `user_key` varchar(128) NOT NULL,
  `asset` varchar(64) NOT NULL,
  `asset_key` varchar(128) NOT NULL,
  `TIMESTAMP` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`key`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1 AUTO_INCREMENT=244 ;

--
-- Dumping data for table `user_events`
--

INSERT INTO `user_events` (`key`, `user_key`, `asset`, `asset_key`, `TIMESTAMP`) VALUES
(100, '9fa79ecf-40da-40d3-9b4c-cb8451efd90e', 'GROComputer', '903c4c41-d387-40a5-a464-fa7c7f920a4c', '2013-12-14 22:34:02'),
(101, '9fa79ecf-40da-40d3-9b4c-cb8451efd90e', 'GROComputer', '903c4c41-d387-40a5-a464-fa7c7f920a4c', '2013-12-14 22:34:03');

-- --------------------------------------------------------

--
-- Table structure for table `worlds`
--

CREATE TABLE IF NOT EXISTS `worlds` (
  `key` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `name` varchar(256) NOT NULL,
  `TIMESTAMP` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`key`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1 AUTO_INCREMENT=2 ;

--
-- Dumping data for table `worlds`
--

INSERT INTO `worlds` (`key`, `name`, `TIMESTAMP`) VALUES
(1, 'Horizon_Three_Demo', '2013-09-01 03:11:01');

-- --------------------------------------------------------

--
-- Table structure for table `world_states`
--

CREATE TABLE IF NOT EXISTS `world_states` (
  `world_key` int(10) unsigned NOT NULL,
  `asset_name` varchar(64) NOT NULL,
  `predicate_label` varchar(64) NOT NULL,
  `value` varchar(64) NOT NULL,
  `TIMESTAMP` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`world_key`,`predicate_label`,`asset_name`,`value`),
  KEY `world_key` (`world_key`),
  KEY `asset_name` (`asset_name`),
  KEY `value` (`value`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `world_states`
--

INSERT INTO `world_states` (`world_key`, `asset_name`, `predicate_label`, `value`, `TIMESTAMP`) VALUES
(1, 'VitalSignsMonitor', 'bandAt', 'PatientArm', '2014-01-25 17:29:49'),
(1, 'PatientBill', 'bedAt', 'EmergencyRoom', '2014-01-25 17:29:49'),
(1, 'CoffeeCup', 'cupAt', 'Table', '2014-01-25 17:29:49'),
(1, 'CoffeeCup', 'cupContains', 'Empty', '2014-01-25 17:29:49'),
(1, 'RequestPathology', 'forPatient', 'None', '2014-01-25 17:29:49'),
(1, 'BloodSampleVials', 'Has', 'None', '2014-01-25 17:29:49'),
(1, 'PatientBill', 'isHearingTested', 'False', '2014-01-25 17:29:49'),
(1, 'PatientBill', 'isVisuallyExamined', 'False', '2014-01-25 17:29:49'),
(1, 'VitalSignsMonitor', 'mouthpieceAt', 'PatientHead', '2014-01-25 17:29:49'),
(1, 'GROComputer', 'orderLogged', 'False', '2014-01-25 17:29:49'),
(1, 'GROComputer', 'orderProcessed', 'False', '2014-01-25 17:29:49'),
(1, 'ExamReport', 'reportTo', 'None', '2014-01-25 17:29:49'),
(1, 'RequestXRay', 'requestApproved', 'None', '2014-01-25 17:29:49'),
(1, 'RequestXRay', 'requestAt', 'Critical', '2014-01-25 17:29:49'),
(1, 'RequestXRay', 'requestForBodyPart', 'None', '2014-01-25 17:29:49'),
(1, 'VitalSignsMonitor', 'sensorAt', 'PatientArm', '2014-01-25 17:29:49'),
(1, 'Truck', 'truckAt', 'Entrance', '2014-01-25 17:29:49'),
(1, 'Truck', 'truckLoadStatus', 'Loaded', '2014-01-25 17:29:49'),
(1, 'MachineXRay', 'xrayAt', 'XRayMiddle', '2014-01-25 17:29:49'),
(1, 'MachineXRay', 'xrayForBodyPart', 'None', '2014-01-25 17:29:49'),
(1, 'ReportXRay', 'xrayReportTo', 'None', '2014-01-25 17:29:49');

--
-- Triggers `world_states`
--
DROP TRIGGER IF EXISTS `ins_world_state`;
DELIMITER //
CREATE TRIGGER `ins_world_state` AFTER INSERT ON `world_states`
 FOR EACH ROW UPDATE `access_records`
SET `last_updated` = NOW()
WHERE `world_key` = NEW.world_key
//
DELIMITER ;
DROP TRIGGER IF EXISTS `upd_world_state`;
DELIMITER //
CREATE TRIGGER `upd_world_state` AFTER UPDATE ON `world_states`
 FOR EACH ROW UPDATE `access_records`
SET `last_updated` = NOW()
WHERE `world_key` = NEW.world_key
//
DELIMITER ;

-- --------------------------------------------------------

--
-- Structure for view `possible_goals`
--
DROP TABLE IF EXISTS `possible_goals`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `possible_goals` AS select distinct concat(`a`.`asset_name`,':',`a`.`predicate_label`,';',`d`.`value`) AS `Goals` from (((`world_states` `a` join `veis_knowledge_base`.`method_post_conditions` `b`) join `veis_knowledge_base`.`variables` `c`) join `veis_knowledge_base`.`domain_values` `d`) where ((`a`.`predicate_label` = `b`.`predicate`) and (`b`.`state` = 1) and (`c`.`identifier` = `b`.`variable`) and (`d`.`name` = `c`.`domain_name`) and (`a`.`asset_name` <> '__FREE')) order by `a`.`asset_name` limit 0,100;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `access_records`
--
ALTER TABLE `access_records`
  ADD CONSTRAINT `access_records_ibfk_1` FOREIGN KEY (`world_key`) REFERENCES `worlds` (`key`) ON DELETE NO ACTION ON UPDATE NO ACTION;

--
-- Constraints for table `asset_service_routines`
--
ALTER TABLE `asset_service_routines`
  ADD CONSTRAINT `asset_service_routines_ibfk_1` FOREIGN KEY (`world_key`) REFERENCES `worlds` (`key`) ON DELETE NO ACTION ON UPDATE NO ACTION;

--
-- Constraints for table `world_states`
--
ALTER TABLE `world_states`
  ADD CONSTRAINT `world_states_ibfk_1` FOREIGN KEY (`world_key`) REFERENCES `worlds` (`key`) ON DELETE NO ACTION ON UPDATE NO ACTION;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
