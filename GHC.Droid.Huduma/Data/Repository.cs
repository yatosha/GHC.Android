using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;

namespace GHC.Data
{
    public static class Repository
    {
        private static SQLiteAsyncConnection database;
        /// <summary>
        /// Initialize SQLite database
        /// </summary>
        public static async Task InitializeDatabase()
        {
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "database");
            database = new SQLiteAsyncConnection(dbPath, false);

            await database.CreateTableAsync<RequestVM>();
        }

        internal static async Task<RequestVM> GeRequest(long id)
        {
            CheckDatabase();
            
            //Sale sale = await database.Table<Sale>().Where(x => x.Id == saleId).FirstOrDefaultAsync();
            return await database.GetAsync<RequestVM>(id);
        }

        public static async Task<List<RequestVM>> GetRequests()
        {
            CheckDatabase();

            return await database.Table<RequestVM>().OrderBy(x => x.Id).ToListAsync();
        }

        public static async Task<List<RequestVM>> GetPendingRequests()
        {
            CheckDatabase();

            return await database.Table<RequestVM>().Where(x => x.CompletedTime == null).OrderBy(x => x.Id).ToListAsync();
        }

        public static async Task SaveRequest(RequestVM request)
        {
            CheckDatabase();

            await database.InsertOrReplaceAsync(request);
        }

        private static void CheckDatabase()
        {
            if (database == null)
            {
                string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "database");
                database = new SQLiteAsyncConnection(dbPath, false);
            }
        }
    }
}