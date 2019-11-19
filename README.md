
# Data Warehouse Data Check Application

## Purpose
The purpose of this application is to help user find the rows in the System Title of Warehouse that cause the cube to have issues building

## Issue 
Customers experience an error in the format:<br />
Errors in the OLAP storage engine: The attribute key cannot be found when processing: Table: 'dbo_vDimWorkItemOverlay', Column: 'System_Title', Value: 'WorkItem Title here '. The attribute is 'System_Title'. Errors in the OLAP storage engine: The process operation ended because the number of errors encountered during processing reached the defined limit of allowable errors for the operation. Errors in the OLAP storage engine: An error occurred while the 'Work Item' attribute of the 'Work Item' dimension from the 'TFS_Analysis' database was being processed. Internal error: The operation terminated unsuccessfully. Server: The current operation was cancelled because another operation in the transaction failed.<br />
The issue is that the SQL server collation/version sees 2 titles as a match that the .Net code do NOT.
For instance if the Collation is not Unicode sensitive and there is a Unicode Char -OR- if it is an older version of SQL and there is an Ascii Control Char, for instance Unit Separator, at the end of the string. 

## Description
In order to make this application work for the most scenarios as possible the application does not try to use SQL itself to find any of the problematic characters.  Instead the application asks SQL to return all the distinct rows that IT can see and the minimum length that SQL sees for that group of rows.  It takes each title that SQL sees, then searches for all the rows that match the substring of the shortest length with a wild card, that do not ALSO exist in the distinct list.  It then asks .NET if there is more than 1 distinct row in that group, if so it will write that title to a file for use to investigate.  Once it is done iterating through the list it opens the results to investigate with notepad, and closes itself.

## Instructions
To use the application, you can run as default: DWDataCheck.exe<br />
Default Values:<br />
Server: (local)<br />
DB: TFS_Warehouse<br />
File: C:\Temp\DWDataCheckLog.log<br />
You can Run in the following formats:<br />
DWDataCheck.exe ServerName<br />
OR<br />
DWDataCheck.exe ServerName DBName<br />
OR<br />
DWDataCheck.exe ServerName DBName FileName<br />

## Code
As of the time of this Documents Creation the Code is located at https://github.com/SamGrantham/DWDataCheck


