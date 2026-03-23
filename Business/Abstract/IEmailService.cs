using Business.Dtos.Contact;
using Core.Result.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface IEmailService
    {
        Task <IResult> SentContactEmailAsync(ContactSendDto dto);
    }
}
