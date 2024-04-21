# AspNetCore Identity How To


# Starting Sql Server docker container
´´´´powershell and ensure docker is running
$sa_password = "sa pwd goes here"
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=$sa_password" -p 1433:1433 -d -v sqlvolume:/var/opt/mssql --rm --name mymssql mcr.microsoft.com/mssql/server:2022-latest
# checkin for the image
docker ps   


## Creating the project
These are the packages I've installed to work with Identity
1.- Create the project
/*CLI vscode*/
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer


## Using Secrets to manage the password
dotnet user-secrets init
$sa_password = "YOUPASSWORD"
dotnet user-secrets set "ConnectionStrings:SecurityContext" "Server=localhost; Database=AspNetCoreIdentity; User Id=sa; Password=$sa_password; TrustServerCertificate=True"

## Using secrets to manage the JWT key
dotnet user-secrets set "AspNetCoreIdentityJWTKet" "ASDFDSDFGASDASDFASVSADFASDGASDFDFGDGRHGGHSDHSDGASDVDFGADGDHSFHSDHSDGASERTEAGADGDFHSDFGASXCSDF"


## After created the : IdentityDbContext class and addint the configuration on programs .cs
dotnet ef migrations add AspNetCoreIdentity
dotnet ef database update