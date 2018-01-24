--upgrade scripts from nopCommerce 4.00 to 4.10

--new locale resources
declare @resources xml
--a resource will be deleted if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Admin.Configuration.Currencies.Fields.CurrencyCode.Hint">
    <Value>The currency code. For a list of currency codes, go to: https://en.wikipedia.org/wiki/ISO_4217</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Fields.Avatar">
    <Value>Avatar</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ExportImportAllowDownloadImages">
    <Value>Export/Import products. Allow download images</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ExportImportAllowDownloadImages.Hint">
    <Value>Check if images can be downloaded from remote server when exporting products</Value>
  </LocaleResource>  
  <LocaleResource Name="Account.Fields.County">
    <Value>County</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.County.Required">
    <Value>County is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Address.Fields.County">
    <Value>County</Value>
  </LocaleResource>
  <LocaleResource Name="Address.Fields.County.Required">
    <Value>County is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.Fields.County">
    <Value>County</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.Fields.County.Hint">
    <Value>Enter county.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.Fields.County.Required">
    <Value>County is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.CountyEnabled">
    <Value>''County'' enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.CountyEnabled.Hint">
    <Value>Set if ''County'' is enabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.CountyRequired">
    <Value>''County'' required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.AddressFormFields.CountyRequired.Hint">
    <Value>Check if ''County'' is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.CountyEnabled">
    <Value>''County'' enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.CountyEnabled.Hint">
    <Value>Set if ''County'' is enabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.CountyRequired">
    <Value>''County'' required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.CountyRequired.Hint">
    <Value>Check if ''County'' is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Fields.County">
    <Value>County</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Fields.County.Hint">
    <Value>The county.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Fields.County.Required">
    <Value>County is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Address.County">
    <Value>County</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.List.County">
    <Value>County</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.List.County.Hint">
    <Value>Search by a specific county.</Value>
  </LocaleResource>
</Language>
'

CREATE TABLE #LocaleStringResourceTmp
	(
		[ResourceName] [nvarchar](200) NOT NULL,
		[ResourceValue] [nvarchar](max) NOT NULL
	)

INSERT INTO #LocaleStringResourceTmp (ResourceName, ResourceValue)
SELECT	nref.value('@Name', 'nvarchar(200)'), nref.value('Value[1]', 'nvarchar(MAX)')
FROM	@resources.nodes('//Language/LocaleResource') AS R(nref)

--do it for each existing language
DECLARE @ExistingLanguageID int
DECLARE cur_existinglanguage CURSOR FOR
SELECT [ID]
FROM [Language]
OPEN cur_existinglanguage
FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
WHILE @@FETCH_STATUS = 0
BEGIN
	DECLARE @ResourceName nvarchar(200)
	DECLARE @ResourceValue nvarchar(MAX)
	DECLARE cur_localeresource CURSOR FOR
	SELECT ResourceName, ResourceValue
	FROM #LocaleStringResourceTmp
	OPEN cur_localeresource
	FETCH NEXT FROM cur_localeresource INTO @ResourceName, @ResourceValue
	WHILE @@FETCH_STATUS = 0
	BEGIN
		IF (EXISTS (SELECT 1 FROM [LocaleStringResource] WHERE LanguageID=@ExistingLanguageID AND ResourceName=@ResourceName))
		BEGIN
			UPDATE [LocaleStringResource]
			SET [ResourceValue]=@ResourceValue
			WHERE LanguageID=@ExistingLanguageID AND ResourceName=@ResourceName
		END
		ELSE 
		BEGIN
			INSERT INTO [LocaleStringResource]
			(
				[LanguageId],
				[ResourceName],
				[ResourceValue]
			)
			VALUES
			(
				@ExistingLanguageID,
				@ResourceName,
				@ResourceValue
			)
		END
		
		IF (@ResourceValue is null or @ResourceValue = '')
		BEGIN
			DELETE [LocaleStringResource]
			WHERE LanguageID=@ExistingLanguageID AND ResourceName=@ResourceName
		END
		
		FETCH NEXT FROM cur_localeresource INTO @ResourceName, @ResourceValue
	END
	CLOSE cur_localeresource
	DEALLOCATE cur_localeresource


	--fetch next language identifier
	FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
END
CLOSE cur_existinglanguage
DEALLOCATE cur_existinglanguage

DROP TABLE #LocaleStringResourceTmp
GO

--new index
IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_GetLowStockProducts' and object_id=object_id(N'[dbo].[Product]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_GetLowStockProducts] ON [Product] (Deleted ASC, VendorId ASC, ProductTypeId ASC, ManageInventoryMethodId ASC, MinStockQuantity ASC, UseMultipleWarehouses ASC)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.exportimportallowdownloadimages')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.exportimportallowdownloadimages', N'false', 0)
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[Address]') AND NAME = 'County')
BEGIN
	ALTER TABLE [Address]
	ADD [County] NVARCHAR (MAX) NULL
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'addresssettings.countyenabled')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'addresssettings.countyenabled', N'false', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'addresssettings.countyrequired')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'addresssettings.countyrequired', N'false', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.countyenabled')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'customersettings.countyenabled', N'false', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.countyrequired')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'customersettings.countyrequired', N'false', 0)
END
GO