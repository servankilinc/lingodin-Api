using Castle.DynamicProxy;
using Core.Exceptions;
using System.Reflection;

namespace Core.CrossCuttingConcerns;
public class DataAccessExceptionHandlerInspector : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        var methodInfo = invocation.MethodInvocationTarget ?? invocation.Method;
        var attribute = methodInfo.GetCustomAttributes(typeof(DataAccessExceptionHandlerAttribute), true).FirstOrDefault();
        var classAttribute = methodInfo.DeclaringType?.GetCustomAttributes(typeof(DataAccessExceptionHandlerAttribute), true).FirstOrDefault();
        if (attribute == null && classAttribute == null)
        {
            invocation.Proceed();
            return;
        }

        try
        {
            if (!typeof(Task).IsAssignableFrom(methodInfo.ReturnType))
            {
                invocation.Proceed();
            }
            else if (!methodInfo.ReturnType.IsGenericType)
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

                invocation.ReturnValue = genericMethod.Invoke(this, new object[] { invocation });
            }
        }
        catch (Exception exception)
        {
            throw new DataAccessException(exception.Message);
        }
    }

    private async Task InterceptAsync(IInvocation invocation)
    {
        try
        {
            invocation.Proceed();
            await (Task)invocation.ReturnValue;
        }
        catch (Exception exception)
        {
            throw new DataAccessException(exception.Message);
        }
    }

    private async Task<TResult> InterceptAsyncGeneric<TResult>(IInvocation invocation)
    {
        try
        {
            invocation.Proceed();
            var result = await (Task<TResult>)invocation.ReturnValue;
            return result;
        }
        catch (Exception exception)
        {
            throw new DataAccessException(exception.Message);
        }
    }
}


[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true, Inherited = true)]
public class DataAccessExceptionHandlerAttribute : Attribute
{
}