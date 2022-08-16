using AtoCash.Authentication;
using AtoCash.Data;
using EmailService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace AtoCash
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
    //        "ConnectionStrings": {
    //            "GoogleCloudGmailServer": "Server=34.93.3.254;Port=5432;Database=AtoCashDB;User Id=postgres;Password=Pa55word2019!123;Pooling=true;Timeout=300; CommandTimeout=300",
    //"GoogleCloudAtominosServer": "Server=35.200.228.204;Port=5432;Database=AtoCashDB;User Id=postgres;Password=Pa55word2019!123;Pooling=true;",
    //"PostgreSQLConnectionString": "Server=localhost;Port=5432;Database=AtoCashDB;User Id=postgres;Password=Pa55word2019!123;Pooling=true;",
    //"PostgreSQLInLocalAppInContainer": "Server=host.docker.internal;Port=5432;Database=AtoCashDB;User Id=postgres;Password=Pa55word2019!123;Pooling=true;",
    //"WithinContainerPostGreSQL": "Server=postgresdata;Port=5432;Database=AtoCashDB;User Id=postgres;Password=Pa55word2019!123;Pooling=true;Timeout=300; CommandTimeout=300"

            services.AddDbContextPool<AtoCashDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("GoogleCloudGmailServer")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AtoCashDbContext>()
                .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>(TokenOptions.DefaultProvider);    // to provide default token for password reset

            services.AddSwaggerDocumentation();

            //services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
            //.AddEntityFrameworkStores<AtoCashDbContext>()

            //services.AddHttpsRedirection(options => options.HttpsPort = 443);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opts =>
            {
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false, // setting it to false, as we dont know the users connecting to this server
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,


                    ValidIssuer ="https://localhost:5001",
                    ValidAudience = "https://localhost:5001",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySecretKey12323232"))
                };
            });

            //services.AddCors(options =>
            //  options.AddPolicy("myCorsPolicy", builder => {
            //      builder.AllowAnyOrigin()
            //               .AllowAnyHeader()
            //               .AllowAnyMethod();
                        //  }
            //  ));
            services.AddControllers();
            services.AddCors(options =>
               options.AddPolicy("myCorsPolicy", builder => {
                   builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                   }
               ));
            //email service
            var emailConfig = Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
            services.AddSingleton(emailConfig);

            services.AddScoped<IEmailSender, EmailSender>();
            ///

            //for file upload from Angular Form
            services.Configure<FormOptions>(o =>
            {
                o.ValueLengthLimit = int.MaxValue;
               o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
                });
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AtoCash", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AtoCash v1"));
            }
            app.UseStaticFiles();
      

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, @"Images")),
                RequestPath = "/app/Images"
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, @"Reportdocs")),
                RequestPath = "/app/Reportdocs"
            });

            app.UseHttpsRedirection();
            app.UseCors("myCorsPolicy");
            app.UseRouting();
            //app.UseForwardedHeaders(new ForwardedHeadersOptions
            //{
            //    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            //});

            //var forwardingOptions = new ForwardedHeadersOptions()
            //{
            //    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            //};
            //forwardingOptions.KnownNetworks.Clear(); // Loopback by default, this should be temporary
            //forwardingOptions.KnownProxies.Clear(); // Update to include

            //app.UseForwardedHeaders(forwardingOptions);




            app.UseAuthentication(); //add before MVC
            app.UseAuthorization();

            app.UseSwaggerDocumentation();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
