namespace Driven
{
    public abstract class RootEntityBase<TRootEntity> : IRootEntity
        where TRootEntity : RootEntityBase<TRootEntity>
    {
        private object _id;
        private int _version;

        public object Id { get { return _id; } }
        public int Version { get { return _version; } }

        object IRootEntity.Id { get { return _id; } set { _id = value; } }
        int IRootEntity.Version { get { return _version; } set { _version = value; } }

        void IRootEntity.Mutate(object e)
        {
            RedirectToWhen.InvokeEventOptional((TRootEntity)this, e);
        }
    }
}