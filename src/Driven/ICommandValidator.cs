using System.Collections.Generic;

namespace Driven
{
    public interface ICommandValidator
    {
        IEnumerable<string> Validate(object command);
    }
}