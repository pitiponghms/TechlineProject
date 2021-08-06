/****** Object:  Trigger [dbo].[trgInsert]    Script Date: 6/17/2021 8:43:24 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER TRIGGER [dbo].[trgInsert] ON [dbo].[ro_case] 
AFTER INSERT 
AS 
BEGIN
    DECLARE @YearPrefix CHAR(4)
	DECLARE @YearCnt int
    SET @YearPrefix = RIGHT(CAST(YEAR(GETDATE()) AS CHAR(4)), 4) 
	--SET @YearCnt= count(*)  from ro_case  where inserted.tafyear=getyear()
	select @YearCnt = count(*) from ro_case where tafyear=YEAR(getdate())
    UPDATE dbo.ro_case
    SET tafno ='TAF-' + @YearPrefix + '-' + RIGHT('00000' + CAST(@YearCnt AS VARCHAR(5)), 5) 
    FROM INSERTED i
    WHERE dbo.ro_case.id = i.id
END
GO

ALTER TABLE [dbo].[ro_case] ENABLE TRIGGER [trgInsert]
GO