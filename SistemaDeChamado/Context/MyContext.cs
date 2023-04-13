using Microsoft.EntityFrameworkCore;
using SistemaDeChamado.Entity;
using SistemaDeChamado.Essencial;
namespace SistemaDeChamado.Context
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuarios>().Property(e=>e.Email).IsRequired(false);
            modelBuilder.Entity<Chamado>().HasOne(d => d.Usuario).WithMany().IsRequired(false).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Chamado>().HasOne(d => d.Tecnico).WithMany().IsRequired(false).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Chamado>().HasOne(d => d.Cliente).WithMany().IsRequired(false).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Chamado>().HasOne(d => d.StatusChamado).WithMany().IsRequired(false).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<ChamadoLinhas>().HasOne(d => d.StatusChamado).WithMany().IsRequired(false).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<ChamadoLinhas>().HasOne(d => d.Usuario).WithMany().IsRequired(false).OnDelete(DeleteBehavior.NoAction);
            //modelBuilder.Entity<ChamadoLinhas>().HasOne(d => d.Chamado).WithMany().IsRequired(false).OnDelete(DeleteBehavior.ClientNoAction);
            modelBuilder.Entity<Usuarios>().HasOne(d => d.cliente).WithMany().IsRequired(false).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Usuarios>().HasData(
            new Usuarios
            {
                Id = 1,
                Usuario = "Manager",
                Nome = "AJC Informatica",
                Senha = Criptografia.Criptografar("123", "3fCi6hZNjl0Ib@f"),
                Classificacao = ControleUsuario.Master,
                idSistema = "3fCi6hZNjl0Ib@f",
                Email = "jose.junior@ajcinformatica.com.br",
                Ativo = true,
                ClienteMaster = false
            });
            modelBuilder.Entity<StatusChamado>().HasData(new StatusChamado { Id = 1, Descricao = "Aberto" });
            modelBuilder.Entity<StatusChamado>().HasData(new StatusChamado { Id = 2, Descricao = "Aguardando Atendimento" });
            modelBuilder.Entity<StatusChamado>().HasData(new StatusChamado { Id = 3, Descricao = "Em Andamento" });
            modelBuilder.Entity<StatusChamado>().HasData(new StatusChamado { Id = 4, Descricao = "Aguardando Cliente" });
            modelBuilder.Entity<StatusChamado>().HasData(new StatusChamado { Id = 5, Descricao = "Aguardando Terceiros" });
            modelBuilder.Entity<StatusChamado>().HasData(new StatusChamado { Id = 6, Descricao = "Finalizado" });
            //modelBuilder.Entity<PreviaMensal>().ToSqlQuery("");

        }
        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<StatusChamado> StatusChamado { get; set; }
        public DbSet<Chamado> Chamados { get; set; }
        public DbSet<ChamadoLinhas> ChamadoLinhas { get; set; }
        //public virtual DbSet<PreviaMensal> PreviaMensal { get; set; }
        
        
    }
}
