using Autofac;
using Autofac.Extras.DynamicProxy;
using Core.CrossCuttingConcerns;
using DataAccess.Abstract;
using DataAccess.Concrete;

namespace DataAccess;

public class AutofacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<OTPDal>().As<IOTPDal>()
            .EnableInterfaceInterceptors()
            .InterceptedBy(typeof(DataAccessExceptionHandlerInspector))
            .InstancePerLifetimeScope();

        builder.RegisterType<UserDal>().As<IUserDal>()
            .EnableInterfaceInterceptors()
            .InterceptedBy(typeof(DataAccessExceptionHandlerInspector))
            .InstancePerLifetimeScope();

        builder.RegisterType<RoleDal>().As<IRoleDal>()
            .EnableInterfaceInterceptors()
            .InterceptedBy(typeof(DataAccessExceptionHandlerInspector))
            .InstancePerLifetimeScope();

        builder.RegisterType<UserRoleDal>().As<IUserRoleDal>()
            .EnableInterfaceInterceptors()
            .InterceptedBy(typeof(DataAccessExceptionHandlerInspector))
            .InstancePerLifetimeScope();

        builder.RegisterType<WordDal>().As<IWordDal>()
            .EnableInterfaceInterceptors()
            .InterceptedBy(typeof(DataAccessExceptionHandlerInspector))
            .InstancePerLifetimeScope();

        builder.RegisterType<CategoryDal>().As<ICategoryDal>()
            .EnableInterfaceInterceptors()
            .InterceptedBy(typeof(DataAccessExceptionHandlerInspector))
            .InstancePerLifetimeScope();

        builder.RegisterType<FavoriteDal>().As<IFavoriteDal>()
            .EnableInterfaceInterceptors()
            .InterceptedBy(typeof(DataAccessExceptionHandlerInspector))
            .InstancePerLifetimeScope();

        builder.RegisterType<LearningDal>().As<ILearningDal>()
            .EnableInterfaceInterceptors()
            .InterceptedBy(typeof(DataAccessExceptionHandlerInspector))
            .InstancePerLifetimeScope();
    }
}
