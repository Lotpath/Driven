using System;

namespace Driven
{
    public abstract class RootEntityBase<TRootEntity> : IRootEntity
        where TRootEntity : RootEntityBase<TRootEntity>
    {
        private Guid _id;
        private int _version;

        Guid IMemento.Id { get { return _id; } set { _id = value; } }
        int IMemento.Version { get { return _version; } set { _version = value; } }

        public Guid Id { get { return _id; } }
        public int Version { get { return _version; } }

        void IRootEntity.Mutate(object e)
        {
            RedirectToWhen.InvokeEventOptional((TRootEntity)this, e);
        }
    }
}