using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AtomStore.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AtomStore.Data.EF;
using AtomStore.Data.Entities;
using AutoMapper;
using AtomStore.Application.AutoMapper;
using AtomStore.Data.EF.Repositories;
using AtomStore.Data.IRepositories;
using AtomStore.Application.Interfaces;
using AtomStore.Application.Implementation;
using Newtonsoft.Json.Serialization;
using AtomStore.Helpers;
using AtomStore.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using AtomStore.Authorization;
using AtomStore.Extensions;
using AtomStore.Services;
using Microsoft.AspNetCore.Mvc.Razor;
using PaulMiami.AspNetCore.Mvc.Recaptcha;
using AtomStore.Aplication.Dapper.Interfaces;
using AtomStore.Aplication.Dapper.Implimentation;
using AtomStore.Hubs;
using AtomStore.Stripe;

namespace AtomStore
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection"),o=>o.MigrationsAssembly("AtomStore.Data.EF")));
            services.AddIdentity<AppUser, AppRole>()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options =>
            {
                //password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;

                //lock out settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;

                //user settings
                options.User.RequireUniqueEmail = true;
            });

            services.AddScoped<UserManager<AppUser>, UserManager<AppUser>>();
            services.AddScoped<RoleManager<AppRole>, RoleManager<AppRole>>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddTransient<DbInitializer>();

            //
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IViewRenderService, ViewRenderService>();
            services.AddMvc(options =>
                {
                    options.CacheProfiles.Add("Default",
                        new CacheProfile()
                        {
                            Duration = 60
                        });
                    options.CacheProfiles.Add("Never",
                        new CacheProfile()
                        {
                            Location = ResponseCacheLocation.None,
                            NoStore = true
                        });
                }).AddViewLocalization(
                    LanguageViewLocationExpanderFormat.Suffix,
                    opts => { opts.ResourcesPath = "Resources"; })
                .AddDataAnnotationsLocalization()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });
            services.AddRecaptcha(new RecaptchaOptions()
            {
                SiteKey = Configuration["Recaptcha:SiteKey"],
                SecretKey = Configuration["Recaptcha:SecretKey"]

            });
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(2);
                options.Cookie.HttpOnly = true;
            });
            services.AddAuthentication()
                .AddFacebook(facebookOpts =>
                {
                    facebookOpts.AppId = Configuration["Authentication:Facebook:AppId"];
                    facebookOpts.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                })
                .AddGoogle(googleOpts =>
                {
                    googleOpts.ClientId = Configuration["Authentication:Google:ClientId"];
                    googleOpts.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                });

            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));
            //
            services.AddScoped<IUserClaimsPrincipalFactory<AppUser>, CustomClaimsPrincipalFactory>();
            services.AddImageResizer();

            services.AddAutoMapper();
            services.AddSingleton(Mapper.Configuration);
            services.AddScoped(sp => new Mapper(sp.GetRequiredService<AutoMapper.IConfigurationProvider>(), sp.GetService));

            services.AddTransient(typeof(IUnitOfWork), typeof(EFUnitOfWork));
            services.AddTransient(typeof(IRepository<,>), typeof(EFRepository<,>));

            //Repositories
            services.AddTransient<IProductCategoryRepository, ProductCategoryRepository>();
            services.AddTransient<IFunctionRepository, FunctionRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<ITagRepository, TagRepository>();
            services.AddTransient<IProductTagRepository, ProductTagRepository>();
            services.AddTransient<IPermissionRepository, PermissionRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IOrderDetailRepository, OrderDetailRepository>();
            services.AddTransient<ISizeRepository, SizeRepository>();
            services.AddTransient<IColorRepository, ColorRepository>();
            services.AddTransient<IProductQuantityRepository, ProductQuantityRepository>();
            services.AddTransient<IProductImageRepository, ProductImageRepository>();

            //Services
            services.AddTransient<IProductCategoryService,ProductCategoryService>();
            services.AddTransient<IFunctionService, FunctionService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IAuthorizationHandler, BaseResourceAuthorizationHandlerAuthorization>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<ICommonService, CommonService>();
            services.AddTransient<IFeedbackService, FeedbackService>();
            services.AddTransient<IContactService, ContactService>();
            services.AddTransient<IReportService, ReportService>();
            services.AddTransient<IChatService, ChatService>();
            services.AddTransient<IWishlistService, WishlistService>();
            services.AddTransient<IViewedlistService, ViewedlistService>();
            services.AddTransient<IProductFeedbackService, ProductFeedbackService>();

            services.AddSignalR();


            services.AddMvc().AddJsonOptions(options=>options.SerializerSettings.ContractResolver =new DefaultContractResolver());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,DbInitializer dbInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseImageResizer();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseAuthentication();
            app.UseSignalR(route=>
            {
                route.MapHub<ChatHub>("/Chatter");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(name: "areaRoute",
                    template: "{area:exists}/{controller=Login}/{action=Index}/{id?}");
            });
            dbInitializer.Seed().Wait();
        }
    }
}
