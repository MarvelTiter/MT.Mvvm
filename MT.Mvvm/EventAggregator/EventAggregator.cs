using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

namespace MT.Mvvm
{
    public interface IEventAggregator
    {
        void Register<TMessage>(object recipient, Action<TMessage> action);

        void Register<TMessage>(object recipient, object token, Action<TMessage> action);

        void Send<TMessage>(TMessage message);

        void Send<TMessage, TTarget>(TMessage message);

        void Send<TMessage>(TMessage message, object token);

        void Unregister(object recipient);

        void Unregister<TMessage>(object recipient);

        void Unregister<TMessage>(object recipient, object token);

        void Unregister<TMessage>(object recipient, Action<TMessage> action);

        void Unregister<TMessage>(object recipient, object token, Action<TMessage> action);
    }
    public class EventAggregator : IEventAggregator
    {
        private static readonly object CreationLock = new object();
        private static IEventAggregator _defaultInstance;
        private readonly object _registerLock = new object();
        private Dictionary<Type, List<WeakActionAndToken>> _registerActions;
        private bool _isCleanupRegistered;

        public static IEventAggregator Default
        {
            get
            {
                if (_defaultInstance == null)
                {
                    lock (CreationLock)
                    {
                        if (_defaultInstance == null)
                        {
                            _defaultInstance = new EventAggregator();
                        }
                    }
                }

                return _defaultInstance;
            }
        }
        public static void OverrideDefault(IEventAggregator newAggregator)
        {
            _defaultInstance = newAggregator;
        }

        public static void Reset()
        {
            _defaultInstance = null;
        }

        #region IEventAggregator Member
        public void Register<TMessage>(object recipient, Action<TMessage> action)
        {
            Register<TMessage>(recipient, null, action);
        }

        public void Register<TMessage>(object recipient, object token, Action<TMessage> action)
        {
            lock (_registerLock)
            {
                var messageType = typeof(TMessage);

                Dictionary<Type, List<WeakActionAndToken>> recipients;

                if (_registerActions == null)
                {
                    _registerActions = new Dictionary<Type, List<WeakActionAndToken>>();
                }
                recipients = _registerActions;

                lock (recipients)
                {
                    List<WeakActionAndToken> list;

                    if (!recipients.ContainsKey(messageType))
                    {
                        list = new List<WeakActionAndToken>();
                        recipients.Add(messageType, list);
                    }
                    else
                    {
                        list = recipients[messageType];
                    }

                    var weakAction = new WeakAction<TMessage>(action);

                    var item = new WeakActionAndToken
                    {
                        Action = weakAction,
                        Token = token,
                        Recipient = recipient,
                    };

                    list.Add(item);
                }
            }
        }

        public void Send<TMessage>(TMessage message)
        {
            doSend(message, null, null);
        }

        public void Send<TMessage, TTarget>(TMessage message)
        {
            doSend(message, typeof(TTarget), null);
        }

        public void Send<TMessage>(TMessage message, object token)
        {
            doSend(message, null, token);
        }

        public void Unregister(object recipient)
        {

        }

        public void Unregister<TMessage>(object recipient)
        {
            throw new NotImplementedException();
        }

        public void Unregister<TMessage>(object recipient, object token)
        {
            throw new NotImplementedException();
        }

        public void Unregister<TMessage>(object recipient, Action<TMessage> action)
        {
            throw new NotImplementedException();
        }

        public void Unregister<TMessage>(object recipient, object token, Action<TMessage> action)
        {
            throw new NotImplementedException();
        }
        #endregion

        private void doSend<TMessage>(TMessage message, Type messageTargetType, object token)
        {
            var messageType = typeof(TMessage);

            if (_registerActions != null)
            {
                List<WeakActionAndToken> list = null;
                lock (_registerActions)
                {
                    if (_registerActions.ContainsKey(messageType))
                    {
                        list = _registerActions[messageType].Take(_registerActions[messageType].Count()).ToList();
                    }
                }
                if (list != null)
                {
                    SendToList(message, list, messageTargetType, token);
                }
            }
        }

        private static void SendToList<TMessage>(
            TMessage message,
            IEnumerable<WeakActionAndToken> weakActionsAndTokens,
            Type messageTargetType,
            object token)
        {
            if (weakActionsAndTokens != null)
            {
                var list = weakActionsAndTokens.ToList();
                var listClone = list.Take(list.Count()).ToList();

                foreach (var item in listClone)
                {
                    var execute = item.Action as IExecutable;
                    if (execute != null
                        && (messageTargetType == null
                            || item.Recipient.GetType() == messageTargetType
                            || messageTargetType.IsAssignableFrom(item.Recipient.GetType()))
                        && ((item.Token == null && token == null)
                            || item.Token != null && item.Token.Equals(token)))
                    {
                        execute.ExecuteWithObject(message);
                    }
                }
            }
        }
        private static void CleanupList(IDictionary<Type, List<WeakActionAndToken>> lists)
        {
            if (lists == null)
            {
                return;
            }

            lock (lists)
            {
                var listsToRemove = new List<Type>();
                foreach (var list in lists)
                {
                    var recipientsToRemove = list.Value
                        .Where(item => item.Action == null || !item.Action.CheckAlive())
                        .ToList();

                    foreach (var recipient in recipientsToRemove)
                    {
                        list.Value.Remove(recipient);
                    }

                    if (list.Value.Count == 0)
                    {
                        listsToRemove.Add(list.Key);
                    }
                }

                foreach (var key in listsToRemove)
                {
                    lists.Remove(key);
                }
            }
        }
        private void Cleanup()
        {
            CleanupList(_registerActions);
            _isCleanupRegistered = false;
        }
        private void RequireCleanUp()
        {
            foreach (var item in _registerActions.Values)
            {
                foreach (var w in item)
                {
                    if (w.Action is WeakDelegate wd && !wd.CheckAlive())
                    {
                        item.Remove(w);
                    }
                }
            }
            if (!_isCleanupRegistered)
            {
                Action cleanupAction = Cleanup;

                Dispatcher.CurrentDispatcher.BeginInvoke(
                    cleanupAction,
                    DispatcherPriority.ApplicationIdle,
                    null);
                _isCleanupRegistered = true;
            }
        }

        private struct WeakActionAndToken
        {
            public WeakDelegate Action;

            public object Token;

            public object Recipient;
        }
    }
}
