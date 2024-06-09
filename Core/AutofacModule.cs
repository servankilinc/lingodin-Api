using Autofac;
using Core.CrossCuttingConcerns;
using Core.Utils.Caching;

namespace Core;
public class AutofacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // ******** Interceptors **********
        builder.RegisterType<ValidationInterceptor>();
        builder.RegisterType<CacheInterceptor>();
        builder.RegisterType<CacheRemoveInterceptor>();
        builder.RegisterType<CacheRemoveGroupInterceptor>();
        builder.RegisterType<BusinessExceptionHandlerInspector>();
        builder.RegisterType<DataAccessExceptionHandlerInspector>();

        // ********** Cache Service **********
        builder.RegisterType<CacheService>().As<ICacheService>().SingleInstance();
    }
}