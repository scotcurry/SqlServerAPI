## Three tiered application ##
Application Flow

- Start by going to <Server URL>/SalesTaxByState
- Loads a Razor Page (See Razor Page Info)

## SQLServerAPI Solution - Database Tier ##

## Get Lets Encrypt Certificate ##
Get cert:
Make sure DynDNS points to IP address
Make sure port forwarding points to Ubuntu instance and nginx is running
sudo certbot certonly --nginx

openssl pkcs12 -export -out certificate.pfx -inkey privkey1.pem -in fullchain1.pem