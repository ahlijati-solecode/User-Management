*Update Database schema*


Scaffold-DbContext "Server=dbpgdev.solecode.tech;Database=mufg_ens_user_management;Port=5432;User Id=mufg_ens;Password=bXVmZ19lbnNfMjgwMio=;" Npgsql.EntityFrameworkCore.PostgreSQL -OutputDir Models\Generated -Context UserDbContext -Force

Scaffold-DbContext "Server=dbpgdev.solecode.tech;Database=mufg_ens_configuration_management;Port=5432;User Id=mufg_ens;Password=bXVmZ19lbnNfMjgwMio=" Npgsql.EntityFrameworkCore.PostgreSQL -OutputDir Models\Generated -Context ConfigurationDbContext -Force
