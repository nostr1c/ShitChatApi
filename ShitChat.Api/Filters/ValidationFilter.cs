using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ShitChat.Application.DTOs;

namespace ShitChat.Api.Filters;

public class ValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments)
        {
            if (argument.Value == null) continue;

            var type = argument.Value.GetType();

            if (type.IsPrimitive || type == typeof(string) || type == typeof(Guid))
                continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.Value.GetType());
            var validator = _serviceProvider.GetService(validatorType) as IValidator;

            if (validator != null)
            {
                var validationResult = await validator.ValidateAsync(new ValidationContext<object>(argument.Value));

                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToList()
                        );

                    context.Result = new BadRequestObjectResult(ResponseHelper.Error<object>(
                        message: "ErrorValidationFailed",
                        errors: errors,
                        status: StatusCodes.Status400BadRequest
                    ));

                    return;
                }
            }
        }

        await next();
    }
}
