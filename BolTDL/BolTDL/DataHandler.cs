using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BolTDL
{
    public class DataHandler
    {
        static string curPath;
        static string filePath;
        private static bool exists;
        const string fileName = "save.boltd";
        const string backupFileName = "save.boltd.old";
        const string backupErrorFileName = "backup.boltd";

        public static void Save(ToDoList list)
        {
            SetUp();

            if (exists)
            {
                File.Delete(backupFileName);
                File.Move(fileName, backupFileName);
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

        public static ToDoList Load()
        {
            SetUp();
            ToDoList t = new ToDoList();

            try
            {
                if (exists)
                {
                    string line = "";
                    using (StreamReader reader = File.OpenText(filePath))
                    {
                        while ((line = reader.ReadLine()) != null)
                        {
                            t.AddTask(JsonConvert.DeserializeObject<BolTask>(line));
                        }
                    }
                }
            }
            catch (JsonReaderException e) //Error parsing JSON from save file
            {
                File.Delete(backupErrorFileName);
                File.Move(fileName, backupErrorFileName);
                t.AddTask(new BolTask("There was an error loading your save file, open for more info.", "Please check backup.boltd in your BolTDL folder.\n" +
                    "It should contain the data which was attempted to be loaded. Perhaps you can recover it manually.\n\nThe error message is as follows:\n" + e.Message, TaskPriority.HIGH));
            }

            return t;
        }

        private static void SetUp()
        {
            curPath = Directory.GetCurrentDirectory();
            filePath = Path.Combine(curPath, fileName);
            exists = File.Exists(filePath);
        }
    }
}
