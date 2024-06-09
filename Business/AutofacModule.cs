using Autofac;
using Autofac.Extras.DynamicProxy;
using Business.Abstract;
using Business.Concrete;
using Core.CrossCuttingConcerns;

namespace Business;
public class AutofacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {

        // ******** Services **********
        builder.RegisterType<OTPService>().As<IOTPService>()
            .EnableInterfaceInterceptors()
            .InterceptedBy(typeof(BusinessExceptionHandlerInspector))
            .InstancePerLifetimeScope();
        
        builder.RegisterType<MailService>().As<IMailService>()
            .EnableInterfaceInterceptors()
            .InterceptedBy(typeof(BusinessExceptionHandlerInspector))
            .InstancePerLifetimeScope();
        
        builder.RegisterType<FileService>().As<IFileService>()
            .EnableInterfaceInterceptors()
            .InterceptedBy(typeof(BusinessExceptionHandlerInspector))
            .InstancePerLifetimeScope();

        builder.RegisterType<TokenService>().As<ITokenService>()
            .EnableInterfaceInterceptors()
            .InterceptedBy(typeof(BusinessExceptionHandlerInspector))
            .InstancePerLifetimeScope();

        builder.RegisterType<OAuthService>().As<IOAuthService>()
            .EnableInterfaceInterceptors()
            .InterceptedBy(typeof(BusinessExceptionHandlerInspector))
            .InstancePerLifetimeScope();

        builder.RegisterType<AuthService>().As<IAuthService>()
            .EnableInterfaceInterceptors()
            .InterceptedBy(typeof(ValidationInterceptor), typeof(BusinessExceptionHandlerInspector))
            .InstancePerLifetimeScope();

        builder.RegisterType<UserService>().As<IUserService>()
            .EnableInterfaceInterceptors()
            .InterceptedBy(typeof(ValidationInterceptor), typeof(BusinessExceptionHandlerInspector), typeof(CacheRemoveInterceptor), typeof(CacheRemoveGroupInterceptor), typeof(CacheInterceptor))
            .InstancePerLifetimeScope();

        builder.RegisterType<RoleService>().As<IRoleService>()
            .EnableInterfaceInterceptors()
            .InterceptedBy(typeof(ValidationInterceptor), typeof(BusinessExceptionHandlerInspector), typeof(CacheRemoveInterceptor), typeof(CacheRemoveGroupInterceptor), typeof(CacheInterceptor))
            .InstancePerLifetimeScope();

        builder.RegisterType<CategoryService>().As<ICategoryService>()
            .EnableInterfaceInterceptors()
            .InterceptedBy(typeof(ValidationInterceptor), typeof(BusinessExceptionHandlerInspector), typeof(CacheRemoveInterceptor), typeof(CacheRemoveGroupInterceptor), typeof(CacheInterceptor))
            .InstancePerLifetimeScope();

        builder.RegisterType<WordService>().As<IWordService>()
            .EnableInterfaceInterceptors()
            .InterceptedBy(typeof(ValidationInterceptor), typeof(BusinessExceptionHandlerInspector), typeof(CacheRemoveInterceptor), typeof(CacheRemoveGroupInterceptor), typeof(CacheInterceptor))
            .InstancePerLifetimeScope();
    }
}