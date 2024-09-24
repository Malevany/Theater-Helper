GO
CREATE VIEW ActorsWithTitles AS
SELECT        dbo.Actors.Actor_ID AS ID, dbo.Actors.Surname, dbo.Actors.Name, dbo.Actors.Patronymic, dbo.Actors.Salary, dbo.Titles.Name AS Title
FROM            dbo.Actors INNER JOIN
                         dbo.ActorsTitles ON dbo.Actors.Actor_ID = dbo.ActorsTitles.Actor_ID INNER JOIN
                         dbo.Titles ON dbo.ActorsTitles.Title_ID = dbo.Titles.Title_ID;

GO
CREATE VIEW RolesWithPlayTitleAndImportance AS
SELECT        R.Role_ID, P.Name, R.Name AS RoleName, I.Name AS Importance
FROM            dbo.Roles AS R INNER JOIN
                         dbo.PlayRoles AS PR ON PR.Role_ID = R.Role_ID INNER JOIN
                         dbo.Plays AS P ON PR.Play_ID = P.Play_ID INNER JOIN
                         dbo.RoleImportances AS RI ON RI.Role_ID = R.Role_ID INNER JOIN
                         dbo.Importances AS I ON RI.Importance_ID = I.Importance_ID;

GO
CREATE PROCEDURE [dbo].[AddActorWithTitle]
	@NewActor_ID INT OUT,
    @Surname VARCHAR(100),
    @Name VARCHAR(100),
    @Patronymic VARCHAR(100),
    @Salary DECIMAL(10,2),
    @TitleName VARCHAR(100)
AS
BEGIN
    BEGIN TRANSACTION;

    INSERT INTO Actors (Surname, Name, Patronymic, Salary)
    VALUES (@Surname, @Name, @Patronymic, @Salary);

    SET @NewActor_ID = SCOPE_IDENTITY();

    DECLARE @TitleID INT;
    SELECT @TitleID = Title_ID
    FROM Titles
    WHERE Name = @TitleName;
	
    INSERT INTO ActorsTitles (Actor_ID, Title_ID)
    VALUES (@NewActor_ID, @TitleID);

    COMMIT TRANSACTION;
END;

GO
CREATE PROCEDURE [dbo].[AddNewExecutors]
    @Troupe_ID INT,
    @Role_ID INT,
    @MainActor_ID INT,
    @Doubler_ID INT
AS
BEGIN
    INSERT INTO TroupeComposition (Troupe_ID, Role_ID, MainActor_ID, DoublerActor_ID)
	VALUES (@Troupe_ID, @Role_ID, @MainActor_ID, @Doubler_ID)
END;

GO 
CREATE PROCEDURE [dbo].[AddNewPlay]
	@NewPlay_ID INT OUT,
	@PlayName VARCHAR(100),
	@Duration Time,
	@PlayStatusName VARCHAR(100)
AS
BEGIN
    BEGIN TRANSACTION;
	
	INSERT INTO Plays(Name, Duration)
	VALUES (@PlayName, @Duration);
	SET @NewPlay_ID = SCOPE_IDENTITY();

	DECLARE @PlayStatus_ID INT;
    SELECT @PlayStatus_ID = PlayStatuses_ID
    FROM PlayStatuses
    WHERE Name = @PlayStatusName;
    
	INSERT INTO Repertoire(Play_ID, PlayStatuses_ID)
	VALUES (@NewPlay_ID, @PlayStatus_ID)

    COMMIT TRANSACTION;
END;

GO
CREATE PROCEDURE [dbo].[AddNewRole]
	@NewRole_ID INT OUT,
	@CharacterName VARCHAR(100),
	@ImportanceName VARCHAR(100),
	@PlayTitle VARCHAR(100)
AS
BEGIN
    BEGIN TRANSACTION;

    INSERT INTO Roles(Name)
    VALUES (@CharacterName);

    SET @NewRole_ID = SCOPE_IDENTITY();
	
	DECLARE @Importance_ID INT;
    SELECT @Importance_ID = Importance_ID
    FROM Importances
    WHERE Name = @ImportanceName;

    INSERT INTO RoleImportances(Role_ID, Importance_ID)
    VALUES (@NewRole_ID, @Importance_ID);

	DECLARE @Play_ID INT;
    SELECT @Play_ID = Play_ID
    FROM Plays
    WHERE Name = @PlayTitle;

	INSERT INTO PlayRoles(Play_ID, Role_ID)
	VALUES (@Play_ID, @NewRole_ID)

    COMMIT TRANSACTION;
END;

GO
CREATE PROCEDURE [dbo].[AddNewTroupe]
    @NewTroupeID INT OUTPUT
AS
BEGIN
    INSERT INTO Troupes DEFAULT VALUES;
    SET @NewTroupeID = SCOPE_IDENTITY();
END;

GO
CREATE PROCEDURE [dbo].[AddSessionSubmissions]
	@NewSessionSubmissions_ID INT OUT,
	@SessionDate DateTime,
	@NumberOfSoldTickets INT,
	@Play_ID INT,
	@Troupe_ID INT
AS
BEGIN
    BEGIN TRANSACTION;
	
	DECLARE @NewSession_ID INT;
	INSERT INTO Sessions(SessionDate, NumberOfSoldTickets)
	VALUES (@SessionDate, @NumberOfSoldTickets);
	SET @NewSession_ID = SCOPE_IDENTITY();

	DECLARE @NewSubmissions_ID INT;
	INSERT INTO Submissions(Play_ID, Troupe_ID)
	VALUES (@Play_ID, @Troupe_ID)
	SET @NewSubmissions_ID = SCOPE_IDENTITY();
    
	INSERT INTO SessionSubmissions(Session_ID, Submission_ID)
	VALUES (@NewSession_ID, @NewSubmissions_ID)
	SET @NewSessionSubmissions_ID = SCOPE_IDENTITY();

    COMMIT TRANSACTION;
END;

GO
CREATE PROCEDURE [dbo].[GetActorsWithTitles] AS
BEGIN 
	SELECT Actors.Actor_ID, Actors.Surname, Actors.Name, Actors.Patronymic, Actors.Salary, Titles.Name AS Title
	  FROM Actors JOIN ActorsTitles ON Actors.Actor_ID = ActorsTitles.Actor_ID
				  JOIN Titles ON ActorsTitles.Title_ID = Titles.Title_ID
END;

GO
CREATE PROCEDURE [dbo].[GetDatesByMonthAndYear]
    @Date DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  SS.ID, SS.Session_ID, SS.Submission_ID
	FROM SessionSubmissions AS SS
		JOIN Sessions AS S ON SS.Session_ID = S.Session_ID
    WHERE YEAR(SessionDate) = YEAR(@Date) AND MONTH(SessionDate) = MONTH(@Date);
END;

GO
CREATE PROCEDURE [dbo].[GetRepertoire] AS
	SELECT P.Play_ID, P.Name, P.Duration, PS.Name
	FROM Plays AS P 
		JOIN Repertoire AS R ON P.Play_ID = R.Play_ID
		JOIN PlayStatuses AS PS ON R.PlayStatuses_ID = PS.PlayStatuses_ID;

GO
CREATE PROCEDURE [dbo].[GetRolesForPlay] 
	@PlayName VARCHAR(100)
AS
SELECT R.Role_ID, P.Name, R.Name, I.Name
	FROM Roles AS R JOIN PlayRoles AS PR ON PR.Role_ID = R.Role_ID
					JOIN Plays AS P ON PR.Play_ID = P.Play_ID
					JOIN RoleImportances AS RI ON RI.Role_ID = R.Role_ID
					JOIN Importances AS I ON RI.Importance_ID = I.Importance_ID
	WHERE @PlayName = P.Name;

GO
CREATE PROCEDURE [dbo].[GetTimtableByMonthAndYear]
	@Date DATETIME
AS
BEGIN
	SELECT SS.ID,
		   SS.Session_ID, SessionDate, NumberOfSoldTickets,
		   SS.Submission_ID,
		   Plays.Play_ID, Plays.Name, Duration, 
		   TC.Troupe_ID, 
		   R.Role_ID, Plays.Name, R.Name, I.Name,
		   MA.Actor_ID, MA.Surname, MA.Name, MA.Patronymic, MA.Salary, MAT.Name,
		   DA.Actor_ID, DA.Surname, DA.Name, DA.Patronymic, DA.Salary, DAT.Name
	FROM SessionSubmissions AS SS
	    JOIN Sessions ON SS.Session_ID = Sessions.Session_ID
	    JOIN Submissions ON SS.Submission_ID = Submissions.Submission_ID
		JOIN Plays ON Submissions.Play_ID = Plays.Play_ID
		JOIN TroupeComposition AS TC ON Submissions.Troupe_ID = TC.Troupe_ID
		JOIN Roles AS R ON TC.Role_ID = R.Role_ID
		JOIN RoleImportances AS RI ON R.Role_ID = RI.Role_ID
		JOIN Importances AS I ON RI.Importance_ID = I.Importance_ID
		JOIN Actors AS MA ON TC.MainActor_ID = MA.Actor_ID
		JOIN ActorsTitles AS MAAT ON MA.Actor_ID = MAAT.Actor_ID
		JOIN Titles AS MAT ON MAAT.Title_ID = MAT.Title_ID
		JOIN Actors AS DA ON TC.DoublerActor_ID = DA.Actor_ID
		JOIN ActorsTitles AS DAAT ON DA.Actor_ID = DAAT.Actor_ID
		JOIN Titles AS DAT ON DAAT.Title_ID = DAT.Title_ID
	WHERE YEAR(SessionDate) = YEAR(@Date) AND MONTH(SessionDate) = MONTH(@Date);
END;

GO
CREATE PROCEDURE [dbo].[GetTroupesForPlay]
	@PlayName VARCHAR(100)
AS
SELECT TC.ID, TC.Troupe_ID, TC.Role_ID, TC.MainActor_ID, TC.DoublerActor_ID
FROM TroupeComposition AS TC JOIN Roles AS R ON TC.Role_ID = R.Role_ID
							 JOIN PlayRoles AS PR ON R.Role_ID = PR.Role_ID
							 JOIN Plays AS P ON PR.Play_ID = P.Play_ID
	WHERE P.Name = @PlayName;

GO
CREATE PROCEDURE [dbo].[UpdateActorWithTitle]
	@Actor_ID INT,
    @Surname VARCHAR(100),
    @Name VARCHAR(100),
    @Patronymic VARCHAR(100),
    @Salary DECIMAL(10,2),
    @TitleName VARCHAR(100)
AS
BEGIN
    BEGIN TRANSACTION;

    DECLARE @NewActorID INT;
    UPDATE Actors
		SET Surname = @Surname,
			Name = @Name,
			Patronymic = @Patronymic,
			Salary = @Salary
		WHERE Actor_ID = @Actor_ID

    DECLARE @TitleID INT;
    SELECT @TitleID = Title_ID
    FROM Titles
    WHERE Name = @TitleName;
	
    UPDATE ActorsTitles 
		SET	Title_ID = @TitleID
		WHERE Actor_ID = @Actor_ID

    COMMIT TRANSACTION;
END;

GO
CREATE PROCEDURE [dbo].[UpdateExecutors]
    @Executors_ID INT,
    @Role_ID INT,
    @MainActor_ID INT,
    @Doubler_ID INT
AS
BEGIN
    UPDATE TroupeComposition
    SET Role_ID = @Role_ID,
        MainActor_ID = @MainActor_ID,
        DoublerActor_ID = @Doubler_ID
    WHERE ID = @Executors_ID;
END;

GO
CREATE PROCEDURE [dbo].[UpdatePlay]
	@Play_ID INT,
	@PlayName VARCHAR(100),
	@Duration Time
AS
BEGIN
    UPDATE Plays 
	SET Name = @PlayName,
		Duration = @Duration
	WHERE Play_ID = @Play_ID;
END;

GO
CREATE PROCEDURE [dbo].[UpdateRole]
	@Role_ID VARCHAR(100),
	@RoleName VARCHAR(100),
	@ImportanceName VARCHAR(100)
AS
BEGIN
    BEGIN TRANSACTION;

    UPDATE Roles 
	SET Name = @RoleName
	WHERE Role_ID = @Role_ID;

	
	DECLARE @Importance_ID INT;
    SELECT @Importance_ID = Importance_ID
    FROM Importances
    WHERE Name = @ImportanceName;

    UPDATE RoleImportances
	SET Importance_ID = @Importance_ID
	WHERE Role_ID = @Role_ID;

	COMMIT TRANSACTION;
END;

GO
CREATE PROCEDURE [dbo].[UpdateSessionSubmissions]
	@Session_ID INT,
	@SessionDate DateTime,
	@NumberOfSoldTickets INT,
	@Submission_ID INT,
	@Play_ID INT,
	@Troupe_ID INT
AS
BEGIN
    BEGIN TRANSACTION;

    UPDATE Sessions
		SET SessionDate = @SessionDate,
			NumberOfSoldTickets = @NumberOfSoldTickets
		WHERE Session_ID = @Session_ID
	
    UPDATE Submissions 
		SET	Play_ID = @Play_ID,
			Troupe_ID = @Troupe_ID
		WHERE Submission_ID = @Submission_ID

    COMMIT TRANSACTION;
END;