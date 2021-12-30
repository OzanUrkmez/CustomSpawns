using System.Collections.Generic;

namespace CustomSpawns.Data
{
    public interface IValidator<T>
    {
        IList<string> ValidateData(T t);
    }
}