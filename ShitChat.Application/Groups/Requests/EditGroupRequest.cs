﻿using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShitChat.Application.Groups.Requests;

public class EditGroupRequest
{
    public required string Name { get; set; }
}

public class EditGroupRequestValidator : AbstractValidator<EditGroupRequest>
{
    public EditGroupRequestValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage("ErrorGroupNameCannotBeEmpty")
            .MinimumLength(2)
                .WithMessage("ErrorGroupNameMinLength");
    }
}
