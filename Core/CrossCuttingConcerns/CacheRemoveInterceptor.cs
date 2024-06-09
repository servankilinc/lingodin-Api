using Castle.DynamicProxy;
using Core.Utils.Caching;
using System.Reflection;

namespace Core.CrossCuttingConcerns;
public class CacheRemoveInterceptor : IInterceptor 
{
    private readonly ICacheService _cacheService;
    public CacheRemoveInterceptor(ICacheService cacheService) => _cacheService = cacheService;


    public void Intercept(IInvocation invocation)
    {
        var methodInfo = invocation.MethodInvocationTarget ?? invocation.Method;
        var attribute = methodInfo.GetCustomAttributes(typeof(CacheRemoveAttribute), true).FirstOrDefault() as CacheRemoveAttribute;
        if (attribute == null || string.IsNullOrEmpty(attribute.CacheKey))
        {
            invocation.Proceed();
            return;
        }

        if (!typeof(Task).IsAssignableFrom(methodInfo.ReturnType))
        {
            if (methodInfo.ReturnType == typeof(void))
            {
                InterceptVoidSync(invocation, attribute.CacheKey);
            }
            else
            {
                InterceptSync(invocation, attribute.CacheKey);
            }
        }
        else
        {
            if (!methodInfo.ReturnType.IsGenericType)
            {
                invocation.ReturnValue = InterceptAsync(invocation, attribute.CacheKey);
            }
            else
            {
                var returnType = methodInfo.ReturnType.GetGenericArguments().FirstOrDefault(); ;
                if (returnType == null) { invocation.ReturnValue = InterceptAsync(invocation, attribute.CacheKey); return; }
                var method = GetType().GetMethod(nameof(InterceptAsyncGeneric), BindingFlags.NonPublic | BindingFlags.Instance);
                if (method == null) throw new InvalidOperationException("InterceptAsyncGeneric method not found.");
                var genericMethod = method.MakeGenericMethod(returnType);
                if (genericMethod == null) throw new InvalidOperationException("InterceptAsyncGeneric has not been created.");

                invocation.ReturnValue = genericMethod.Invoke(this, new object[] { invocation, attribute.CacheKey });
            }
        }
    }

    private void InterceptVoidSync(IInvocation invocation, string cacheKey)
    {
        // on before...
        invocation.Proceed();
        // on success...
        _cacheService.RemoveFromCache(cacheKey);
    }

    private void InterceptSync(IInvocation invocation, string cacheKey)
    {
        // on before...
        invocation.Proceed();
        // on success...
        _cacheService.RemoveFromCache(cacheKey);
    }

    private async Task InterceptAsync(IInvocation invocation, string cacheKey)
    {
        // on before...
        invocation.Proceed();
        await (Task)invocation.ReturnValue;
        // on success...
        _cacheService.RemoveFromCache(cacheKey);
    }

    private async Task<TResult> InterceptAsyncGeneric<TResult>(IInvocation invocation, string cacheKey)
    {
        // on before...
        invocation.Proceed();
        var result = await (Task<TResult>)invocation.ReturnValue;
        // on success...
        _cacheService.RemoveFromCache(cacheKey);
        return result;
    }
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true, Inherited = true)]
public class CacheRemoveAttribute : Attribute
{
    public string CacheKey { get; }
    public CacheRemoveAttribute(string cacheKey) => CacheKey = cacheKey;
}