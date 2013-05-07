﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using Tarynn.Sql;
using Analytics;
using TScript;

namespace Tarynn.Core
{
    public class Tarynn
    {
        Dictionary<string, SpecialResponse> specialResponses = new Dictionary<string, SpecialResponse>();
        FastStatement allStatements = new FastStatement();


        public Tarynn()
        {
            LoadDatabase();         
            LoadSpecialResponses();
        }

        public Query RelateQuery(Query q)
        {
            return null;
        }

        public Query TypeQuery(Query q)
        {
            return null;
        }

        public Query InitialQuery(string queryString)
        {
            Query q = new Query(queryString);

            

            return q;
        }

        public string RunScript(string name)
        {
            Interpreter i = new Interpreter(name);
            i.Validate();
            return i.GetFinalText();
        }

        private void SetInitialGreeting()
        {
            TConsole.Info("Setting initial greeting");
            SpecialResponse r = new SpecialResponse();
            r.Key = "greeting";
            r.Value = "Hello Mark";
            specialResponses.Add("greeting", r);
            SpecialResponse.Insert(r);
        }

        private void LoadDatabase()
        {
            TConsole.Info("Initializing Database");
            SqlManager.SharedInstance.PerformNecessaryMigrations();
        }

        private void LoadSpecialResponses()
        {
            TConsole.Info("Loading special responses");
            SpecialResponse[] responses = (SpecialResponse[])SpecialResponse.All();
            foreach(SpecialResponse r in responses)
            {
                specialResponses.Add(r.Key, r);
            }
            if (!specialResponses.ContainsKey("greeting"))
            {
                SetInitialGreeting();
            }
        }
    }
}
