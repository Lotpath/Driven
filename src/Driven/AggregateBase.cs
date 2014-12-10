using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Driven
{
    public abstract class AggregateBase<TRootEntity> : AggregateBase
       where TRootEntity : IRootEntity
    {
        protected TRootEntity RootEntity { get; private set; }

        protected AggregateBase(TRootEntity rootEntity)
        {
            RootEntity = rootEntity;
            Id = rootEntity.Id;
            Version = rootEntity.Version;
        }

        protected override Type GetRootEntityType()
        {
            return typeof (TRootEntity);
        }

        protected override IRootEntity GetRootEntity()
        {
            return RootEntity;
        }

        protected void Register<TEvent>()
        {
            Register<TEvent>(e => ((IRootEntity)RootEntity).Mutate(e));
        }
    }

    public abstract class AggregateBase : IAggregate, IEquatable<IAggregate>
    {
        private readonly ICollection<object> _uncommittedEvents = new LinkedList<object>();
        private IRouteEvents _registeredRoutes;

        public object Id { get; protected set; }
        public int Version { get; protected set; }

        private IRouteEvents RegisteredRoutes
        {
            get
            {
                return _registeredRoutes ?? (_registeredRoutes = new EventRouter(this));
            }
            set
            {
                if (value == null)
                    throw new InvalidOperationException("AggregateBase must have an event router to function");

                _registeredRoutes = value;
            }
        }

        protected void Register<T>(Action<T> route)
        {
            this.RegisteredRoutes.Register(route);
        }

        protected void RaiseEvent(object @event)
        {
            ((IAggregate)this).ApplyEvent(@event);
            this._uncommittedEvents.Add(@event);
        }
        void IAggregate.ApplyEvent(object @event)
        {
            this.RegisteredRoutes.Dispatch(@event);
            this.Version++;
        }
        ICollection IAggregate.GetUncommittedEvents()
        {
            return (ICollection)this._uncommittedEvents;
        }
        void IAggregate.ClearUncommittedEvents()
        {
            this._uncommittedEvents.Clear();
        }

        IRootEntity IAggregate.GetRootEntity()
        {
            var snapshot = this.GetRootEntity();
            snapshot.Id = this.Id;
            snapshot.Version = this.Version;
            return snapshot;
        }

        Type IAggregate.RootEntityType { get { return GetRootEntityType(); } }

        protected virtual Type GetRootEntityType()
        {
            return null;
        }

        protected virtual IRootEntity GetRootEntity()
        {
            return null;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return this.Equals(obj as IAggregate);
        }
        public virtual bool Equals(IAggregate other)
        {
            if (ReferenceEquals(this, other)) return true;
            return null != other && other.Id == this.Id;
        }

        private interface IRouteEvents
        {
            void Register<T>(Action<T> handler);
            void Dispatch(object eventMessage);
        }

        private class EventRouter : IRouteEvents
        {
            private readonly IDictionary<Type, Action<object>> _handlers = new Dictionary<Type, Action<object>>();
            private IAggregate _registered;


            public EventRouter(IAggregate aggregate)
            {
                Register(aggregate);
            }

            public void Register<T>(Action<T> handler)
            {
                if (handler == null)
                    throw new ArgumentNullException("handler");

                this.Register(typeof(T), @event => handler((T)@event));
            }

            private void Register(IAggregate aggregate)
            {
                if (aggregate == null)
                    throw new ArgumentNullException("aggregate");

                this._registered = aggregate;

                // Get instance methods named Apply with one parameter returning void
                var applyMethods = aggregate.GetType()
                                            .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                            .Where(m => m.Name == "Apply" && m.GetParameters().Length == 1 && m.ReturnParameter.ParameterType == typeof(void))
                                            .Select(m => new
                                            {
                                                Method = m,
                                                MessageType = m.GetParameters().Single().ParameterType
                                            });

                foreach (var apply in applyMethods)
                {
                    var applyMethod = apply.Method;
                    this._handlers.Add(apply.MessageType, m => applyMethod.Invoke(aggregate, new[] { m as object }));
                }
            }

            public void Dispatch(object eventMessage)
            {
                if (eventMessage == null)
                    throw new ArgumentNullException("eventMessage");

                Action<object> handler;
                if (this._handlers.TryGetValue(eventMessage.GetType(), out handler))
                    handler(eventMessage);
                else 
                    this._registered.ThrowHandlerNotFound(eventMessage);
            }

            private void Register(Type messageType, Action<object> handler)
            {
                this._handlers[messageType] = handler;
            }
        }
    }
}