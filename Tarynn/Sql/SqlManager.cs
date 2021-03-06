﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System.IO;
using Tarynn.Core;
using Analytics;

namespace Tarynn.Sql
{
    class SqlManager
    {
        private TConsole _logger = new TConsole(typeof(SqlManager));

        private static SqlManager instance;

        //connection that stays open until the close command
        SQLiteConnection connection;

        public SqlManager()
        {
            Open();
        }

        public void Open()
        {
            _logger.Info("Checking for database");
            if (!CheckForDatabase())
                CreateDatabase();

            connection = new SQLiteConnection("database.db");
            _logger.Info("Connected to database");
        }

        /// <summary>
        /// A method that includes createTable statements for every database table class
        /// </summary>
        public void PerformNecessaryMigrations()
        {
            _logger.Info("Checking for new migrations");
            //include every class that needs a migration
            connection.CreateTable<Statement>();
            connection.CreateTable<SpecialResponse>();
        }

        /// <summary>
        /// Check to see if the database file exists
        /// </summary>
        /// <returns></returns>
        private bool CheckForDatabase()
        {
            return File.Exists("database.db");
        }

        /// <summary>
        /// Just creates the db file
        /// </summary>
        private void CreateDatabase()
        {
            _logger.Info("Database did not exist. Creating one");
            File.Create("database.db");
        }


        public SQLiteConnection Connection
        {
            get { return connection; }
        }

        /// <summary>
        /// Singleton for a global instance of sql manager 
        /// </summary>
        public static SqlManager SharedInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SqlManager();
                }
                return instance;
            }
        }
    }
}
