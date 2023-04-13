using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SistemaDeChamado.Context;
using SistemaDeChamado.Models;

namespace SistemaDeChamado
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            var config = new AutoMapper.MapperConfiguration(cfg =>
            {                
                cfg.CreateMap<UsuarioViewModel, Entity.Usuarios>().ReverseMap();
                cfg.CreateMap<CrieateUsuarioViewModel, Entity.Usuarios>().ReverseMap();
                cfg.CreateMap<EditarUsuariosViewModel, Entity.Usuarios>().ReverseMap();
                cfg.CreateMap<CreateChamadoViewModel, Entity.Chamado>().ReverseMap();
                cfg.CreateMap<CreateChamadoLinhasViewModel, Entity.ChamadoLinhas>().ReverseMap();
            });
            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);


            string conectar = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<MyContext>(options =>
                options.UseMySql(conectar, ServerVersion.AutoDetect(conectar)));


            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(opt =>
                    {
                        //opt.Cookie.Name = "_auth";
                        opt.Cookie.HttpOnly = true;
                        opt.LoginPath = new PathString("/login/Index");
                        opt.LogoutPath = new PathString("/login/logou");
                        opt.AccessDeniedPath = new PathString("/login/Index");
                        opt.ExpireTimeSpan = new System.TimeSpan(1, 0, 0);
                        opt.SlidingExpiration = true;
                        //opt.ExpireTimeSpan = TimeSpan.FromDays(1);

                    });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddControllersWithViews();
            services.AddMemoryCache();

            services.AddSession();
        }

        public void Configure(IApplicationBuilder app,
       IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }


            //app.UseHttpsRedirection();


            //app.UseAuthentication();

            app.UseStaticFiles();
            app.UseRouting();

            app.UseSession();

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Login}/{action=Index}/{id?}");
            });
        }
    }
}
