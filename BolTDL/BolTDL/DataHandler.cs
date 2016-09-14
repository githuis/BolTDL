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

        public static void Save(ToDoList list)
        {
            taskListname = list.Name;
            SetUp(taskListname);

            try
            {
				if(File.Exists(Path.Combine(curPath, taskListname + backupFileName)))
					File.Delete(taskListname + backupFileName);
				if(File.Exists(Path.Combine(curPath, taskListname + fileExtension)))
                	File.Move(taskListname + fileExtension, taskListname + backupFileName);
            }
            catch (System.IO.FileNotFoundException)
            {
				//TODO FIX
                throw;
            }

            using (FileStream stream = File.Create(filePath))
            {
                Byte[] info;
                int offset = 0;
                for (int i = 0; i < list.Length; i++)
                {
                    BolTask curTask = list.GetTaskAt(i);
                    string s = JsonConvert.SerializeObject(list.GetTaskAt(i)) + "\n";
                    info = new UTF8Encoding(true).GetBytes(s);

                    stream.Write(info, 0, info.Length);
                    offset += info.Length;
                }
            }
        }

        public static List<ToDoList> Load()
        {
            LoadSetUp();
            string[] files = Directory.GetFiles(curPath, "*.boltd");

            List<ToDoList> lists = new List<ToDoList>();

            if (files.Length == 0)
            {
                lists.Add(new ToDoList("New Tab"));
                return lists;
            }

            try
            {
                string line;
                for (int i = 0; i < files.Length; i++)
                {
                    line = "";
					lists.Add(new ToDoList(Path.GetFileName(files[i]).Replace(".boltd", "")));
                    using (StreamReader reader = File.OpenText(files[i]))
                    {
                        while((line = reader.ReadLine()) != null)
                        {
                            lists[i].AddTask(JsonConvert.DeserializeObject<BolTask>(line));
                        }
                    }
                }
            }
            catch (JsonReaderException e) //Error parsing JSON from save file
            {
                File.Delete(backupErrorFileName);
                File.Move(fileName, backupErrorFileName);
                lists[0].AddTask(new BolTask("There was an error loading your save file, open for more info.", "Please check backup.boltd in your BolTDL folder.\n" +
                    "It should contain the data which was attempted to be loaded. Perhaps you can recover it manually.\n\nThe error message is as follows:\n" + e.Message, TaskPriority.HIGH));
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
                Byte[] info;
                int offset = 0;

                info = new UTF8Encoding(true).GetBytes(jsonSettings);

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
