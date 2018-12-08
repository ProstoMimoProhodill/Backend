﻿using System;
using System.Collections.Generic;
using GraphLabs.Backend.Api.Controllers;
using GraphLabs.Backend.DAL;
using GraphLabs.Backend.Domain;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using Microsoft.OData.Edm;
using ServiceLifetime = Microsoft.OData.ServiceLifetime;

namespace GraphLabs.Backend.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _environment;

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var migrationsAssembly = GetType().Assembly.FullName;
            services.AddDbContext<GraphLabsContext>(
                o => o.UseSqlite("Data Source=GraphLabs.sqlite", b => b.MigrationsAssembly(migrationsAssembly)));

            if (_environment.IsDevelopment())
            {
                services.AddSingleton<InitialData>();
                services.AddCors();
            }
            else
            {
                throw new NotImplementedException("Для продакшна сделаем чуть позже");
            }

            services.AddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>();

            services.AddOData();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void Configure(IApplicationBuilder app)
        {
            if (_environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors(builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseMvc(builder =>
            {
                builder.Select().Expand().Filter().OrderBy().MaxTop(100).Count();

                const string routeName = "odata";
                builder.MapODataServiceRoute(routeName, routeName, routeBuilder => routeBuilder
                    .AddService(ServiceLifetime.Singleton, sp => GetEdmModel())
                    .AddService<IEnumerable<IODataRoutingConvention>>(ServiceLifetime.Singleton,
                        sp => ODataRoutingConventions.CreateDefaultWithAttributeRouting(routeName, builder))
                );
            });
        }
        
        private static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder
            {
                Namespace = "GraphLabs"
            };

            // TaskModules =============================================================================================
            var taskModule = builder.EntitySet<TaskModule>("TaskModules").EntityType;
            taskModule.HasKey(m => m.Id);
            taskModule.HasMany(m => m.Variants);
            
            taskModule.Function(nameof(TaskModulesController.RandomVariant)).Returns<IActionResult>();
            
            var downloadFunc = taskModule.Function(nameof(TaskModulesController.Download));
            downloadFunc.Parameter<string>("path").Required();
            downloadFunc.Returns<IActionResult>();

            // TaskVariants ============================================================================================
            var taskVariant = builder.EntitySet<TaskVariant>("TaskVariants").EntityType;
            taskVariant.HasKey(v => v.Id);
            taskVariant.HasRequired(v => v.TaskModule);

            // Users ===================================================================================================
            var user = builder.EntityType<User>();
            user.HasKey(u => u.Id);
            user.Abstract();

            var teacher = builder.EntitySet<Teacher>("Teachers").EntityType;
            teacher.DerivesFrom<User>();

            var student = builder.EntitySet<Student>("Students").EntityType;
            student.DerivesFrom<User>();
            
            // Unbound operations ======================================================================================
            var downloadImageFunc = builder.Function(nameof(ImagesLibraryController.DownloadImage));
            downloadImageFunc.Parameter<string>("name");
            downloadImageFunc.Returns(typeof(IActionResult));


            builder.EnableLowerCamelCase();
            
            return builder.GetEdmModel();
        }
    }
}