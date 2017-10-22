using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BolTDL
{
    public class ToDoList
    {
		[JsonProperty("listtitle")]
        public string Name { get; private set; }
        public int Length
        {
            get
            {
                return Tasks.Count;
            }
        }

		public List<BolTask> GetAllTasks
		{
			get
			{
				return Tasks;	
			}
		}

        private List<BolTask> Tasks;

		[JsonConstructor]
        public ToDoList()
        {
            Tasks = new List<BolTask>();
        }

        public ToDoList(string name)
        {
            Tasks = new List<BolTask>();
			Name = name;
        }

		public void SetName(string newName)
		{
			Name = newName;
		}

        public void AddTask(BolTask tsk)
        {
            //TODO Validate input?
            Tasks.Add(tsk);
        }

        public BolTask GetTaskAt(int index)
        {
            return Tasks[index];
        }

		public void DeleteTaskAt (int index)
		{
			Tasks.RemoveAt(index);
		}
    }
}
