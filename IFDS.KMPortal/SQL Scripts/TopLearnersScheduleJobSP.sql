DECLARE @Member nvarchar(100)
DECLARE @Web nvarchar(50)
DECLARE @List nvarchar(100)
DECLARE @ProcessFlag bit
DECLARE @AddtoReputationScore bit 
DECLARE @Columnname nvarchar(100)
DECLARE @alldepartment nvarchar(50)
DECLARE @ID int
DECLARE @Displayname nvarchar(200)

SET @alldepartment = 'All Departments'

DECLARE tobeprocessed CURSOR FAST_FORWARD FOR
SELECT s.Member, s.Web, s.List, s.ProcessFlag, s.ID, s.Displayname, d.AddtoReputationScore, d.Columnname FROM dbo.KMP_Z_URLAccessEntries AS s INNER JOIN dbo.KMP_Z_ProcessLists as d on s.List = d.Listname WHERE s.ProcessFlag = 'FALSE'

OPEN tobeprocessed;

FETCH NEXT FROM tobeprocessed INTO @Member, @web, @List, @ProcessFlag, @ID, @Displayname,@AddtoReputationScore, @Columnname;

WHILE @@FETCH_STATUS = 0 BEGIN

PRINT @Member;
PRINT @web;
PRINT @List;
PRINT @ProcessFlag;
PRINT @AddtoReputationScore;
PRINT @Columnname;
PRINT '-----ID------'
PRINT @ID;

DECLARE @count int

SET @count = (SELECT COUNT(*) FROM dbo.KMP_Z_TopLearners WHERE Member = @Member AND web = @Web)
	
    IF(@count = 0) 
	BEGIN
	
	PRINT 'NOT FOUND'
	
	DECLARE @sql nvarchar(max) = 'INSERT INTO [dbo].[KMP_Z_TopLearners] (Member, web, ' + @Columnname + ', ReputationScore, Displayname) VALUES (' + '''' + @Member + '''' +' , '+ ''''+ @web +'''' +' , 1, 1,' + '''' + @Displayname + '''' + ')';
	PRINT @sql
	exec sp_executesql @sql, N''

	END
	ELSE 
	BEGIN

	PRINT 'AddtoReputationScore =' 
	PRINT @AddtoReputationScore
	
	DECLARE @temp nvarchar(max) 

	IF(@AddtoReputationScore = 1)
	BEGIN
	SET @temp = 'UPDATE [dbo].[KMP_Z_TopLearners] SET ' + @Columnname + ' = ' + @Columnname + ' + 1, ReputationScore = ReputationScore + 1  WHERE Member = ' + ''''  + @Member + ''''  + ' AND web = ' + '''' +  @Web  + '''' ;
	PRINT @temp
	exec sp_executesql @temp, N''
	END 

	IF(@AddtoReputationScore = 0)
	BEGIN
	SET @temp = 'UPDATE [dbo].[KMP_Z_TopLearners] SET ' + @Columnname + ' = ' + @Columnname + ' + 1 WHERE Member = ' + ''''  + @Member + ''''  + ' AND web = ' + '''' +  @Web  + '''' ;
	PRINT @temp
	exec sp_executesql @temp, N''
	END

	END

	--Next check and process for all departments

	DECLARE @count1 int

    SET @count1 = (SELECT COUNT(*) FROM dbo.KMP_Z_TopLearners WHERE Member = @Member AND web = @alldepartment)
	
    IF(@count1 = 0) 
	BEGIN
	
	PRINT 'NOT FOUND ALL Departments'
	
	DECLARE @sql1 nvarchar(max) = 'INSERT INTO [dbo].[KMP_Z_TopLearners] (Member, web, ' + @Columnname + ', ReputationScore, Displayname) VALUES (' + '''' + @Member + '''' +' , '+ ''''+ @alldepartment +'''' +' , 1, 1,' + '''' + @Displayname + '''' + ')';
	PRINT @sql1
	exec sp_executesql @sql1, N''

	END
	ELSE 
	BEGIN

	PRINT 'AddtoReputationScore =' 
	PRINT @AddtoReputationScore
	
	DECLARE @temp1 nvarchar(max) 

	IF(@AddtoReputationScore = 1)
	BEGIN
	SET @temp1 = 'UPDATE [dbo].[KMP_Z_TopLearners] SET ' + @Columnname + ' = ' + @Columnname + ' + 1, ReputationScore = ReputationScore + 1  WHERE Member = ' + ''''  + @Member + ''''  + ' AND web = ' + '''' +  @alldepartment  + '''' ;
	PRINT @temp1
	exec sp_executesql @temp1, N''
	END 

	IF(@AddtoReputationScore = 0)
	BEGIN
	SET @temp1 = 'UPDATE [dbo].[KMP_Z_TopLearners] SET ' + @Columnname + ' = ' + @Columnname + ' + 1 WHERE Member = ' + ''''  + @Member + ''''  + ' AND web = ' + '''' +  @alldepartment  + '''' ;
	PRINT @temp1
	exec sp_executesql @temp1, N''
	END

	END  


	-- End of check and process for all departments
	-- Update the processflag to TRUE
	DECLARE @temp3 nvarchar(max) 
	SET @temp3 = 'UPDATE [dbo].[KMP_Z_URLAccessEntries] SET ProcessFlag = 1 WHERE ID = ' + CAST(@ID AS NVARCHAR(20));
	PRINT @temp3
	exec sp_executesql @temp3, N''

	-- end of update of processflag to TRUE


FETCH NEXT FROM tobeprocessed INTO @Member, @web, @List, @ProcessFlag, @ID,  @Displayname, @AddtoReputationScore, @Columnname;
END

CLOSE tobeprocessed;
DEALLOCATE tobeprocessed;