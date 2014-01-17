using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Driven
{
    [Serializable]
    public class CommandValidationException : Exception
    {
        public CommandValidationException(IEnumerable<string> errors) : base(ErrorMessage(errors))
        {
            Errors = errors.ToList();
        }

        protected CommandValidationException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context) { }

        public IEnumerable<string> Errors { get; private set; } 

        private static string ErrorMessage(IEnumerable<string> errors)
        {
            return !errors.Any() ? "" : string.Join(Environment.NewLine, errors);
        }
    }
}