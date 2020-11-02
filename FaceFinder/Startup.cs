using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FaceFinder.Data.Interfaces;
using FaceFinder.Data.Mocks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FaceFinder
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IFaces, MockFace>();//связываем интерфейс с классом, его реализующим
            services.AddTransient<IPersons, MockPerson>();
            services.AddMvc(mvcOtions =>
            {
                mvcOtions.EnableEndpointRouting = false;
            }); 
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            //services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseDeveloperExceptionPage();
            app.UseStatusCodePages();//коды страниц
            app.UseStaticFiles();
            //app.UseMvcWithDefaultRoute();//url адрес который вызывает контроллер по умолчанию

            app.UseRouting();

            //app.Use(async (context, next) =>
            //{
            //    // получаем конечную точку
            //    Endpoint endpoint = context.GetEndpoint();

            //    if (endpoint != null)
            //    {
            //        // получаем шаблон маршрута, который ассоциирован с конечной точкой
            //        var routePattern = (endpoint as Microsoft.AspNetCore.Routing.RouteEndpoint)?.RoutePattern?.RawText;

            //        Debug.WriteLine($"Endpoint Name: {endpoint.DisplayName}");
            //        Debug.WriteLine($"Route Pattern: {routePattern}");

            //        // если конечная точка определена, передаем обработку дальше
            //        await next();//передаем управление следующему компоненту в конвейере, которым в данном случае является EndpointMiddleware.
            //    }
            //    else
            //    {
            //        Debug.WriteLine("Endpoint: null");
            //        // если конечная точка не определена, завершаем обработку
            //        await context.Response.WriteAsync("Endpoint is not defined");
            //    }
            //});
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "2",
            //        pattern: "{controller}/{action}/");
            //    //endpoints.MapControllerRoute(
            //    //    name: "1",
            //    //    pattern: "api/{controller=Face}/{action=List}/{id?}");
            //});
            app.Use(async (context, next) =>
            {
                Debug.WriteLine("path: "+context.Request.Path.Value);
                    await next();
            });
            app.UseMvc(routes =>
            {
                //routes.MapRoute(
                //    name: "default",
                //    template: "{controller=api}/{action=ListPersons}/{id?}");
                routes.MapRoute("index", "/", new { controller = "Home", action = "FindPerson" });
                routes.MapRoute("ListPerson", "person", new { controller = "Home", action = "ListPerson" });
                routes.MapRoute("AddPerson", "person/add", new { controller = "Home", action = "AddPerson" });
                routes.MapRoute("EditPerson", "person/{person_id}", new { controller = "Home", action = "EditPerson" });
                routes.MapRoute("DeletePerson", "person/{person_id}/delete", new { controller = "Home", action = "DeletePerson" });
                routes.MapRoute("ListFace", "person/{person_id}/face", new { controller = "Home", action = "ListFace" });
                routes.MapRoute("AddFace", "person/{person_id}/face/add", new { controller = "Home", action = "AddFace" });
                routes.MapRoute("GetFace", "person/{person_id}/face/{face_id}", new { controller = "Home", action = "GetFace" });
                routes.MapRoute("DeleteFace", "person/{person_id}/face/{face_id}/delete", new { controller = "Home", action = "DeleteFace" });
                routes.MapRoute("empty", "empty", new { controller = "Home", action = "Empty" });

            });
        }
    }
}
