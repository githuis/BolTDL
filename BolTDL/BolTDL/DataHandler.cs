using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace BolTDL
{
    public class DataHandler
    {
        static string curPath;
        static string filePath;
        private static string taskListname;
        const string fileExtension = ".boltd";
        const string fileName = "save.boltd";
        const string backupFileName = ".boltd.old";
        const string backupErrorFileName = "backup.boltd";

		public static void ListSave(List<ToDoList> list)
		{
			SetUp ("MyList");
			using (FileStream stream = File.Create(filePath))
			{
				Byte[] info;
				string s = JsonConvert.SerializeObject(list, Formatting.Indented);
				info = new UTF8Encoding (true).GetBytes (s);

				stream.Write (info, 0, info.Length);
			}
		}

		public static List<ToDoList> ListLoad()
		{
			string currentPath = Directory.GetCurrentDirectory ();
			string[] listFiles = Directory.GetFiles (currentPath, "MyList.boltd");

			List<ToDoList> lists = new List<ToDoList> ();

            if (listFiles == null || listFiles.Length == 0)
            {
                lists.Add(new ToDoList("New tab"));
                return lists;
            }

			string listFile = listFiles?[0];


			if(listFile == "")
			{
				lists.Add (new ToDoList ("New tab"));
				return lists;
			}
				
			try
			{
				string json = "";
				using (StreamReader sr = File.OpenText(listFile))
				{
					string line = "";
					while((line = sr.ReadLine()) != null)
					{
						json += line;
					}
				}

				lists = JsonConvert.DeserializeObject<List<ToDoList>>(json);
			} 
			catch (Exception ex)
			{
				Console.WriteLine ("Errors, damn boi");
				Console.WriteLine (ex.Message);
			}

			return lists;
		}

		public static bool TryDeleteSave(string listName)
		{
			LoadSetUp ();
			string p = Path.Combine (curPath, listName + fileExtension);
			bool exists = File.Exists (p);
			if (exists)
				File.Delete (p);

			return exists && !File.Exists(p);
		}

        private static void SetUp(string taskName)
        {
            curPath = Directory.GetCurrentDirectory();
            filePath = Path.Combine(curPath, taskName + fileExtension);
        }

        private static void LoadSetUp()
        {
            curPath = Directory.GetCurrentDirectory();
        }

        /// <summary>
        /// Use this method to save settings for whatever frontend is using the BolTDL Core
        /// </summary>
        /// <param name="jsonSettings">The settings, in json format</param>
        public static void ExportSettings(string settingsName, string jsonSettings)
        {
            LoadSetUp();
            string file = Path.Combine(curPath, settingsName);

            if (File.Exists(file))
                File.Delete(file);

            using (FileStream stream = File.Create(file))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(jsonSettings);
                stream.Write(info, 0, info.Length);
            }
        }

        public static string ImportSettings(string settingsName)
        {
            LoadSetUp();
            string json = "";

            try
            {
                string line = "";
                string file = Path.Combine(curPath, settingsName);
                if(File.Exists(file))
                {

                    using (StreamReader reader = File.OpenText(file))
                    {
                        while( (line = reader.ReadLine()) != null)
                        {
                            json += line;
                        }
                    }
                }
				else
				{
					string username = Environment.UserName;
					string s = @"{""useColors"":true,""useDescriptions"":true,""username"":""" + username + @"""}";
					return s;
				}
            }
            catch (JsonReaderException e)
            {
                //TODO FIX
                Console.WriteLine("Error loading json, heres the error code: " + e.Message);
                throw;
            }

            return json;
        }
    }
}