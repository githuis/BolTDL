using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BolTDL
{
    public interface IListNavigator
    {
        int CurrentTaskIndex { get; }
        ToDoList list { get; }

        void NextTask();
        void PrevoiusTask();
        void FindTask(); //One for title and one for description?
    }
}
