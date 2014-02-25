using System;

namespace Driven
{
    public abstract class RootEntityBase : IRootEntity
    {
        private Guid _id;
        private int _version;

        Guid IMemento.Id { get { return _id; } set { _id = value; } }
        int IMemento.Version { get { return _version; } set { _version = value; } }

        public Guid Id { get { return _id; } }
        public int Version { get { return _version; } }
    }
}