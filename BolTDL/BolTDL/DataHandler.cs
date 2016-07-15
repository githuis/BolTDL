using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BolTDL
{
    public class DataHandler
    {
        static string curPath;
        static string filePath;
        private static bool exists;
        const string fileName = "save.boltd";
        const string backupFileName = "save.boltd.old";
        const char splitChar = '\u29ea';

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
                    string s = string.Format("{0}{1}{2}\n", curTask.Title, splitChar, curTask.Description);
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

            if(exists)
            {
                string allInfo = "";
                using (StreamReader reader = File.OpenText(filePath))
                {
                    while ((allInfo = reader.ReadLine()) != null)
                    {
                        string[] x = allInfo.Split(splitChar);
                        if(x.Length >= 2)
                            t.AddTask(new BolTask(x[0], x[1]));
                    }
                }
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
