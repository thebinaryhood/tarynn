﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using Tarynn.Sql;

namespace Tarynn.Core
{
    public class Tarynn
    {
        Dictionary<string, SpecialResponse> specialResponses = new Dictionary<string, SpecialResponse>();

        public Tarynn()
        {
            LoadDatabase();         
            LoadSpecialResponses();
        }

        public string SubmitQuery(string queryString)
        {
            return "test";
        }

        private void SetInitialGreeting()
        {
            Console.WriteLine("Setting initial greeting");
            SpecialResponse r = new SpecialResponse();
            r.Key = "greeting";
            r.Value = "Hello Mark";
            specialResponses.Add("greeting", r);
            SpecialResponse.Insert(r);
        }

        private void LoadDatabase()
        {
            Console.WriteLine("Initializing Database");
            SqlManager.SharedInstance.PerformNecessaryMigrations();
        }

        private void LoadSpecialResponses()
        {
            Console.WriteLine("Loading special responses");
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