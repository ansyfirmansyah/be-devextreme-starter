-- Generate Entity berdasarkan DB --
dotnet ef dbcontext scaffold "Server=[path dan nama server];Database=[nama database];User Id=[nama user database];Password=[password database];TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer --output-dir Data/Models --context DataEntities --force --no-onconfiguring --use-database-names

gunakan --use-database-names jika ingin nama model / entity sesuai nama di database, jika dihapus maka akan auto konversi ke PascalCase