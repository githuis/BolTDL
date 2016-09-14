using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BolTDL
{
    public class ToDoList
    {

        public string Name { get; private set; }
        public int Length
        {
            get
            {
                return Tasks.Count;
            }
        }
        private List<BolTask> Tasks;

        public ToDoList()
        {
            Tasks = new List<BolTask>();
        }

        public ToDoList(string name)
        {
            Tasks = new List<BolTask>();
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
