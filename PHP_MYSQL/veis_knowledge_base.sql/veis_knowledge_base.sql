-- phpMyAdmin SQL Dump
-- version 4.0.4
-- http://www.phpmyadmin.net
--
-- Host: localhost
-- Generation Time: Jan 25, 2014 at 05:30 PM
-- Server version: 5.6.12-log
-- PHP Version: 5.4.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `veis_knowledge_base`
--
CREATE DATABASE IF NOT EXISTS `veis_knowledge_base` DEFAULT CHARACTER SET latin1 COLLATE latin1_swedish_ci;
USE `veis_knowledge_base`;

-- --------------------------------------------------------

--
-- Table structure for table `activity_methods`
--

CREATE TABLE IF NOT EXISTS `activity_methods` (
  `name` varchar(64) NOT NULL,
  `TIMESTAMP` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `activity_methods`
--

INSERT INTO `activity_methods` (`name`, `TIMESTAMP`) VALUES
('Add Blood', '2013-12-04 04:24:38'),
('Add Report', '2013-12-04 04:24:38'),
('Add XRay Report', '2013-12-04 04:24:38'),
('Approve XRay Request', '2013-12-04 04:24:38'),
('Cancel Request', '2013-12-04 04:24:38'),
('Examine Visual', '2013-12-05 04:36:00'),
('Execute XRay', '2013-12-04 04:28:37'),
('Fill Coffee', '2013-12-04 04:24:38'),
('Log Order', '2013-12-04 04:25:56'),
('Make Request', '2013-12-04 04:24:38'),
('Make XRay Request', '2013-12-04 04:24:38'),
('Move Bed', '2013-12-05 05:20:50'),
('Move Blood Pressure Band', '2013-12-04 04:24:38'),
('Move Cup', '2013-12-05 04:37:03'),
('Move Forklift', '2013-12-05 04:36:26'),
('Move Mouthpiece', '2013-12-04 04:24:38'),
('Move Pulse Sensor', '2013-12-04 04:24:38'),
('Move Request', '2013-09-13 03:21:46'),
('Move Truck', '2013-12-04 04:26:05'),
('Move XRay Machine', '2013-12-04 04:24:38'),
('Process Order', '2013-12-04 04:26:21'),
('Reset Order', '2013-12-04 04:26:27'),
('Test Hearing', '2013-12-05 04:35:04'),
('Unload or Load goods', '2013-12-05 04:36:14');

-- --------------------------------------------------------

--
-- Table structure for table `asset`
--

CREATE TABLE IF NOT EXISTS `asset` (
  `name` varchar(64) NOT NULL,
  `TIMESTAMP` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `asset`
--

INSERT INTO `asset` (`name`, `TIMESTAMP`) VALUES
('BloodSampleVials', '2013-10-11 00:43:23'),
('CoffeeCup', '2013-12-05 04:54:05'),
('ExamReport', '2014-01-25 14:56:33'),
('GROComputer', '2013-10-11 00:44:03'),
('MachineXRay', '2014-01-25 17:07:05'),
('PatientBill', '2013-10-11 00:43:16'),
('ReportXRay', '2014-01-25 17:09:44'),
('RequestPathology', '2013-10-11 00:43:47'),
('RequestXRay', '2013-10-11 00:43:41'),
('Truck', '2013-09-01 03:07:17'),
('VitalSignsMonitor', '2013-10-11 00:43:53');

-- --------------------------------------------------------

--
-- Table structure for table `asset_initial_state`
--

CREATE TABLE IF NOT EXISTS `asset_initial_state` (
  `asset_name` varchar(64) NOT NULL,
  `predicate` varchar(64) NOT NULL,
  `variable_name` varchar(64) NOT NULL,
  `value` varchar(64) NOT NULL,
  `TIMESTAMP` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`asset_name`,`predicate`,`variable_name`),
  KEY `asset_name` (`asset_name`),
  KEY `predicate_label` (`predicate`),
  KEY `predicate_value` (`variable_name`,`value`),
  KEY `value` (`value`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `asset_initial_state`
--

INSERT INTO `asset_initial_state` (`asset_name`, `predicate`, `variable_name`, `value`, `TIMESTAMP`) VALUES
('BloodSampleVials', 'Has', 'Vial contains', 'None', '2013-09-12 16:37:36'),
('CoffeeCup', 'cupAt', 'Cup from', 'Table', '2013-11-20 04:24:35'),
('CoffeeCup', 'cupContains', 'Coffee Cup Is', 'Empty', '2013-11-20 04:38:35'),
('ExamReport', 'reportTo', 'Reportee from', 'None', '2013-09-13 02:59:42'),
('GROComputer', 'orderLogged', 'Order logged', 'False', '2013-09-01 03:07:29'),
('GROComputer', 'orderProcessed', 'Order processed', 'False', '2013-09-01 03:07:29'),
('MachineXRay', 'xrayAt', 'XRay from', 'XRayMiddle', '2013-09-20 03:15:22'),
('MachineXRay', 'xrayForBodyPart', 'Body part rof', 'None', '2013-10-02 08:12:49'),
('PatientBill', 'bedAt', 'Bed from', 'EmergencyRoom', '2013-10-02 07:38:32'),
('PatientBill', 'isHearingTested', 'Patient current state', 'False', '2013-09-06 01:57:42'),
('PatientBill', 'isVisuallyExamined', 'Patient current state', 'False', '2013-09-06 01:57:42'),
('ReportXRay', 'xrayReportTo', 'Reportee from', 'None', '2013-10-02 08:25:42'),
('RequestPathology', 'forPatient', 'Make rof', 'None', '2013-10-02 05:23:39'),
('RequestXRay', 'requestApproved', 'Body part rof', 'None', '2013-10-02 06:50:04'),
('RequestXRay', 'requestAt', 'Request from', 'Critical', '2013-09-20 00:42:57'),
('RequestXRay', 'requestForBodyPart', 'Body part rof', 'None', '2013-10-02 06:15:38'),
('Truck', 'truckAt', 'Truck from', 'Entrance', '2013-12-04 04:20:04'),
('Truck', 'truckLoadStatus', 'Truck load status', 'Loaded', '2013-09-01 03:07:29'),
('VitalSignsMonitor', 'bandAt', 'VSM from', 'PatientArm', '2013-09-13 02:51:26'),
('VitalSignsMonitor', 'mouthpieceAt', 'VSM from', 'PatientHead', '2013-09-13 02:16:31'),
('VitalSignsMonitor', 'sensorAt', 'VSM from', 'PatientArm', '2013-09-13 02:52:59');

-- --------------------------------------------------------

--
-- Table structure for table `asset_methods`
--

CREATE TABLE IF NOT EXISTS `asset_methods` (
  `asset_name` varchar(64) NOT NULL,
  `method_name` varchar(64) NOT NULL,
  `TIMESTAMP` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`asset_name`,`method_name`),
  KEY `asset_name` (`asset_name`),
  KEY `method_name` (`method_name`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `asset_methods`
--

INSERT INTO `asset_methods` (`asset_name`, `method_name`, `TIMESTAMP`) VALUES
('BloodSampleVials', 'Add Blood', '2013-09-12 16:34:49'),
('CoffeeCup', 'Fill Coffee', '2013-11-20 04:33:05'),
('CoffeeCup', 'Move Cup', '2013-12-05 06:25:42'),
('ExamReport', 'Add Report', '2013-09-12 16:29:50'),
('GROComputer', 'Log Order', '2013-09-01 03:07:39'),
('GROComputer', 'Process Order', '2013-09-01 03:07:39'),
('GROComputer', 'Reset Order', '2013-09-01 03:07:39'),
('MachineXRay', 'Execute XRay', '2013-10-02 08:07:33'),
('MachineXRay', 'Move XRay Machine', '2013-09-20 03:14:16'),
('PatientBill', 'Examine Visual', '2013-09-06 01:23:08'),
('PatientBill', 'Move Bed', '2013-09-01 03:07:39'),
('PatientBill', 'Test Hearing', '2013-09-13 02:09:48'),
('ReportXRay', 'Add XRay Report', '2013-10-02 08:32:04'),
('RequestPathology', 'Make Request', '2013-10-02 03:58:54'),
('RequestXRay', 'Approve XRay Request', '2013-10-02 06:35:50'),
('RequestXRay', 'Make XRay Request', '2013-10-02 05:55:00'),
('RequestXRay', 'Move Request', '2013-09-13 03:22:01'),
('Truck', 'Move Truck', '2013-09-01 03:07:39'),
('Truck', 'Unload or Load goods', '2013-09-01 03:07:39'),
('VitalSignsMonitor', 'Move Blood Pressure Band', '2013-09-06 02:30:52'),
('VitalSignsMonitor', 'Move Mouthpiece', '2013-09-06 02:30:52'),
('VitalSignsMonitor', 'Move Pulse Sensor', '2013-09-06 02:31:01');

-- --------------------------------------------------------

--
-- Table structure for table `domains`
--

CREATE TABLE IF NOT EXISTS `domains` (
  `name` varchar(64) NOT NULL,
  `TIMESTAMP` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `domains`
--

INSERT INTO `domains` (`name`, `TIMESTAMP`) VALUES
('Actor', '2013-09-12 16:28:34'),
('bed_locations', '2013-09-01 03:07:48'),
('Boolean', '2013-09-01 03:07:48'),
('CoffeeCupState', '2013-11-20 04:33:18'),
('CoffeePosition', '2013-11-20 04:19:47'),
('loading_status', '2013-09-01 03:07:48'),
('Patient', '2013-09-12 16:39:02'),
('Patient_list', '2013-10-02 04:01:20'),
('Patient_part', '2013-10-02 05:41:17'),
('patient_state', '2013-09-12 16:19:10'),
('Request_locations', '2013-09-13 04:05:35'),
('Sample', '2013-09-12 16:33:26'),
('VSM_locations', '2013-09-12 16:22:25'),
('warehouse_locations', '2013-09-01 03:07:48'),
('XRay_locations', '2013-09-12 16:50:37');

-- --------------------------------------------------------

--
-- Table structure for table `domain_values`
--

CREATE TABLE IF NOT EXISTS `domain_values` (
  `name` varchar(64) NOT NULL,
  `value` varchar(64) NOT NULL,
  `TIMESTAMP` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`name`,`value`),
  KEY `name` (`name`),
  KEY `value` (`value`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `domain_values`
--

INSERT INTO `domain_values` (`name`, `value`, `TIMESTAMP`) VALUES
('Actor', 'Doctor', '2013-09-12 16:28:53'),
('Actor', 'None', '2013-10-02 07:07:10'),
('Actor', 'Nurse', '2013-09-12 16:28:53'),
('Actor', 'XRayTechnician', '2013-10-11 00:47:30'),
('bed_locations', 'EmergencyRoom', '2013-10-11 00:41:01'),
('bed_locations', 'ExaminationRoom', '2013-10-11 00:35:39'),
('bed_locations', 'ICUBed02', '2013-12-03 08:12:45'),
('bed_locations', 'IntensiveCare', '2013-09-13 02:07:26'),
('bed_locations', 'Machine', '2013-12-05 05:25:59'),
('bed_locations', 'RadiologyRoom', '2013-10-11 00:47:30'),
('Boolean', 'False', '2013-09-06 01:54:27'),
('Boolean', 'True', '2013-09-06 01:54:21'),
('CoffeeCupState', 'Coffee', '2013-11-20 04:33:43'),
('CoffeeCupState', 'Empty', '2013-11-20 04:33:43'),
('CoffeePosition', 'ExaminationRoom', '2013-12-05 05:41:19'),
('CoffeePosition', 'Machine', '2013-11-20 04:20:07'),
('CoffeePosition', 'Table', '2013-11-20 04:20:07'),
('loading_status', 'Loaded', '2013-09-01 03:07:57'),
('loading_status', 'Unloaded', '2013-09-01 03:07:57'),
('Patient', 'PatientBill', '2013-10-11 00:47:30'),
('Patient', 'PatientChest', '2013-10-11 00:47:30'),
('Patient', 'PatientHead', '2013-10-11 00:47:30'),
('Patient_list', 'BillGate', '2013-10-11 00:47:30'),
('Patient_list', 'None', '2013-10-02 04:07:22'),
('Patient_list', 'SteveJob', '2013-10-11 00:47:30'),
('Patient_part', 'Head', '2013-10-02 05:42:23'),
('Patient_part', 'None', '2013-10-02 05:42:12'),
('Patient_part', 'Torso', '2013-10-02 05:42:12'),
('patient_state', 'False', '2013-09-06 01:19:11'),
('patient_state', 'True', '2013-09-06 01:19:11'),
('Request_locations', 'Critical', '2013-09-20 00:34:00'),
('Request_locations', 'Radiology', '2013-09-20 00:34:00'),
('Sample', 'None', '2013-09-12 16:33:48'),
('Sample', 'PatientArm', '2013-10-11 00:47:30'),
('VSM_locations', 'PatientArm', '2013-10-11 00:47:30'),
('VSM_locations', 'PatientChest', '2013-10-11 00:47:30'),
('VSM_locations', 'PatientHead', '2013-10-11 00:47:59'),
('VSM_locations', 'VitalSignsMonitor', '2013-10-11 00:47:59'),
('warehouse_locations', 'Bay05', '2013-10-11 00:47:59'),
('warehouse_locations', 'Entrance', '2013-09-01 03:07:57'),
('warehouse_locations', 'Exit', '2013-09-01 03:07:57'),
('warehouse_locations', 'GRO', '2013-09-01 03:07:57'),
('warehouse_locations', 'Hidden', '2013-09-01 03:07:57'),
('XRay_locations', 'XRayMiddle', '2013-10-11 00:47:59'),
('XRay_locations', 'XRayTop', '2013-10-11 00:47:59');

-- --------------------------------------------------------

--
-- Table structure for table `kpn_templates`
--

CREATE TABLE IF NOT EXISTS `kpn_templates` (
  `key` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `pre` text NOT NULL,
  `a` text NOT NULL,
  `neg_b` text NOT NULL,
  `TIMESTAMP` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`key`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1 AUTO_INCREMENT=11 ;

--
-- Dumping data for table `kpn_templates`
--

INSERT INTO `kpn_templates` (`key`, `pre`, `a`, `neg_b`, `TIMESTAMP`) VALUES
(1, 'at,Bay 5,Bed 5;_unoccupied,Bay 4;', 'at,Bay 4,Bed 5;_unoccupied,Bay 5;', 'at,Bay 5,Bed 5;_unoccupied,Bay 4;', '2013-09-01 03:08:07'),
(2, 'at,Bay 6,Bed 6;_unoccupied,Bay 5;', 'at,Bay 5,Bed 6;_unoccupied,Bay 6;', 'at,Bay 6,Bed 6;_unoccupied,Bay 5;', '2013-09-01 03:08:07'),
(3, 'at,Bay 6,Bed 6;at,Bay 5,Bed 5;_unoccupied,Bay 4;', 'at,Bay 4,Bed 5;at,Bay 5,Bed 6;_unoccupied,Bay 6;', 'at,Bay 5,Bed 5;_unoccupied,Bay 4;at,Bay 6,Bed 6;_unoccupied,Bay 5;', '2013-09-01 03:08:07'),
(4, 'at,Bay 4,Bed 4;_unoccupied,Bay 3;', 'at,Bay 3,Bed 4;_unoccupied,Bay 4;', 'at,Bay 4,Bed 4;_unoccupied,Bay 3;', '2013-09-01 03:08:07'),
(5, 'at,Bay 1,Bed 2;_unoccupied,Bay 4;', 'at,Bay 4,Bed 2;_unoccupied,Bay 1;', 'at,Bay 1,Bed 2;_unoccupied,Bay 4;', '2013-09-01 03:08:07'),
(6, 'at,Bay 5,Bed 5;_unoccupied,Bay 1;', 'at,Bay 1,Bed 5;_unoccupied,Bay 5;', 'at,Bay 5,Bed 5;_unoccupied,Bay 1;', '2013-09-01 03:08:07'),
(7, 'at,Bay 6,Bed 6;_unoccupied,Bay 5;', 'at,Bay 5,Bed 6;_unoccupied,Bay 6;', 'at,Bay 6,Bed 6;_unoccupied,Bay 5;', '2013-09-01 03:08:07'),
(8, 'at,Bay 1,Bed 2;at,Bay 4,Bed 4;_unoccupied,Bay 3;', 'at,Bay 3,Bed 4;at,Bay 4,Bed 2;_unoccupied,Bay 1;', 'at,Bay 4,Bed 4;_unoccupied,Bay 3;at,Bay 1,Bed 2;_unoccupied,Bay 4;', '2013-09-01 03:08:07'),
(9, 'at,Bay 6,Bed 6;at,Bay 5,Bed 5;_unoccupied,Bay 1;', 'at,Bay 1,Bed 5;at,Bay 5,Bed 6;_unoccupied,Bay 6;', 'at,Bay 5,Bed 5;_unoccupied,Bay 1;at,Bay 6,Bed 6;_unoccupied,Bay 5;', '2013-09-01 03:08:07'),
(10, 'at,Bay 6,Bed 6;at,Bay 5,Bed 5;at,Bay 1,Bed 2;at,Bay 4,Bed 4;_unoccupied,Bay 3;', 'at,Bay 3,Bed 4;at,Bay 4,Bed 2;at,Bay 1,Bed 5;at,Bay 5,Bed 6;_unoccupied,Bay 6;', 'at,Bay 4,Bed 4;_unoccupied,Bay 3;at,Bay 1,Bed 2;_unoccupied,Bay 4;at,Bay 5,Bed 5;_unoccupied,Bay 1;at,Bay 6,Bed 6;_unoccupied,Bay 5;', '2013-09-01 03:08:07');

-- --------------------------------------------------------

--
-- Table structure for table `method_parameters`
--

CREATE TABLE IF NOT EXISTS `method_parameters` (
  `method_name` varchar(64) NOT NULL,
  `variable` varchar(64) NOT NULL,
  `TIMESTAMP` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`method_name`,`variable`),
  KEY `identifier` (`variable`),
  KEY `name` (`method_name`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `method_parameters`
--

INSERT INTO `method_parameters` (`method_name`, `variable`, `TIMESTAMP`) VALUES
('Add Blood', 'Blood from', '2013-09-12 16:35:16'),
('Add Blood', 'Vial contains', '2013-09-12 16:35:16'),
('Add Report', 'Reportee from', '2013-09-12 16:30:17'),
('Add Report', 'Reportee to', '2013-10-02 03:42:47'),
('Add XRay Report', 'Reportee from', '2013-10-02 08:32:35'),
('Add XRay Report', 'Reportee to', '2013-10-02 08:32:35'),
('Approve XRay Request', 'Body part for', '2013-10-02 06:36:19'),
('Approve XRay Request', 'Body part rof', '2013-10-02 06:36:19'),
('Examine Visual', 'Patient current state', '2013-09-06 01:20:49'),
('Examine Visual', 'Patient executed', '2013-09-06 01:20:49'),
('Execute XRay', 'Body part for', '2013-10-02 08:08:11'),
('Execute XRay', 'Body part rof', '2013-10-02 08:08:11'),
('Fill Coffee', 'Coffee Cup Is', '2013-11-20 04:36:41'),
('Fill Coffee', 'Coffee Cup Is Fill', '2013-11-20 04:36:41'),
('Log Order', 'Order log action', '2013-09-01 03:08:17'),
('Log Order', 'Order logged', '2013-09-01 03:08:17'),
('Make Request', 'Make for', '2013-10-02 04:08:45'),
('Make Request', 'Make rof', '2013-10-02 04:08:45'),
('Make XRay Request', 'Body part for', '2013-10-02 05:58:58'),
('Make XRay Request', 'Body part rof', '2013-10-02 05:59:06'),
('Move Bed', 'Bed from', '2013-09-01 03:08:17'),
('Move Bed', 'Bed to', '2013-09-01 03:08:17'),
('Move Blood Pressure Band', 'VSM from', '2013-09-06 02:32:13'),
('Move Blood Pressure Band', 'VSM to', '2013-09-06 02:32:13'),
('Move Cup', 'Cup from', '2013-11-20 04:21:08'),
('Move Cup', 'Cup to', '2013-11-20 04:21:08'),
('Move Mouthpiece', 'VSM from', '2013-09-06 02:22:22'),
('Move Mouthpiece', 'VSM to', '2013-09-06 02:22:22'),
('Move Pulse Sensor', 'VSM from', '2013-09-06 02:35:53'),
('Move Pulse Sensor', 'VSM to', '2013-09-06 02:35:53'),
('Move Request', 'Request from', '2013-09-20 00:37:56'),
('Move Request', 'Request to', '2013-09-20 00:38:12'),
('Move Truck', 'Truck from', '2013-09-01 03:08:17'),
('Move Truck', 'Truck to', '2013-09-01 03:08:17'),
('Move XRay Machine', 'XRay from', '2013-09-20 03:10:44'),
('Move XRay Machine', 'XRay to', '2013-09-20 03:10:44'),
('Process Order', 'Order process action', '2013-09-01 03:08:17'),
('Process Order', 'Order processed', '2013-09-01 03:08:17'),
('Reset Order', 'Order logged', '2013-09-01 03:08:17'),
('Reset Order', 'Order processed', '2013-09-01 03:08:17'),
('Reset Order', 'Order reset action', '2013-09-01 03:08:17'),
('Test Hearing', 'Patient current state', '2013-09-06 01:48:29'),
('Test Hearing', 'Patient executed', '2013-09-06 01:48:29'),
('Unload or Load goods', 'Desired load state', '2013-09-01 03:08:17'),
('Unload or Load goods', 'Truck from', '2013-09-01 03:08:17'),
('Unload or Load goods', 'Truck load status', '2013-09-01 03:08:17');

-- --------------------------------------------------------

--
-- Table structure for table `method_post_conditions`
--

CREATE TABLE IF NOT EXISTS `method_post_conditions` (
  `method_name` varchar(64) NOT NULL,
  `predicate` varchar(64) NOT NULL,
  `variable` varchar(64) NOT NULL,
  `state` tinyint(1) NOT NULL,
  `TIMESTAMP` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`method_name`,`predicate`,`variable`),
  KEY `predicate` (`predicate`),
  KEY `variable` (`variable`),
  KEY `method_name` (`method_name`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `method_post_conditions`
--

INSERT INTO `method_post_conditions` (`method_name`, `predicate`, `variable`, `state`, `TIMESTAMP`) VALUES
('Add Blood', 'Has', 'Blood from', 1, '2013-09-12 16:37:01'),
('Add Blood', 'Has', 'Vial contains', 0, '2013-09-12 16:37:01'),
('Add Report', 'reportTo', 'Reportee from', 0, '2013-10-02 03:51:38'),
('Add Report', 'reportTo', 'Reportee to', 1, '2013-10-02 03:51:50'),
('Add XRay Report', 'xrayReportTo', 'Reportee from', 0, '2013-10-02 08:33:53'),
('Add XRay Report', 'xrayReportTo', 'Reportee to', 1, '2013-10-02 08:33:53'),
('Approve XRay Request', 'requestApproved', 'Body part for', 1, '2013-10-02 06:49:37'),
('Approve XRay Request', 'requestApproved', 'Body part rof', 0, '2013-10-02 06:49:37'),
('Examine Visual', 'isVisuallyExamined', 'Patient current state', 0, '2013-09-06 01:47:56'),
('Examine Visual', 'isVisuallyExamined', 'Patient executed', 1, '2013-09-06 01:47:56'),
('Execute XRay', 'xrayForBodyPart', 'Body part for', 1, '2013-10-02 08:12:28'),
('Execute XRay', 'xrayForBodyPart', 'Body part rof', 0, '2013-10-02 08:12:28'),
('Fill Coffee', 'cupContains', 'Coffee Cup Is', 0, '2013-11-20 04:38:12'),
('Fill Coffee', 'cupContains', 'Coffee Cup Is Fill', 1, '2013-11-20 04:38:12'),
('Log Order', 'orderLogged', 'Order log action', 1, '2013-09-01 03:08:25'),
('Log Order', 'orderLogged', 'Order logged', 0, '2013-09-01 03:08:25'),
('Make Request', 'forPatient', 'Make for', 1, '2013-10-02 06:14:54'),
('Make Request', 'forPatient', 'Make rof', 0, '2013-10-02 06:14:54'),
('Make XRay Request', 'requestForBodyPart', 'Body part for', 1, '2013-10-02 06:03:39'),
('Make XRay Request', 'requestForBodyPart', 'Body part rof', 0, '2013-10-02 06:03:39'),
('Move Bed', 'bedAt', 'Bed from', 0, '2013-09-01 03:08:25'),
('Move Bed', 'bedAt', 'Bed to', 1, '2013-09-01 03:08:25'),
('Move Bed', '_unoccupied', 'Bed from', 1, '2013-09-01 03:08:25'),
('Move Bed', '_unoccupied', 'Bed to', 0, '2013-09-01 03:08:25'),
('Move Blood Pressure Band', 'bandAt', 'VSM from', 0, '2013-09-06 02:35:07'),
('Move Blood Pressure Band', 'bandAt', 'VSM to', 1, '2013-09-06 02:35:07'),
('Move Cup', 'cupAt', 'Cup from', 0, '2013-11-20 04:23:56'),
('Move Cup', 'cupAt', 'Cup to', 1, '2013-11-20 04:23:56'),
('Move Cup', '_unoccupied', 'Cup from', 1, '2013-12-05 04:04:58'),
('Move Cup', '_unoccupied', 'Cup to', 0, '2013-12-05 04:04:58'),
('Move Mouthpiece', 'mouthpieceAt', 'VSM from', 0, '2013-09-06 02:24:26'),
('Move Mouthpiece', 'mouthpieceAt', 'VSM to', 1, '2013-09-06 02:24:26'),
('Move Pulse Sensor', 'sensorAt', 'VSM from', 0, '2013-09-12 16:27:39'),
('Move Pulse Sensor', 'sensorAt', 'VSM to', 1, '2013-09-12 16:27:39'),
('Move Request', 'requestAt', 'Request from', 0, '2013-09-20 03:32:07'),
('Move Request', 'requestAt', 'Request to', 1, '2013-09-20 03:32:07'),
('Move Truck', 'truckAt', 'Truck from', 0, '2013-10-03 03:05:43'),
('Move Truck', 'truckAt', 'Truck to', 1, '2013-10-03 03:05:43'),
('Move Truck', '_unoccupied', 'Truck from', 1, '2013-09-01 03:08:25'),
('Move Truck', '_unoccupied', 'Truck to', 0, '2013-09-01 03:08:25'),
('Move XRay Machine', 'xrayAt', 'XRay from', 0, '2013-09-20 03:29:35'),
('Move XRay Machine', 'xrayAt', 'XRay to', 1, '2013-09-20 03:29:35'),
('Process Order', 'orderProcessed', 'Order process action', 1, '2013-09-01 03:08:25'),
('Process Order', 'orderProcessed', 'Order processed', 0, '2013-09-01 03:08:25'),
('Reset Order', 'orderLogged', 'Order logged', 0, '2013-09-01 03:08:25'),
('Reset Order', 'orderLogged', 'Order reset action', 1, '2013-09-01 03:08:25'),
('Reset Order', 'orderProcessed', 'Order processed', 0, '2013-09-01 03:08:25'),
('Reset Order', 'orderProcessed', 'Order reset action', 1, '2013-09-01 03:08:25'),
('Test Hearing', 'isHearingTested', 'Patient current state', 0, '2013-09-06 01:50:33'),
('Test Hearing', 'isHearingTested', 'Patient executed', 1, '2013-09-06 01:50:33'),
('Unload or Load goods', 'truckLoadStatus', 'Desired load state', 1, '2013-12-14 22:26:10'),
('Unload or Load goods', 'truckLoadStatus', 'Truck load status', 0, '2013-12-14 22:26:10');

-- --------------------------------------------------------

--
-- Table structure for table `method_preconditions`
--

CREATE TABLE IF NOT EXISTS `method_preconditions` (
  `method_name` varchar(64) NOT NULL,
  `predicate` varchar(64) NOT NULL,
  `variable` varchar(64) NOT NULL,
  `TIMESTAMP` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`method_name`,`predicate`,`variable`),
  KEY `predicate` (`predicate`),
  KEY `variable` (`variable`),
  KEY `method_name` (`method_name`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `method_preconditions`
--

INSERT INTO `method_preconditions` (`method_name`, `predicate`, `variable`, `TIMESTAMP`) VALUES
('Add Blood', 'Has', 'Vial contains', '2013-09-12 16:36:17'),
('Add Report', 'reportTo', 'Reportee from', '2013-10-02 03:46:17'),
('Add XRay Report', 'xrayReportTo', 'Reportee from', '2013-10-02 08:33:13'),
('Approve XRay Request', 'requestApproved', 'Body part rof', '2013-10-02 06:49:11'),
('Examine Visual', 'isVisuallyExamined', 'Patient current state', '2013-09-06 01:47:32'),
('Execute XRay', 'xrayForBodyPart', 'Body part rof', '2013-10-02 08:11:55'),
('Fill Coffee', 'cupContains', 'Coffee Cup Is', '2013-11-20 04:37:35'),
('Log Order', 'orderLogged', 'Order logged', '2013-09-01 03:08:35'),
('Make Request', 'forPatient', 'Make rof', '2013-10-02 06:14:04'),
('Make XRay Request', 'requestForBodyPart', 'Body part rof', '2013-10-02 06:03:08'),
('Move Bed', 'bedAt', 'Bed from', '2013-09-01 06:23:57'),
('Move Blood Pressure Band', 'bandAt', 'VSM from', '2013-09-06 02:34:38'),
('Move Cup', 'cupAt', 'Cup from', '2013-11-20 04:22:39'),
('Move Mouthpiece', 'mouthpieceAt', 'VSM from', '2013-09-06 02:23:43'),
('Move Pulse Sensor', 'sensorAt', 'VSM from', '2013-09-12 16:26:30'),
('Move Request', 'requestAt', 'Request from', '2013-09-20 03:31:01'),
('Move Truck', 'truckAt', 'Truck from', '2013-10-02 07:41:43'),
('Move XRay Machine', 'xrayAt', 'XRay from', '2013-09-20 03:25:58'),
('Process Order', 'orderProcessed', 'Order processed', '2013-09-01 03:08:35'),
('Reset Order', 'orderLogged', 'Order logged', '2013-09-01 03:08:35'),
('Reset Order', 'orderProcessed', 'Order processed', '2013-09-01 03:08:35'),
('Test Hearing', 'isHearingTested', 'Patient current state', '2013-09-06 01:49:48'),
('Unload or Load goods', 'truckAt', 'Truck from', '2013-10-02 07:44:22'),
('Unload or Load goods', 'truckLoadStatus', 'Truck load status', '2013-09-01 03:08:35');

-- --------------------------------------------------------

--
-- Table structure for table `method_service_calls`
--

CREATE TABLE IF NOT EXISTS `method_service_calls` (
  `method_name` varchar(64) NOT NULL,
  `service_call` varchar(64) NOT NULL,
  `variable` varchar(64) NOT NULL,
  `value` varchar(64) NOT NULL,
  `TIMESTAMP` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`method_name`,`service_call`,`variable`,`value`),
  KEY `variable` (`variable`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `method_service_calls`
--

INSERT INTO `method_service_calls` (`method_name`, `service_call`, `variable`, `value`, `TIMESTAMP`) VALUES
('Move Bed', 'Move', 'Bed to', '', '2013-09-01 03:08:47'),
('Move Cup', 'Move', 'Cup to', '', '2013-12-05 06:38:00'),
('Move Forklift', 'Move', 'Forklift to', '', '2013-09-01 03:08:47'),
('Move Truck', 'Move', 'Truck to', '', '2013-09-01 03:08:47'),
('Unload or Load goods', 'Move Goods', 'Truck from', '', '2013-09-01 03:08:47');

-- --------------------------------------------------------

--
-- Table structure for table `predicate_labels`
--

CREATE TABLE IF NOT EXISTS `predicate_labels` (
  `predicate` varchar(64) NOT NULL,
  `TIMESTAMP` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`predicate`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `predicate_labels`
--

INSERT INTO `predicate_labels` (`predicate`, `TIMESTAMP`) VALUES
('bandAt', '2013-10-11 00:21:02'),
('bedAt', '2013-12-05 05:22:02'),
('cupAt', '2013-12-05 05:27:31'),
('cupContains', '2013-12-05 05:27:39'),
('forPatient', '2013-10-11 00:21:12'),
('Has', '2014-01-25 16:25:31'),
('isHearingTested', '2013-09-06 01:48:57'),
('isVisuallyExamined', '2013-09-06 01:21:49'),
('mouthpieceAt', '2013-10-11 00:23:26'),
('orderLogged', '2013-10-11 00:23:26'),
('orderProcessed', '2013-10-11 00:23:26'),
('reportTo', '2013-10-11 00:23:26'),
('requestApproved', '2013-10-11 00:23:26'),
('requestAt', '2013-10-11 00:23:26'),
('requestForBodyPart', '2013-10-11 00:23:26'),
('sensorAt', '2013-10-11 00:23:26'),
('truckAt', '2013-10-11 00:23:26'),
('truckLoadStatus', '2013-12-04 04:20:49'),
('xrayAt', '2013-10-11 00:23:26'),
('xrayForBodyPart', '2013-10-11 00:23:26'),
('xrayReportTo', '2013-10-11 00:23:26'),
('_unoccupied', '2013-09-01 03:09:12');

-- --------------------------------------------------------

--
-- Table structure for table `predicate_parameters`
--

CREATE TABLE IF NOT EXISTS `predicate_parameters` (
  `predicate` varchar(64) NOT NULL,
  `variable` varchar(64) NOT NULL,
  `TIMESTAMP` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`predicate`,`variable`),
  KEY `predicate` (`predicate`),
  KEY `identifier` (`variable`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `predicate_parameters`
--

INSERT INTO `predicate_parameters` (`predicate`, `variable`, `TIMESTAMP`) VALUES
('bandAt', 'VSM from', '2013-09-06 02:33:47'),
('bandAt', 'VSM to', '2013-09-06 02:33:47'),
('bedAt', 'Bed from', '2013-10-02 07:36:26'),
('bedAt', 'Bed to', '2013-10-02 07:36:26'),
('cupAt', 'Cup from', '2013-11-20 04:22:03'),
('cupAt', 'Cup to', '2013-11-20 04:22:03'),
('cupContains', 'Coffee Cup Is', '2013-11-20 04:37:15'),
('cupContains', 'Coffee Cup Is Fill', '2013-11-20 04:37:15'),
('forPatient', 'Make for', '2013-10-02 04:09:53'),
('forPatient', 'Make rof', '2013-10-02 04:09:53'),
('Has', 'Blood from', '2013-09-12 16:35:46'),
('Has', 'Vial contains', '2013-09-12 16:35:46'),
('isHearingTested', 'Patient current state', '2013-09-06 01:49:27'),
('isHearingTested', 'Patient executed', '2013-09-06 01:49:27'),
('isVisuallyExamined', 'Patient current state', '2013-09-06 01:47:11'),
('isVisuallyExamined', 'Patient executed', '2013-09-06 01:47:11'),
('mouthpieceAt', 'VSM from', '2013-09-06 02:22:57'),
('mouthpieceAt', 'VSM to', '2013-09-06 02:22:57'),
('orderLogged', 'Order log action', '2013-09-01 03:09:22'),
('orderLogged', 'Order logged', '2013-09-01 03:09:22'),
('orderLogged', 'Order reset action', '2013-09-01 03:09:22'),
('orderProcessed', 'Order process action', '2013-09-01 03:09:22'),
('orderProcessed', 'Order processed', '2013-09-01 03:09:22'),
('orderProcessed', 'Order reset action', '2013-09-01 03:09:22'),
('reportTo', 'Reportee from', '2013-09-12 16:32:43'),
('reportTo', 'Reportee to', '2013-10-02 03:44:14'),
('requestApproved', 'Body part for', '2013-10-02 06:48:05'),
('requestApproved', 'Body part rof', '2013-10-02 06:48:05'),
('requestAt', 'Request from', '2013-09-20 00:36:39'),
('requestAt', 'Request to', '2013-09-20 00:36:39'),
('requestForBodyPart', 'Body part for', '2013-10-02 06:12:01'),
('requestForBodyPart', 'Body part rof', '2013-10-02 06:12:01'),
('sensorAt', 'VSM from', '2013-09-06 02:36:57'),
('sensorAt', 'VSM to', '2013-09-06 02:36:57'),
('truckAt', 'Truck from', '2013-10-03 02:54:37'),
('truckAt', 'Truck to', '2013-10-03 02:54:37'),
('truckAt', 'VSM from', '2013-10-02 07:41:26'),
('truckAt', 'VSM to', '2013-10-02 07:41:26'),
('truckLoadStatus', 'Desired load state', '2013-09-01 03:09:22'),
('truckLoadStatus', 'Truck load status', '2013-09-01 03:09:22'),
('xrayAt', 'XRay from', '2013-09-20 03:12:07'),
('xrayAt', 'XRay to', '2013-09-20 03:12:07'),
('xrayForBodyPart', 'Body part for', '2013-10-02 08:11:33'),
('xrayForBodyPart', 'Body part rof', '2013-10-02 08:11:33'),
('xrayReportTo', 'Reportee from', '2013-10-02 08:24:04'),
('xrayReportTo', 'Reportee to', '2013-10-02 08:24:04'),
('_unoccupied', 'Bed from', '2013-09-01 03:09:22'),
('_unoccupied', 'Bed to', '2013-09-01 03:09:22'),
('_unoccupied', 'Forklift from', '2013-09-01 03:09:22'),
('_unoccupied', 'Forklift to', '2013-09-01 03:09:22'),
('_unoccupied', 'Truck from', '2013-09-01 03:09:22'),
('_unoccupied', 'Truck to', '2013-09-01 03:09:22');

-- --------------------------------------------------------

--
-- Table structure for table `template_children`
--

CREATE TABLE IF NOT EXISTS `template_children` (
  `parent` int(10) unsigned NOT NULL,
  `child` int(10) unsigned NOT NULL,
  `TIMESTAMP` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`parent`,`child`),
  KEY `parent` (`parent`),
  KEY `child` (`child`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `template_children`
--

INSERT INTO `template_children` (`parent`, `child`, `TIMESTAMP`) VALUES
(3, 1, '2013-09-01 03:09:30'),
(3, 2, '2013-09-01 03:09:30'),
(8, 4, '2013-09-01 03:09:30'),
(8, 5, '2013-09-01 03:09:30'),
(9, 6, '2013-09-01 03:09:30'),
(9, 7, '2013-09-01 03:09:30'),
(10, 8, '2013-09-01 03:09:30'),
(10, 9, '2013-09-01 03:09:30');

-- --------------------------------------------------------

--
-- Table structure for table `template_labels`
--

CREATE TABLE IF NOT EXISTS `template_labels` (
  `key` int(10) unsigned NOT NULL,
  `name` varchar(64) NOT NULL,
  `method_name` varchar(64) NOT NULL,
  `asset` varchar(64) NOT NULL,
  `asset_key` varchar(128) NOT NULL,
  `actor` varchar(64) NOT NULL,
  `actor_key` varchar(128) NOT NULL,
  `param_val_list` varchar(256) NOT NULL,
  `TIMESTAMP` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`key`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `variables`
--

CREATE TABLE IF NOT EXISTS `variables` (
  `identifier` varchar(64) NOT NULL,
  `domain_name` varchar(64) NOT NULL,
  `TIMESTAMP` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`identifier`),
  KEY `name` (`domain_name`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `variables`
--

INSERT INTO `variables` (`identifier`, `domain_name`, `TIMESTAMP`) VALUES
('Bed from', 'bed_locations', '2013-09-01 03:09:53'),
('Bed to', 'bed_locations', '2013-12-05 05:23:30'),
('Blood from', 'Sample', '2013-12-04 04:34:13'),
('Body part for', 'Patient_part', '2013-10-02 05:44:57'),
('Body part rof', 'Patient_part', '2013-10-02 05:44:57'),
('Coffee Cup Is', 'CoffeeCupState', '2013-12-04 04:34:13'),
('Coffee Cup Is Fill', 'CoffeeCupState', '2013-12-04 04:34:14'),
('Cup from', 'CoffeePosition', '2013-12-04 04:34:14'),
('Cup to', 'CoffeePosition', '2013-12-04 04:34:14'),
('Desired load state', 'loading_status', '2013-09-01 03:09:53'),
('Forklift from', 'warehouse_locations', '2013-09-01 03:09:53'),
('Forklift to', 'warehouse_locations', '2013-09-01 03:09:53'),
('Goods at', 'warehouse_locations', '2013-09-01 03:09:53'),
('Make for', 'Patient_list', '2013-10-02 04:08:04'),
('Make rof', 'Patient_list', '2013-10-02 04:08:04'),
('Order log action', 'Boolean', '2013-09-01 03:09:53'),
('Order logged', 'Boolean', '2013-09-01 03:09:53'),
('Order process action', 'Boolean', '2013-09-01 03:09:53'),
('Order processed', 'Boolean', '2013-09-01 03:09:53'),
('Order reset action', 'Boolean', '2013-09-01 03:09:53'),
('Patient current state', 'patient_state', '2013-12-04 04:41:34'),
('Patient executed', 'patient_state', '2013-12-04 04:42:13'),
('Patient part', 'Patient', '2013-12-04 04:42:31'),
('Reportee from', 'Actor', '2013-10-02 03:42:01'),
('Reportee to', 'Actor', '2013-10-02 03:42:11'),
('Request from', 'Request_locations', '2013-09-20 00:35:56'),
('Request to', 'Request_locations', '2013-09-20 00:35:56'),
('state', 'Boolean', '2013-09-01 03:09:53'),
('Truck from', 'warehouse_locations', '2013-09-01 03:09:53'),
('Truck load status', 'loading_status', '2013-09-01 03:09:53'),
('Truck to', 'warehouse_locations', '2013-09-01 03:09:53'),
('Vial contains', 'Sample', '2013-12-04 04:43:18'),
('VSM from', 'VSM_locations', '2013-12-04 04:41:10'),
('VSM to', 'VSM_locations', '2013-12-04 04:42:22'),
('XRay from', 'XRay_locations', '2013-09-20 03:09:36'),
('XRay to', 'XRay_locations', '2013-09-20 03:09:52');

--
-- Constraints for dumped tables
--

--
-- Constraints for table `asset_initial_state`
--
ALTER TABLE `asset_initial_state`
  ADD CONSTRAINT `asset_initial_state_ibfk_1` FOREIGN KEY (`asset_name`) REFERENCES `asset` (`name`) ON DELETE NO ACTION ON UPDATE CASCADE,
  ADD CONSTRAINT `asset_initial_state_ibfk_2` FOREIGN KEY (`predicate`) REFERENCES `predicate_labels` (`predicate`) ON DELETE NO ACTION ON UPDATE CASCADE,
  ADD CONSTRAINT `asset_initial_state_ibfk_4` FOREIGN KEY (`value`) REFERENCES `domain_values` (`value`) ON DELETE NO ACTION ON UPDATE CASCADE,
  ADD CONSTRAINT `asset_initial_state_ibfk_5` FOREIGN KEY (`variable_name`) REFERENCES `variables` (`identifier`) ON DELETE NO ACTION ON UPDATE CASCADE;

--
-- Constraints for table `asset_methods`
--
ALTER TABLE `asset_methods`
  ADD CONSTRAINT `asset_name` FOREIGN KEY (`asset_name`) REFERENCES `asset` (`name`) ON DELETE NO ACTION ON UPDATE CASCADE,
  ADD CONSTRAINT `method_name` FOREIGN KEY (`method_name`) REFERENCES `activity_methods` (`name`) ON DELETE NO ACTION ON UPDATE CASCADE;

--
-- Constraints for table `domain_values`
--
ALTER TABLE `domain_values`
  ADD CONSTRAINT `name` FOREIGN KEY (`name`) REFERENCES `domains` (`name`) ON DELETE NO ACTION ON UPDATE CASCADE;

--
-- Constraints for table `method_parameters`
--
ALTER TABLE `method_parameters`
  ADD CONSTRAINT `method_parameters_ibfk_1` FOREIGN KEY (`method_name`) REFERENCES `activity_methods` (`name`) ON DELETE NO ACTION ON UPDATE CASCADE,
  ADD CONSTRAINT `method_parameters_ibfk_2` FOREIGN KEY (`variable`) REFERENCES `variables` (`identifier`) ON DELETE NO ACTION ON UPDATE CASCADE;

--
-- Constraints for table `method_post_conditions`
--
ALTER TABLE `method_post_conditions`
  ADD CONSTRAINT `method_post_conditions_ibfk_1` FOREIGN KEY (`method_name`) REFERENCES `activity_methods` (`name`) ON DELETE NO ACTION ON UPDATE CASCADE,
  ADD CONSTRAINT `method_post_conditions_ibfk_2` FOREIGN KEY (`predicate`) REFERENCES `predicate_parameters` (`predicate`) ON DELETE NO ACTION ON UPDATE CASCADE,
  ADD CONSTRAINT `method_post_conditions_ibfk_3` FOREIGN KEY (`variable`) REFERENCES `predicate_parameters` (`variable`) ON UPDATE CASCADE;

--
-- Constraints for table `method_preconditions`
--
ALTER TABLE `method_preconditions`
  ADD CONSTRAINT `method_preconditions_ibfk_1` FOREIGN KEY (`method_name`) REFERENCES `activity_methods` (`name`) ON DELETE NO ACTION ON UPDATE CASCADE,
  ADD CONSTRAINT `method_preconditions_ibfk_2` FOREIGN KEY (`predicate`) REFERENCES `predicate_parameters` (`predicate`) ON DELETE NO ACTION ON UPDATE CASCADE,
  ADD CONSTRAINT `method_preconditions_ibfk_3` FOREIGN KEY (`variable`) REFERENCES `predicate_parameters` (`variable`) ON UPDATE CASCADE;

--
-- Constraints for table `method_service_calls`
--
ALTER TABLE `method_service_calls`
  ADD CONSTRAINT `method_service_calls_ibfk_1` FOREIGN KEY (`method_name`) REFERENCES `activity_methods` (`name`) ON DELETE NO ACTION ON UPDATE CASCADE,
  ADD CONSTRAINT `method_service_calls_ibfk_2` FOREIGN KEY (`variable`) REFERENCES `variables` (`identifier`) ON DELETE NO ACTION ON UPDATE CASCADE;

--
-- Constraints for table `predicate_parameters`
--
ALTER TABLE `predicate_parameters`
  ADD CONSTRAINT `predicate_parameters_ibfk_1` FOREIGN KEY (`predicate`) REFERENCES `predicate_labels` (`predicate`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `predicate_parameters_ibfk_2` FOREIGN KEY (`variable`) REFERENCES `variables` (`identifier`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `template_children`
--
ALTER TABLE `template_children`
  ADD CONSTRAINT `child` FOREIGN KEY (`child`) REFERENCES `kpn_templates` (`key`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `parent` FOREIGN KEY (`parent`) REFERENCES `kpn_templates` (`key`) ON DELETE NO ACTION ON UPDATE NO ACTION;

--
-- Constraints for table `template_labels`
--
ALTER TABLE `template_labels`
  ADD CONSTRAINT `template_labels_ibfk_1` FOREIGN KEY (`key`) REFERENCES `kpn_templates` (`key`) ON DELETE NO ACTION ON UPDATE NO ACTION;

--
-- Constraints for table `variables`
--
ALTER TABLE `variables`
  ADD CONSTRAINT `variables_ibfk_1` FOREIGN KEY (`domain_name`) REFERENCES `domains` (`name`) ON DELETE NO ACTION ON UPDATE CASCADE;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
