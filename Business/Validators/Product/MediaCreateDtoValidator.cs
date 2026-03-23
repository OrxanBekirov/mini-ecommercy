using Business.Dtos.Media;
using Entities.Enum;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Validators.Product
{
    public class MediaCreateDtoValidator : AbstractValidator<MediaCreateDto>
    {
        public MediaCreateDtoValidator()
        {
            RuleFor(x => x.File).NotNull();

            RuleFor(x => x.OwnerType).IsInEnum();

            RuleFor(x => x)
                .Must(x =>
                    (x.OwnerType == MediaOwnerType.User && !string.IsNullOrWhiteSpace(x.OwnerKey)) ||
                    (x.OwnerType != MediaOwnerType.User && x.OwnerId.HasValue && x.OwnerId.Value > 0)
                )
                .WithMessage("OwnerType-ə görə OwnerId və ya OwnerKey verilməlidir.");
        }
    }
}
