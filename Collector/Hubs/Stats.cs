using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;

namespace Collector.Hubs
{
    public class LogProcessor
    {
        public static readonly LogProcessor instance = new LogProcessor();

        public event EventHandler<OnActionEventArgs> OnAction;

        private LogProcessor()
        { }

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

    public class ActionProcessor
    {
        public static readonly ActionProcessor instance = new ActionProcessor();

        private ActionProcessor()
        { }

        public void Init()
        {
            Observable.FromEventPattern<LogProcessor.OnActionEventArgs>(LogProcessor.instance, "OnAction")
                .GroupBy(a => a.EventArgs.Session)
                .ForEachAsync(ag =>
                {
                    string lastAction = null;
                    ag.ForEachAsync(a =>
                    {
                        Stats.NewAction(lastAction, a.EventArgs.Id);
                        lastAction = a.EventArgs.Id;
                    });
                });
        }
    }

    public class Stats : Hub
    {
        public static void NewAction(string from, string to)
        {
            var ctx = GlobalHost.ConnectionManager.GetHubContext<Stats>();
            ctx.Clients.All.newAction(from, to);
            Debug.WriteLine("new action");
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