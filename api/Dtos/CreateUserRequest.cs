using FluentValidation;
using api.Repositories;


namespace api.Dto
{
    public class CreateUserRequest
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Username { get; set; }
    }

    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        private readonly UserRepository _userRepository;
        private readonly ILogger<CreateUserRequestValidator> _logger;

        public CreateUserRequestValidator(UserRepository repository, ILogger<CreateUserRequestValidator> logger)
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

            RuleFor(x => x.Username)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("ErrorUsernameCannotBeEmpty")
                .Length(3, 50).WithMessage("ErrorUsernameLength")
                .MustAsync(async (username, cancellation) =>
                {
                    bool exists = await _userRepository.UsernameAlreadyExists(username);

                    _logger.LogInformation($"Username check: {username} exists: {exists}");

                    return !exists;
                }).WithMessage("ErrorUsernameAlreadyExist");
        }
    }
}
