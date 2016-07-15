using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BolTDL
{
    public class ToDoList
    {
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

        public void AddTask(BolTask tsk)
        {
            //Validate?
            Tasks.Add(tsk);
        }

        public BolTask GetTaskAt(int index)
        {
            return Tasks[index];
        }




    }
}
