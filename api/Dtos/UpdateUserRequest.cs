using FluentValidation;
using api.Repositories;


namespace api.Dtos
{
    public class UpdateUserRequest
    {
        public int UserId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }

    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        private readonly UserRepository _userRepository;
        private readonly ILogger<UpdateUserRequestValidator> _logger;

        public UpdateUserRequestValidator(UserRepository repository, ILogger<UpdateUserRequestValidator> logger)
        {
            _userRepository = repository;
            _logger = logger;

            RuleFor(x => x.Firstname)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("ErrorFirstnameCannotBeEmpty")
            .Length(1, 50).WithMessage("ErrorFirstnameLength");

            RuleFor(x => x.Lastname)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("ErrorLastnameCannotBeEmpty")
                .Length(1, 50).WithMessage("ErrorLastnameLength");
        }
    }
}
