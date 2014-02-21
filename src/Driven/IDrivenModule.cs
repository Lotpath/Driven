using System.Collections.Generic;

namespace Driven
{
    public interface IDrivenModule: IHideObjectMembers
    {
        DrivenContext Context { get; }
        IEnumerable<MessageHandler> Handlers { get; }
    }
}