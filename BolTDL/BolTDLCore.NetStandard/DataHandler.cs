using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BolTDLCore.NetStandard
{
    public class DataHandler
    {
        static string curPath;
        static string filePath;
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

            if (listFiles.Length == 0)
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

        public static List<ToDoList> ListLoadWeb(string host, string username, string password)
        {
            var res = GetSavedDate(host, username, password);
            string json = res.Result;

            if (string.IsNullOrEmpty(json))
            {
                var list = new List<ToDoList>();
                list.Add(new ToDoList("New list"));
                return list;
            }
            else
            {
                return JsonConvert.DeserializeObject<List<ToDoList>>(json);
            }

        }

        public static async Task<string> GetSavedDate(string host, string username, string password)
        {
			try {
				host = ReassureHttpIsInHostAddress(host);
				using (var x = new HttpClient())
				{
					var values = new Dictionary<string, string>
					{
						{"username", username},
						{"password", password}
					};

					var content = new FormUrlEncodedContent(values);
					HttpResponseMessage response = await x.PostAsync(host + "/getdata", content);

					var contents = await response.Content.ReadAsStringAsync();

					return contents;
				}
			} catch (Exception ex) {
				throw new DownloadException ("Error during download, please check your internet connection.");
			}
            
        }

        /// <summary>
        /// returns false if web sync failed, true otherwise
        /// </summary>
        /// <returns></returns>
        public static bool ListSaveWeb(string host, string username, string password, List<ToDoList> list)
        {
            string savedata = JsonConvert.SerializeObject(list, Formatting.Indented);
            var res = SetSaveData(host, username, password, savedata);
            string result = res.Result;

            if(result == "Error" || result ==  "Wrong username/password.")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        
        public static async Task<string> SetSaveData(string host, string username, string password, string savedata)
        {
            host = ReassureHttpIsInHostAddress(host);
            using (var x = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                    {"username", username},
                    {"password", password},
                    {"savedata", savedata}
                };

                var content = new FormUrlEncodedContent(values);
                HttpResponseMessage response = await x.PostAsync(host + "/setdata", content);

                var contents = await response.Content.ReadAsStringAsync();
                return contents;
            }
            
        } 

        private static string ReassureHttpIsInHostAddress(string host)
        {
            //TODO turn into oneliner
            if (!host.Contains("http://") && !host.Contains("https://"))
                return "http://" + host;
            else
                return host;
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