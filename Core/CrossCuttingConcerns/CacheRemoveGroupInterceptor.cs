using Castle.DynamicProxy;
using Core.Utils.Caching;
using System.Reflection;

namespace Core.CrossCuttingConcerns;
public class CacheRemoveGroupInterceptor : IInterceptor
{
    private readonly ICacheService _cacheService;
    public CacheRemoveGroupInterceptor(ICacheService cacheService) => _cacheService = cacheService;


    public void Intercept(IInvocation invocation)
    {
        var methodInfo = invocation.MethodInvocationTarget ?? invocation.Method;
        var attribute = methodInfo.GetCustomAttributes(typeof(CacheRemoveGroupAttribute), true).FirstOrDefault() as CacheRemoveGroupAttribute;
        if (attribute == null || attribute.CacheGroupKeys.Length == 0)
        {
            invocation.Proceed();
            return;
        }

        if (!typeof(Task).IsAssignableFrom(methodInfo.ReturnType))
        {
            if (methodInfo.ReturnType == typeof(void))
            {
                InterceptVoidSync(invocation, attribute.CacheGroupKeys);
            }
            else
            {
                InterceptSync(invocation, attribute.CacheGroupKeys);
            }
        }
        else
        {
            if (!methodInfo.ReturnType.IsGenericType)
            {
                invocation.ReturnValue = InterceptAsync(invocation, attribute.CacheGroupKeys);
            }
            else
            {
                var returnType = methodInfo.ReturnType.GetGenericArguments().FirstOrDefault(); ;
                if (returnType == null) { invocation.ReturnValue = InterceptAsync(invocation, attribute.CacheGroupKeys); return; }
                var method = GetType().GetMethod(nameof(InterceptAsyncGeneric), BindingFlags.NonPublic | BindingFlags.Instance);
                if (method == null) throw new InvalidOperationException("InterceptAsyncGeneric method not found.");
                var genericMethod = method.MakeGenericMethod(returnType);
                if (genericMethod == null) throw new InvalidOperationException("InterceptAsyncGeneric has not been created.");

                invocation.ReturnValue = genericMethod.Invoke(this, new object[] { invocation, attribute.CacheGroupKeys });
            }
        }
    }

    private void InterceptVoidSync(IInvocation invocation, string[] cacheGroupKeys)
    {
        // on before...
        invocation.Proceed();
        // on success...
        _cacheService.RemoveCacheGroupKeys(cacheGroupKeys);
    }

    private void InterceptSync(IInvocation invocation, string[] cacheGroupKeys)
    {
        // on before...
        invocation.Proceed();
        // on success...
        _cacheService.RemoveCacheGroupKeys(cacheGroupKeys);
    }

    private async Task InterceptAsync(IInvocation invocation, string[] cacheGroupKeys)
    {
        // on before...
        invocation.Proceed();
        await (Task)invocation.ReturnValue;
        // on success...
        _cacheService.RemoveCacheGroupKeys(cacheGroupKeys);
    }

    private async Task<TResult> InterceptAsyncGeneric<TResult>(IInvocation invocation, string[] cacheGroupKeys)
    {
        // on before...
        invocation.Proceed();
        var result = await (Task<TResult>)invocation.ReturnValue;
        // on success...
        _cacheService.RemoveCacheGroupKeys(cacheGroupKeys);
        return result;
    }
}


[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true, Inherited = true)]
public class CacheRemoveGroupAttribute : Attribute
{
    public string[] CacheGroupKeys { get; }
    public CacheRemoveGroupAttribute(string[] cacheGroupKeys) => CacheGroupKeys = cacheGroupKeys;
}