namespace Api_OberiChat

open Api_OberiChat
open Api_OberiChat.DataContext
open Npgsql
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.EntityFrameworkCore;

type Startup(configuration: IConfiguration) =
    member _.Configuration = configuration

    // This method gets called by the runtime. Use this method to add services to the container.
    member _.ConfigureServices(services: IServiceCollection) =
        // Add framework services.
        services.AddControllers() |> ignore
        services.AddEntityFrameworkNpgsql()
                .AddDbContext<UserContext>(fun x -> x.UseNpgsql(this.Configuration
        .GetConnectionString("DB_API") )) |> ignore 

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    member _.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =
        if (env.IsDevelopment()) then
            app.UseDeveloperExceptionPage() |> ignore
        app.UseHttpsRedirection()
           .UseRouting()
           .UseAuthorization()
           .UseEndpoints(fun endpoints ->
                endpoints.MapControllers() |> ignore
            ) |> ignore
