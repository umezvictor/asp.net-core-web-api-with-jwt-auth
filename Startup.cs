using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileManagerApi.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FileManagerApi
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //config db
            // configure dependency injection for DbContext of type UserContext
            services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DBConnection")));

            //enable cors and add to pipeline before mvc
            services.AddCors();
            //addsingleton -- means create a single instance of our repository
            //it create the single instance and reuses it throughout the project
            //it can cause issues for concurrent requests


            //i useed addScoped -- the service is created once per request within the scope 
            //takes in the interface and its concrete implementation
            //it can now be injected inside our controller
            services.AddScoped<IAuthRepository, AuthRepository>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()); //add cors policy
            app.UseMvc();
        }
    }
}
