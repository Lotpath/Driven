using System;
using SubSpec;

namespace Driven.Specs
{
    public static class SpecificationExtensions
    {
        public static ISpecificationPrimitive Do<TException>(this string message, Action act, Action<TException> exceptionHandler)
            where TException : Exception
        {
            return message.Do(() =>
                {
                    try
                    {
                        act();
                    }
                    catch (Exception ex)
                    {
                        if (ex is TException)
                            exceptionHandler(ex as TException);
                        else
                            throw;
                    }
                });
        }
    }
}