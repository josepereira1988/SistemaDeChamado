using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace SistemaDeChamado.Context
{
    public class ContextFactory : IDesignTimeDbContextFactory<MyContext>
    {
        // FTP DHVkMZb@ 
        ////"DefaultConnection": "server=mysql.portal.ajcinformatica.com.br;port=3306;database=portal07;user=portal07;password=9byMNUa6vjArW6a"

        public MyContext CreateDbContext(string[] args)
        {

            string connectionString = "server=mysql.portal.ajcinformatica.com.br;port=3306;database=portal07;user=portal07;password=9byMNUa6vjArW6a";
            //string connectionString = "server=localhost;port=3306;database=testeAJC;user=root;password=paralela";
            //string connectionString = _Configuration.GetConnectionString("DefaultConnection");
            var optionsBuilder = new DbContextOptionsBuilder<MyContext>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            return new MyContext(optionsBuilder.Options);
        }
    }
}
