using Business.Dtos.Contact;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Validators.Contact
{
    public class ContactSendDtoValidator:AbstractValidator<ContactSendDto>
    {
        public ContactSendDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ad boş ola bilməz.")
                .MaximumLength(100).WithMessage("Ad maksimum 100 simvol ola bilər.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email boş ola bilməz.")
                .EmailAddress().WithMessage("Email formatı düzgün deyil.")
                .MaximumLength(150).WithMessage("Email maksimum 150 simvol ola bilər.");

            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage("Mövzu boş ola bilməz.")
                .MaximumLength(150).WithMessage("Mövzu maksimum 150 simvol ola bilər.");

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Mesaj boş ola bilməz.")
                .MinimumLength(5).WithMessage("Mesaj minimum 5 simvol olmalıdır.")
                .MaximumLength(2000).WithMessage("Mesaj maksimum 2000 simvol ola bilər.");

        }
    }
}
