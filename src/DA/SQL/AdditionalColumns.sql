USE MiracleList_Test;
GO
-- Add Columns
ALTER table [user] add  City nvarchar(MAX);
alter table [user] add Important bit null;
go
UPDATE [user] SET city = 'Mainz' WHERE UserID % 5 = 0
UPDATE [user] SET city = 'Essen' WHERE UserID % 3 = 0
UPDATE [user] SET Important = (select convert(bit, round(1*rand(),0))) WHERE UserID % 2 = 0

--REMOVE columns
USE MiracleList_Test;
GO

ALTER table [user] drop COLUMN  City ;

ALTER table [user] drop COLUMN  Important ;