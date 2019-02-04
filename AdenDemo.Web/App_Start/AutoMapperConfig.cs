using AutoMapper;
using System.Reflection;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(AdenDemo.Web.AutoMapperConfig), "Configure")]
namespace AdenDemo.Web
{
    public static class AutoMapperConfig
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfiles(Assembly.GetExecutingAssembly());
                //cfg.ForAllMaps((map, exp) => exp.ForAllOtherMembers(opt => opt.Ignore()));
            });

        }

    }
}
