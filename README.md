## Three tiered application ##
Application Flow

- Start by going to <Server URL>/SalesTaxByState
- Loads a Razor Page (See Razor Page Info)

## SQLServerAPI Solution - Database Tier ##

If ever rebuilding SQL server you will need to run the following to build the stored procedure.
```
CREATE PROCEDURE [Sales].[spTaxRateByState]
    @CountryRegionCode NVARCHAR(3)
AS 
    SET NOCOUNT ON ;
 
    SELECT  [st].[SalesTaxRateID],
            [st].[Name],
            [st].[TaxRate],
            [st].[TaxType],
            [sp].[Name] AS StateName
    FROM    [Sales].[SalesTaxRate] st
            JOIN [Person].[StateProvince] sp ON [st].[StateProvinceID]
             = [sp].[StateProvinceID]
    WHERE   [sp].[CountryRegionCode] = @CountryRegionCode
    ORDER BY [StateName]
GO
```

## Get Lets Encrypt Certificate - Ubuntu Server ##
Get cert:
Make sure DynDNS points to IP address
Make sure port forwarding points to Ubuntu instance and nginx is running
sudo certbot certonly --nginx

openssl pkcs12 -export -out certificate.pfx -inkey privkey1.pem -in fullchain1.pem