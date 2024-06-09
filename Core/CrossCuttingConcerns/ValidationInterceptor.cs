using Castle.DynamicProxy;
using Core.Exceptions;
using FluentValidation;
using FluentValidation.Results;
using System.Reflection;

namespace Core.CrossCuttingConcerns;

public class ValidationInterceptor : IInterceptor
{
    private readonly IServiceProvider _serviceProvider;
    public ValidationInterceptor(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;


    public void Intercept(IInvocation invocation)
    {
        var methodInfo = invocation.MethodInvocationTarget ?? invocation.Method;
        var attribute = methodInfo.GetCustomAttributes(typeof(ValidationAttribute), true).FirstOrDefault() as ValidationAttribute;
        if (attribute == null || attribute.TargetType == null )
        {
            invocation.Proceed();
            return;
        }
 

        if (!typeof(Task).IsAssignableFrom(methodInfo.ReturnType))
        {
            // On before...
            CheckValidation(invocation, attribute.TargetType);
            invocation.Proceed();
            // On success...
        }
        else if (!methodInfo.ReturnType.IsGenericType)
        {
            invocation.ReturnValue = InterceptAsync(invocation, attribute);
        }
        else
        {
            var returnType = methodInfo.ReturnType.GetGenericArguments().FirstOrDefault(); ;
            if (returnType == null) { invocation.ReturnValue = InterceptAsync(invocation, attribute); return; }
            var method = GetType().GetMethod(nameof(InterceptAsyncGeneric), BindingFlags.NonPublic | BindingFlags.Instance);
            if (method == null) throw new InvalidOperationException("InterceptAsyncGeneric method not found.");
            var genericMethod = method.MakeGenericMethod(returnType);
            if (genericMethod == null) throw new InvalidOperationException("InterceptAsyncGeneric has not been created.");

            invocation.ReturnValue = genericMethod.Invoke(this, new object[] { invocation, attribute });
        }
    }
 

    private async Task InterceptAsync(IInvocation invocation, ValidationAttribute attribute)
    {
        // on before...
        CheckValidation(invocation, attribute.TargetType);

        invocation.Proceed();
        await (Task)invocation.ReturnValue;
        // on success...
    }

    private async Task<TResult> InterceptAsyncGeneric<TResult>(IInvocation invocation, ValidationAttribute attribute)
    {
        // on before...
        CheckValidation(invocation, attribute.TargetType);

        invocation.Proceed();
        var result = await (Task<TResult>)invocation.ReturnValue;
        // on success...
        return result;
    }


    private void CheckValidation(IInvocation invocation, Type targetType)
    {
        var request = invocation.Arguments.FirstOrDefault(arg => arg?.GetType() == targetType);
        if (request == null) throw new InvalidOperationException("Request object to validation could not read.");

        var validatorsType = typeof(IEnumerable<>).MakeGenericType(typeof(IValidator<>).MakeGenericType(targetType));
        if (validatorsType == null) throw new InvalidOperationException("ValidatorsType has not been created.");
        var validators = (IEnumerable<IValidator>)_serviceProvider.GetService(validatorsType)!;
        if (validators == null || !validators.Any()) return;


        var contextType = typeof(ValidationContext<>).MakeGenericType(targetType);
        if (contextType == null) throw new InvalidOperationException("contextType has not been created.");

        var context = (IValidationContext)Activator.CreateInstance(contextType, request)!;
        if (context == null) throw new InvalidOperationException("context has not been created.");

        IEnumerable<ValidationFailure> failures = validators
            .Select(validator => validator.Validate(context))
            .Where(result => result.IsValid == false)
            .SelectMany(result => result.Errors)
            .ToList();

        if (failures.Any()) throw new ValidationException(failures);
    }
}


[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true, Inherited = true)]
public class ValidationAttribute : Attribute
{
    public Type TargetType { get; }
    public ValidationAttribute(Type targetType) => TargetType = targetType;
}