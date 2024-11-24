using api.Models.Reponses;
using FluentValidation.Results;

namespace api.Helpers
{
    public static class ValidationErrorHelper
    {
        public static GenericResponse<T> BuildResponse<T>(ValidationResult validationResult, string message)
        {
            return new GenericResponse<T>
            {
                Errors = validationResult.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Select(error => error.ErrorMessage).ToList()
                    ),
                Message = message
            };
        }
    }
}
