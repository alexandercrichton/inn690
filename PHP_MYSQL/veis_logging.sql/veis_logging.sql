-- phpMyAdmin SQL Dump
-- version 4.0.4
-- http://www.phpmyadmin.net
--
-- Host: localhost
-- Generation Time: Jan 25, 2014 at 05:33 PM
-- Server version: 5.6.12-log
-- PHP Version: 5.4.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `veis_logging`
--
CREATE DATABASE IF NOT EXISTS `veis_logging` DEFAULT CHARACTER SET latin1 COLLATE latin1_swedish_ci;
USE `veis_logging`;

-- --------------------------------------------------------

--
-- Table structure for table `log_event`
--

CREATE TABLE IF NOT EXISTS `log_event` (
  `recordNum` int(11) NOT NULL AUTO_INCREMENT,
  `userName` varchar(50) NOT NULL,
  `resource` varchar(50) NOT NULL,
  `action` varchar(50) NOT NULL,
  `transition` varchar(50) NOT NULL,
  `time` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`recordNum`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1 AUTO_INCREMENT=54 ;

--
-- Dumping data for table `log_event`
--

INSERT INTO `log_event` (`recordNum`, `userName`, `resource`, `action`, `transition`, `time`) VALUES
(1, 'Test User', 'PatientBill', 'Move Bed', 'complete', '2014-01-25 14:49:18'),
(2, 'Test User', 'VitalSignsMonitor', 'Move Mouthpiece', 'complete', '2014-01-25 14:50:06'),
(3, 'Test User', 'VitalSignsMonitor', 'Move Blood Pressure Band', 'complete', '2014-01-25 14:50:33'),
(4, 'Test User', 'VitalSignsMonitor', 'Move Pulse Sensor', 'complete', '2014-01-25 14:50:49'),
(5, 'Test User', 'ExaminationReport', 'Add Report', 'complete', '2014-01-25 14:51:09'),
(6, 'Test User', 'PatientBill', 'Examine Visual', 'complete', '2014-01-25 14:52:01'),
(7, 'Test User', 'PatientBill', 'Test Hearing', 'complete', '2014-01-25 14:52:13'),
(8, 'Test User', 'ExaminationReport', 'Add Report', 'complete', '2014-01-25 14:53:16'),
(9, 'Test User', 'PatientBill', 'Move Bed', 'complete', '2014-01-25 16:16:33'),
(10, 'Test User', 'VitalSignsMonitor', 'Move Mouthpiece', 'complete', '2014-01-25 16:17:47'),
(11, 'Test User', 'PatientBill', 'Examine Visual', 'complete', '2014-01-25 16:18:08'),
(12, 'Test User', 'VitalSignsMonitor', 'Move Pulse Sensor', 'complete', '2014-01-25 16:18:25'),
(13, 'Test User', 'VitalSignsMonitor', 'Move Blood Pressure Band', 'complete', '2014-01-25 16:18:42'),
(14, 'Test User', 'PatientBill', 'Test Hearing', 'complete', '2014-01-25 16:19:27'),
(15, 'Test User', 'PatientBill', 'Test Hearing', 'complete', '2014-01-25 16:20:35'),
(16, 'Test User', 'ExamReport', 'Add Report', 'complete', '2014-01-25 16:22:47'),
(17, 'Test User', 'PatientBill', 'Move Bed', 'complete', '2014-01-25 16:23:05'),
(18, 'Test User', 'BloodSampleVials', 'Add Blood', 'complete', '2014-01-25 16:23:36'),
(19, 'Test User', 'BloodSampleVials', 'Add Blood', 'complete', '2014-01-25 16:26:05'),
(20, 'Test User', 'RequestPathology', 'Make Request', 'complete', '2014-01-25 16:26:40'),
(21, 'Test User', 'VitalSignsMonitor', 'Move Mouthpiece', 'complete', '2014-01-25 16:27:01'),
(22, 'Test User', 'RequestXRay', 'Make XRay Request', 'complete', '2014-01-25 16:27:44'),
(23, 'Test User', 'RequestXRay', 'Approve XRay Request', 'complete', '2014-01-25 16:28:03'),
(24, 'Test User', 'RequestXRay', 'Move Request', 'complete', '2014-01-25 16:28:21'),
(25, 'Test User', 'PatientBill', 'Move Bed', 'complete', '2014-01-25 16:29:43'),
(26, 'Test User', 'PatientBill', 'Move Bed', 'complete', '2014-01-25 16:44:03'),
(27, 'Test User', 'PatientBill', 'Move Bed', 'complete', '2014-01-25 16:52:40'),
(28, 'Test User', 'PatientBill', 'Move Bed', 'complete', '2014-01-25 16:52:59'),
(29, 'Test User', 'PatientBill', 'Move Bed', 'complete', '2014-01-25 16:58:30'),
(30, 'Test User', 'VitalSignsMonitor', 'Move Mouthpiece', 'complete', '2014-01-25 16:59:02'),
(31, 'Test User', 'VitalSignsMonitor', 'Move Mouthpiece', 'complete', '2014-01-25 16:59:58'),
(32, 'Test User', 'PatientBill', 'Examine Visual', 'complete', '2014-01-25 17:00:13'),
(33, 'Test User', 'VitalSignsMonitor', 'Move Pulse Sensor', 'complete', '2014-01-25 17:00:33'),
(34, 'Test User', 'PatientBill', 'Test Hearing', 'complete', '2014-01-25 17:00:47'),
(35, 'Test User', 'VitalSignsMonitor', 'Move Blood Pressure Band', 'complete', '2014-01-25 17:01:04'),
(36, 'Test User', 'ExamReport', 'Add Report', 'complete', '2014-01-25 17:01:19'),
(37, 'Test User', 'PatientBill', 'Move Bed', 'complete', '2014-01-25 17:01:33'),
(38, 'Test User', 'RequestPathology', 'Make Request', 'complete', '2014-01-25 17:02:01'),
(39, 'Test User', 'RequestPathology', 'Make Request', 'complete', '2014-01-25 17:02:31'),
(40, 'Test User', 'BloodSampleVials', 'Add Blood', 'complete', '2014-01-25 17:02:51'),
(41, 'Test User', 'BloodSampleVials', 'Add Blood', 'complete', '2014-01-25 17:03:21'),
(42, 'Test User', 'VitalSignsMonitor', 'Move Mouthpiece', 'complete', '2014-01-25 17:03:37'),
(43, 'Test User', 'RequestXRay', 'Make XRay Request', 'complete', '2014-01-25 17:04:07'),
(44, 'Test User', 'RequestXRay', 'Approve XRay Request', 'complete', '2014-01-25 17:04:25'),
(45, 'Test User', 'RequestXRay', 'Approve XRay Request', 'complete', '2014-01-25 17:04:58'),
(46, 'Test User', 'RequestXRay', 'Move Request', 'complete', '2014-01-25 17:05:14'),
(47, 'Test User', 'PatientBill', 'Move Bed', 'complete', '2014-01-25 17:05:45'),
(48, 'Test User', 'MachineXRay', 'Execute XRay', 'complete', '2014-01-25 17:08:38'),
(49, 'Test User', 'MachineXRay', 'Execute XRay', 'complete', '2014-01-25 17:09:01'),
(50, 'Test User', 'ReportXRay', 'Add XRay Report', 'complete', '2014-01-25 17:09:55'),
(51, 'Test User', 'ReportXRay', 'Add XRay Report', 'complete', '2014-01-25 17:10:20'),
(52, 'Test User', 'PatientBill', 'Move Bed', 'complete', '2014-01-25 17:10:40'),
(53, 'Test User', 'PatientBill', 'Move Bed', 'complete', '2014-01-25 17:15:05');

-- --------------------------------------------------------

--
-- Table structure for table `log_event_extra`
--

CREATE TABLE IF NOT EXISTS `log_event_extra` (
  `recordNum` int(11) NOT NULL,
  `key` varchar(50) NOT NULL,
  `value` varchar(200) NOT NULL,
  PRIMARY KEY (`recordNum`,`key`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `log_event_extra`
--

INSERT INTO `log_event_extra` (`recordNum`, `key`, `value`) VALUES
(1, 'Bed from', 'EmergencyRoom'),
(1, 'Bed to', 'ExaminationRoom'),
(2, 'VSM to', 'VitalSignsMonitor'),
(3, 'VSM to', 'VitalSignsMonitor'),
(4, 'VSM to', 'VitalSignsMonitor'),
(5, 'Reportee to', 'Nurse'),
(6, 'Patient executed', 'True'),
(7, 'Patient executed', 'True'),
(8, 'Reportee to', 'Nurse'),
(9, 'Bed from', 'EmergencyRoom'),
(9, 'Bed to', 'ExaminationRoom'),
(10, 'VSM to', 'VitalSignsMonitor'),
(11, 'Patient executed', 'True'),
(12, 'VSM to', 'VitalSignsMonitor'),
(13, 'VSM to', 'VitalSignsMonitor'),
(14, 'Patient executed', 'True'),
(15, 'Patient executed', 'True'),
(16, 'Reportee to', 'Nurse'),
(17, 'Bed from', 'ExaminationRoom'),
(17, 'Bed to', 'IntensiveCare'),
(18, 'Blood from', 'PatientArm'),
(19, 'Blood from', 'PatientArm'),
(20, 'Make for', 'BillGate'),
(21, 'VSM to', 'PatientHead'),
(22, 'Body part for', 'Head'),
(23, 'Body part for', 'Head'),
(24, 'Request to', 'Radiology'),
(25, 'Bed from', 'IntensiveCare'),
(25, 'Bed to', 'RadiologyRoom'),
(26, 'Bed from', 'RadiologyRoom'),
(26, 'Bed to', 'ExaminationRoom'),
(27, 'Bed from', 'EmergencyRoom'),
(27, 'Bed to', 'ExaminationRoom'),
(28, 'Bed from', 'ExaminationRoom'),
(28, 'Bed to', 'EmergencyRoom'),
(29, 'Bed from', 'EmergencyRoom'),
(29, 'Bed to', 'ExaminationRoom'),
(30, 'VSM to', 'VitalSignsMonitor'),
(31, 'VSM to', 'VitalSignsMonitor'),
(32, 'Patient executed', 'True'),
(33, 'VSM to', 'VitalSignsMonitor'),
(34, 'Patient executed', 'True'),
(35, 'VSM to', 'VitalSignsMonitor'),
(36, 'Reportee to', 'Nurse'),
(37, 'Bed from', 'ExaminationRoom'),
(37, 'Bed to', 'IntensiveCare'),
(38, 'Make for', 'BillGate'),
(39, 'Make for', 'BillGate'),
(40, 'Blood from', 'PatientArm'),
(41, 'Blood from', 'PatientArm'),
(42, 'VSM to', 'PatientHead'),
(43, 'Body part for', 'Head'),
(44, 'Body part for', 'Head'),
(45, 'Body part for', 'Head'),
(46, 'Request to', 'Radiology'),
(47, 'Bed from', 'IntensiveCare'),
(47, 'Bed to', 'RadiologyRoom'),
(48, 'Body part for', 'Head'),
(49, 'Body part for', 'Torso'),
(50, 'Reportee to', 'Doctor'),
(51, 'Reportee to', 'Doctor'),
(52, 'Bed from', 'RadiologyRoom'),
(52, 'Bed to', 'ICUBed02'),
(53, 'Bed from', 'ICUBed02'),
(53, 'Bed to', 'ICUBed02');

-- --------------------------------------------------------

--
-- Table structure for table `log_position`
--

CREATE TABLE IF NOT EXISTS `log_position` (
  `userName` varchar(50) NOT NULL,
  `time` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `x` float NOT NULL,
  `y` float NOT NULL,
  `z` float NOT NULL,
  PRIMARY KEY (`userName`,`time`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
