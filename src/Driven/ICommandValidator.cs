using System.Collections.Generic;

namespace Driven
{
    public interface ICommandValidator
    {
        IEnumerable<string> Validate(object command);
    }

    public class NulloCommandValidator : ICommandValidator
    {
        public IEnumerable<string> Validate(object command)
        {
            return new string[0];
        }
    }
}