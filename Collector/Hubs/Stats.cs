using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Collector.Hubs
{
    public class LogProcessor : Hub
    {
        public event EventHandler<OnActionEventArgs> OnAction;

        public LogProcessor()
        {

        }

        public void Process(string session, Dictionary<string, string> data)
        {
            string referer = data.ValueOrDefault("referer");
            string url = data.ValueOrDefault("url");

            if (!string.IsNullOrEmpty(url) && referer != null)
            {
                DispatchAction(session, referer == "" ? "?" : referer);
                DispatchAction(session, url);
                return;
            }

            string action = data.ValueOrDefault("action");
            if (!string.IsNullOrEmpty(action))
            {
                DispatchAction(session, action);
            }
        }

        private void DispatchAction(string session, string actionId)
        {
            if (OnAction != null)
            {
                OnAction(this, new OnActionEventArgs { Id = actionId, Session = session });
            }
        }

        public class OnActionEventArgs : EventArgs
        {
            public string Session { get; set; }
            public string Id { get; set; }
        }
    }

    public static class ext
    {
        public static TValue ValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            TValue val;
            if (dict.TryGetValue(key, out val))
            {
                return val;
            }
            else
            {
                return default(TValue);
            }
        }
    }
}