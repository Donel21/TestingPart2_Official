USE RaceDay;
GO

-- 1. INSERT EVENTS (Using OrganiserID 7 = John Doe)
IF NOT EXISTS (SELECT 1 FROM [dbo].[Events] WHERE EventName = 'Cape Town Cycle Tour')
BEGIN
    INSERT INTO [dbo].[Events] (OrganiserID, EventName, EventDate, Location, Description, BannerImageUrl)
    VALUES 
    (7, 'Cape Town Cycle Tour', '2026-11-19', 'Cape Town', 'The world''s largest timed cycling event. Beautiful coastal views!', NULL),
    (7, 'Johannesburg City Run', '2026-12-05', 'Johannesburg', 'A fast-paced road race through the heart of Joburg.', NULL),
    (7, 'Durban Beachfront Marathon', '2027-01-15', 'Durban', 'Enjoy the ocean breeze in this flat and fast marathon.', NULL);
END
GO

-- 2. INSERT CATEGORIES
IF NOT EXISTS (SELECT 1 FROM [dbo].[Categories] WHERE CategoryName = '40km Cycle')
BEGIN
    -- Categories for Cape Town
    INSERT INTO [dbo].[Categories] (EventID, CategoryName, DistanceKm, EntryFee)
    VALUES 
    ((SELECT EventID FROM [dbo].[Events] WHERE EventName = 'Cape Town Cycle Tour'), '40km Cycle', 40.00, 350.00),
    ((SELECT EventID FROM [dbo].[Events] WHERE EventName = 'Cape Town Cycle Tour'), '80km Cycle', 80.00, 500.00);

    -- Categories for Johannesburg
    INSERT INTO [dbo].[Categories] (EventID, CategoryName, DistanceKm, EntryFee)
    VALUES 
    ((SELECT EventID FROM [dbo].[Events] WHERE EventName = 'Johannesburg City Run'), '10km Run', 10.00, 150.00),
    ((SELECT EventID FROM [dbo].[Events] WHERE EventName = 'Johannesburg City Run'), '21km Half Marathon', 21.10, 250.00);

    -- Categories for Durban
    INSERT INTO [dbo].[Categories] (EventID, CategoryName, DistanceKm, EntryFee)
    VALUES 
    ((SELECT EventID FROM [dbo].[Events] WHERE EventName = 'Durban Beachfront Marathon'), '42km Marathon', 42.20, 400.00),
    ((SELECT EventID FROM [dbo].[Events] WHERE EventName = 'Durban Beachfront Marathon'), '5km Fun Run', 5.00, 50.00);
END
GO

-- 3. VERIFY THE DATA
SELECT * FROM [dbo].[Events];
SELECT * FROM [dbo].[Categories];

---------
USE RaceDay;


-- 1. INSERT 6 NEW EVENTS (Using IF NOT EXISTS so it doesn't crash if they exist)
IF NOT EXISTS (SELECT 1 FROM [dbo].[Events] WHERE EventName = 'Two Oceans Marathon')
BEGIN
    INSERT INTO [dbo].[Events] (OrganiserID, EventName, EventDate, Location, Description, BannerImageUrl)
    VALUES 
    (7, 'Two Oceans Marathon', '2027-04-15', 'Cape Town', 'The world''s most beautiful marathon. A grueling but breathtaking route.', NULL),
    (7, 'Comrades Ultra-Marathon', '2027-06-10', 'Durban', 'The ultimate human race. An epic 89km journey between Pietermaritzburg and Durban.', NULL),
    (7, 'Pretoria Capital Classic', '2027-02-20', 'Pretoria', 'A fast, flat route perfect for setting a new personal best.', NULL),
    (7, 'Bloemfontein Rose Run', '2027-03-12', 'Bloemfontein', 'Join us in the City of Roses for a beautiful morning run through the botanical gardens.', NULL),
    (7, 'Garden Route Trail Challenge', '2027-05-05', 'Knysna', 'A scenic trail run through the indigenous forests of the Garden Route.', NULL),
    (7, 'Soweto Night Run', '2027-10-30', 'Soweto', 'Experience the vibrant streets of Soweto under the stars in this unique evening race.', NULL);
END


-- 2. INSERT CATEGORIES SAFELY (Checking for duplicates)
-- Two Oceans
IF NOT EXISTS (SELECT 1 FROM [dbo].[Categories] WHERE CategoryName = '56km Ultra Marathon')
BEGIN
    INSERT INTO [dbo].[Categories] (EventID, CategoryName, DistanceKm, EntryFee)
    VALUES 
    ((SELECT EventID FROM [dbo].[Events] WHERE EventName = 'Two Oceans Marathon'), '21km Half Marathon', 21.10, 300.00),
    ((SELECT EventID FROM [dbo].[Events] WHERE EventName = 'Two Oceans Marathon'), '56km Ultra Marathon', 56.00, 650.00);
END

-- Comrades
IF NOT EXISTS (SELECT 1 FROM [dbo].[Categories] WHERE CategoryName = '89km Ultra')
BEGIN
    INSERT INTO [dbo].[Categories] (EventID, CategoryName, DistanceKm, EntryFee)
    VALUES 
    ((SELECT EventID FROM [dbo].[Events] WHERE EventName = 'Comrades Ultra-Marathon'), '89km Ultra', 89.00, 800.00);
END

-- Pretoria
IF NOT EXISTS (SELECT 1 FROM [dbo].[Categories] WHERE CategoryName = '21km Half Marathon' AND EventID = (SELECT EventID FROM [dbo].[Events] WHERE EventName = 'Pretoria Capital Classic'))
BEGIN
    INSERT INTO [dbo].[Categories] (EventID, CategoryName, DistanceKm, EntryFee)
    VALUES 
    ((SELECT EventID FROM [dbo].[Events] WHERE EventName = 'Pretoria Capital Classic'), '10km Run', 10.00, 120.00),
    ((SELECT EventID FROM [dbo].[Events] WHERE EventName = 'Pretoria Capital Classic'), '21km Half Marathon', 21.10, 220.00);
END

-- Bloemfontein
IF NOT EXISTS (SELECT 1 FROM [dbo].[Categories] WHERE CategoryName = '10km Run' AND EventID = (SELECT EventID FROM [dbo].[Events] WHERE EventName = 'Bloemfontein Rose Run'))
BEGIN
    INSERT INTO [dbo].[Categories] (EventID, CategoryName, DistanceKm, EntryFee)
    VALUES 
    ((SELECT EventID FROM [dbo].[Events] WHERE EventName = 'Bloemfontein Rose Run'), '5km Fun Run', 5.00, 60.00),
    ((SELECT EventID FROM [dbo].[Events] WHERE EventName = 'Bloemfontein Rose Run'), '10km Run', 10.00, 110.00);
END

-- Garden Route
IF NOT EXISTS (SELECT 1 FROM [dbo].[Categories] WHERE CategoryName = '30km Trail')
BEGIN
    INSERT INTO [dbo].[Categories] (EventID, CategoryName, DistanceKm, EntryFee)
    VALUES 
    ((SELECT EventID FROM [dbo].[Events] WHERE EventName = 'Garden Route Trail Challenge'), '15km Trail', 15.00, 200.00),
    ((SELECT EventID FROM [dbo].[Events] WHERE EventName = 'Garden Route Trail Challenge'), '30km Trail', 30.00, 350.00);
END

-- Soweto
IF NOT EXISTS (SELECT 1 FROM [dbo].[Categories] WHERE CategoryName = '10km Night Run')
BEGIN
    INSERT INTO [dbo].[Categories] (EventID, CategoryName, DistanceKm, EntryFee)
    VALUES 
    ((SELECT EventID FROM [dbo].[Events] WHERE EventName = 'Soweto Night Run'), '10km Night Run', 10.00, 140.00);
END


-- 3. VERIFY
SELECT * FROM [dbo].[Events];
SELECT * FROM [dbo].[Categories];