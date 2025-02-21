using Cadastro.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.Globalization;
using System.IO;

namespace Cadastro {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        //################################################################################################################################
        public IConfiguration Configuration { get; }
        //CultureInfo usCulture = new CultureInfo("en-US");
        CultureInfo ptCulture = new CultureInfo("pt-BR");

        //################################################################################################################################

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {

            /*services.Configure<RequestLocalizationOptions>(
              options => {
                var supportedCultures = new List<CultureInfo>
                  {
                    ptCulture
                  };

                options.DefaultRequestCulture = new RequestCulture(culture: "pt-BR", uiCulture: "pt-BR");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
              }
            );*/


            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Local")));

            services.AddMvc(options => options.EnableEndpointRouting = false);//
            services.AddControllersWithViews();


            services.Configure<FormOptions>(options => {
                // Set the limit to 256 MB
                options.BufferBodyLengthLimit = 104857600;
                options.MemoryBufferThreshold = 104857600;
                options.MultipartBodyLengthLimit = 104857600;//Máximo 100MB de Upload
            });

            services.AddDistributedMemoryCache();
            services.AddSession(options => {
                options.IOTimeout = TimeSpan.FromHours(3);
                options.IdleTimeout = TimeSpan.FromHours(3);
                options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            services.AddDistributedMemoryCache();
        }

        //################################################################################################################################
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {

            var supportedCultures = new[] { new CultureInfo("pt-BR") };
            app.UseRequestLocalization(new RequestLocalizationOptions {
                DefaultRequestCulture = new RequestCulture(culture: "pt-BR", uiCulture: "pt-BR"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            //Quando em desenvolvimento apresenta erros com detalhes, quando em produção, apresenta página de erro
            if(env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("/Login/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseHttpsRedirection();
            //app.UseStaticFiles(Path.Combine(Environment.CurrentDirectory, "StaticFiles"));
            app.UseStaticFiles(new StaticFileOptions {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Environment.CurrentDirectory, "StaticFiles", "wwwroot"))
            });

            app.UseSession(new SessionOptions() { IOTimeout = TimeSpan.FromHours(8) });

            //app.UseRouting();//

            app.UseAuthorization();

            /*app.UseEndpoints(endpoints => {
              endpoints.MapControllerRoute(
                  name: "default",
                  pattern: "{controller=Login}/{action=Index}/{id?}");
            });*/

            /*string template;
            if (File.Exists(Environment.CurrentDirectory + @$"\wwwroot\Paginas\index.html"))
              template = "{controller=Pagina}/{action=Index}/{id?}";
            else
              template = "{controller=Login}/{action=Index}/{id?}";*/

            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Login}/{action=Index}/{id?}");
            });//
        }
    }
}
