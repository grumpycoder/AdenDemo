using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(AdenDemo.Web.AutoMapperConfig), "Configure")]
namespace AdenDemo.Web
{
    public static class AutoMapperConfig
    {
        public static void Configure()
        {

            //HeroicAutoMapperConfigurator.LoadMapsFromCallerAndReferencedAssemblies(x => x.Name.StartsWith("YourPrefix"));
            //HeroicAutoMapperConfigurator.LoadMapsFromCallerAndReferencedAssemblies();
            //If you run into issues with the maps not being located at runtime, try using this method instead: 
            //HeroicAutoMapperConfigurator.LoadMapsFromAssemblyContainingTypeAndReferencedAssemblies<SomeControllerOrTypeInYourWebProject>();

            Mapper.Initialize(cfg =>
            {
                cfg.AddProfiles(Assembly.GetExecutingAssembly());
            });

        }
        
    }
}
