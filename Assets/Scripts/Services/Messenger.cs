using System;
using System.Collections.Generic;

namespace Services
{
    public interface ISignal
    {
    }

    public class GameMessenger : Messenger<ISignal>
    {
    }

    public class Messenger<TEventBase>
    {
        private readonly Dictionary<Type, object> _subscriptions = new Dictionary<Type, object>();

        public IDisposable Subscribe<TEventType>(Action<TEventType> action, bool highPriority = false)
            where TEventType : TEventBase
        {
            var type = typeof(TEventType);
            LinkedList<Action<TEventType>> subscriptionList;

            if (!_subscriptions.TryGetValue(type, out var packedSubscriptions))
            {
                subscriptionList = new LinkedList<Action<TEventType>>();
                _subscriptions[type] = subscriptionList;
            }
            else
            {
                subscriptionList = (LinkedList<Action<TEventType>>) packedSubscriptions;
            }

            if (subscriptionList.Contains(action))
            {
                return null;
            }

            if (highPriority)
                subscriptionList.AddFirst(action);
            else
                subscriptionList.AddLast(action);

            return new EventSubscription<TEventType>(this, action);
        }

        public void Fire<TEventType>(TEventType eventData = default) where TEventType : TEventBase
        {
            var type = typeof(TEventType);

            if (!_subscriptions.TryGetValue(type, out var packedSubscriptions))
                return;

            var subscriptionList = (LinkedList<Action<TEventType>>) packedSubscriptions;

            var node = subscriptionList.First;
            var endNode = subscriptionList.Last;
            while (node != null)
            {
                var next = node.Next;
                node.Value(eventData);
                if (node == endNode) break;

                node = node.Next ?? next;
            }
        }

        public void Unsubscribe<TEventType>(Action<TEventType> handler) where TEventType : TEventBase
        {
            var type = typeof(TEventType);
            if (!_subscriptions.TryGetValue(type, out var packedSubscriptions))
                return;

            var subscriptionList = (LinkedList<Action<TEventType>>) packedSubscriptions;
            subscriptionList.Remove(handler);
        }

        private void Unsubscribe<TEventType>(EventSubscription<TEventType> eventSubscription)
            where TEventType : TEventBase
        {
            Unsubscribe(eventSubscription.Action);
        }

        private class EventSubscription<TEventType> : IDisposable where TEventType : TEventBase
        {
            private readonly Messenger<TEventBase> _messenger;
            public readonly Action<TEventType> Action;
            private bool _disposed;

            public EventSubscription(Messenger<TEventBase> messenger, Action<TEventType> action)
            {
                Action = action;
                _messenger = messenger;
            }

            public void Dispose()
            {
                if (_disposed) return;

                _messenger.Unsubscribe(this);
                _disposed = true;
            }
        }
    }
}