using System.Collections.Generic;
using BolTDLCore.NetStandard.Tasks;
using Newtonsoft.Json;

namespace BolTDLCore.NetStandard
{
    public class ToDoList
    {
		[JsonProperty("listtitle")]
        public string Name { get; private set; }
        public int Length => Tasks.Count;

        public List<BolTask> GetAllTasks => Tasks;

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
