namespace Driven.Persistence
{
    public interface IConflictWith
    {
        bool ConflictsWith(object uncommitted, object committed);
    }
}