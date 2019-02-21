using System;
using System.Collections.Generic;
// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace MediaPlayer.Generic.Mediator
{
    public static class Mediator<T>
    {
        private static IDictionary<T, Action<object>> _messages = new Dictionary<T, Action<object>>();

        public static void Register(T message, Action<object> callback)
        {
            if (!_messages.ContainsKey(message))
            {
                _messages.Add(message, callback);
            }
        }

        public static void Unregister(T message, Action<object> callback)
        {
            if (_messages.ContainsKey(message))
            {
                _messages.Remove(message);
            }
        }

        public static void NotifyColleagues(T message, object args)
        {
            if (_messages.ContainsKey(message))
            {
                _messages[message].Invoke(args);
            }
        }
    }
}
