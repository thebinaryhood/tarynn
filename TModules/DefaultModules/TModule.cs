﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Analytics;
using Microsoft.SqlServer.Server;
using MongoDB.Driver;
using TRouter;
#if !__MonoCS__
using System.Speech.Synthesis;
#endif
using WitAI;

namespace TModules.Core
{
    public abstract class TModule
    {
        protected TConsole _logger;

        public Dictionary<string, Action<WitOutcome>> Intents { get; protected set;  } 

        public delegate void Heard(Match message);

        private Dictionary<string, Heard> _allCallbacks = new Dictionary<string, Heard>();
        protected IMongoDatabase _database = null;

        public string ModuleName { get; private set; }
        public ModuleManager Host;

        public List<Route> Routes { get; private set; }

        protected TModule(string name, ModuleManager host)
        {
            Host = host;
            ModuleName = name;
            Intents = new Dictionary<string, Action<WitOutcome>> ();
            _logger = new TConsole (GetType ());
            Routes =new List<Route>();
        }

        protected void AddCallback(string pattern, Heard callback)
        {
            _allCallbacks.Add(pattern, callback);
        }

        /// <summary>
        /// Called by the Manager to see if this module responds to a certain message
        /// </summary>
        /// <param name="message">The message typed in</param>
        public bool RespondTo(WitOutcome outcome)
        {
            foreach (string intent in Intents.Keys)
            {
                if (string.Compare(intent, outcome.Intent, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    // I have a callback that responds to such an intent
                    var callback = Intents[intent];
                    callback(outcome);
                    return true;
                }
            }
            
            return false;
        }

        public void SetDatabase(IMongoDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Gets called after all the modules are loaded into the manager
        /// </summary>
        public virtual void Initialize()
        {
            _logger.DebugFormat("Module {0} is being initialized", ModuleName);
        }

        protected void Fail()
        {
            Host.SpeakEventually("I'm sorry, I wasn't able to get everything I needed from that. Please try again.");
        }

        private void AddRoute(HttpVerb verb, string url, Action<TRequest, TResponse> callback)
        {
            Routes.Add(new Route(verb, string.Format("/{0}/{1}", ModuleName.ToLower(), url), callback));
        }

        protected void Get(string url, Action<TRequest, TResponse> callback)
        {
            AddRoute(HttpVerb.Get, url, callback);
        }

        protected void Post(string url, Action<TRequest, TResponse> callback)
        {
            AddRoute(HttpVerb.Post, url, callback);
        }

        protected void Delete(string url, Action<TRequest, TResponse> callback)
        {
            AddRoute(HttpVerb.Delete, url, callback);
        }

        protected void Patch(string url, Action<TRequest, TResponse> callback)
        {
            AddRoute(HttpVerb.Patch, url, callback);
        }
    }
}
