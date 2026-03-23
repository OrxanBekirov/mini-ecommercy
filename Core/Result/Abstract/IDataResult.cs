using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Result.Abstract
{
    public interface IDataResult<out T> : IResult
    {
        T Data { get; }
    }
}
