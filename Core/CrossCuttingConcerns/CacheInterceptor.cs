using Castle.DynamicProxy;
using Core.Utils.Caching;
using Newtonsoft.Json;
using System.Reflection;

namespace Core.CrossCuttingConcerns;
public class CacheInterceptor : IInterceptor
{
    private readonly ICacheService _cacheService;
    public CacheInterceptor(ICacheService cacheService) => _cacheService = cacheService;
    
    
    public void Intercept(IInvocation invocation)
    {
        var methodInfo = invocation.MethodInvocationTarget ?? invocation.Method;
        var attribute = methodInfo.GetCustomAttributes(typeof(CacheAttribute), true).FirstOrDefault() as CacheAttribute;
        if (attribute == null || string.IsNullOrEmpty(attribute.BaseCacheKey)) {
            invocation.Proceed();
            return;
        }

        if (!typeof(Task).IsAssignableFrom(methodInfo.ReturnType))
        {
            if(methodInfo.ReturnType == typeof(void))
            {
                InterceptVoidSync(invocation, attribute);
            }
            else
            {
                InterceptSync(invocation, attribute);
            }
        }
        else 
        {
            if (!methodInfo.ReturnType.IsGenericType)
            {
                invocation.ReturnValue = InterceptAsync(invocation);
            }
            else
            {
                var returnType = methodInfo.ReturnType.GetGenericArguments().FirstOrDefault(); ;
                if (returnType == null) { invocation.ReturnValue = InterceptAsync(invocation); return; }
                var method = GetType().GetMethod(nameof(InterceptAsyncGeneric), BindingFlags.NonPublic | BindingFlags.Instance);
                if (method == null) throw new InvalidOperationException("InterceptAsyncGeneric method not found.");
                var genericMethod = method.MakeGenericMethod(returnType);
                if (genericMethod == null) throw new InvalidOperationException("InterceptAsyncGeneric has not been created.");

                invocation.ReturnValue = genericMethod.Invoke(this, new object[] { invocation, attribute });
            }
        }
    }

    private void InterceptVoidSync(IInvocation invocation, CacheAttribute attribute)
    {
        // on before...
        invocation.Proceed();
        // on success...
        // this method for void methods, already void methods must not use cache attribute
    }

    private void InterceptSync(IInvocation invocation, CacheAttribute attribute)
    {
        // on before...
        var cacheKey = GenerateCacheKey(attribute.BaseCacheKey, invocation.Arguments);
        var resultCache = _cacheService.GetFromCache(cacheKey);
        if (resultCache.IsSuccess)
        {
            var methodInfo = invocation.MethodInvocationTarget ?? invocation.Method;
            var source = JsonConvert.DeserializeObject(resultCache.Source!, methodInfo.ReturnType);
            if (source != null)
            { 
                invocation.ReturnValue = source;
                return;
            }
        }

        invocation.Proceed();
        
        // on success...
        if (invocation.ReturnValue != null)
        {
            _cacheService.AddToCache(cacheKey, attribute.CacheGroupKeys, invocation.ReturnValue);
        }
    }

    private async Task InterceptAsync(IInvocation invocation)
    {
        // on before...
        invocation.Proceed();
        await (Task)invocation.ReturnValue;
        // on success...
        // this method for void async methods, already void methods must not use cache attribute
    }

    private async Task<TResult> InterceptAsyncGeneric<TResult>(IInvocation invocation, CacheAttribute attribute)
    {
        // on before...
        var cacheKey = GenerateCacheKey(attribute.BaseCacheKey, invocation.Arguments);
        var resultCache = _cacheService.GetFromCache(cacheKey);
        if (resultCache.IsSuccess)
        {
            var source = JsonConvert.DeserializeObject<TResult>(resultCache.Source!); 
            if (source != null) return source;
        }

        invocation.Proceed();
        var result = await (Task<TResult>)invocation.ReturnValue;

        // on success...
        if (result != null)
        { 
            _cacheService.AddToCache(cacheKey, attribute.CacheGroupKeys, result);
        }
        return result; 
    }
     

    private string GenerateCacheKey(string baseCacheKey, object[] args)
    {
        if (args.Length > 0) return $"{baseCacheKey}-{string.Join("-", args)}";
        return $"{baseCacheKey}";
    }
}


[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true, Inherited = true)]
public class CacheAttribute : Attribute
{
    public string BaseCacheKey { get; }
    public string[] CacheGroupKeys { get; }
    public CacheAttribute(string baseCacheKey, string[] cacheGroupKeys)
    {
        BaseCacheKey = baseCacheKey;
        CacheGroupKeys = cacheGroupKeys;
    }
}