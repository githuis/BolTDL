using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BolTDL;

namespace BolTDLConsole
{
    class CLListNavigator : IListNavigator
    {
        public int CurrentTaskIndex
        {
            get
            {
                return _currentTaskIndex;
            }

            set
            {
                _currentTaskIndex = value;
            }
        }

        public ToDoList list
        {
            get
            {
                return _list;
            }

            set
            {
                _list = value;
            }
        }

        private int _currentTaskIndex;
        private ToDoList _list;
        private bool _cursorAtTitle = true;
        private enum NavState { InList, OpenTask, AddingTask, PendingDelete };
        private NavState state;

        public CLListNavigator(ToDoList todolist)
        {
            list = todolist;
            state = NavState.InList;
        }

        public void FindTask()
        {
            throw new NotImplementedException();
        }

        public void NextTask()
        {
            if ((CurrentTaskIndex + 1) < list.Length)
            {
                CurrentTaskIndex++;
            }
        }

        public void PrevoiusTask()
        {
            if (CurrentTaskIndex > 0)
            {
                CurrentTaskIndex--;
            }
        }

        public void PrintList()
        {
            Clear();

            if(list.Length > 0)
            {
                BolTask currentTask;
                for (int i = 0; i < list.Length; i++)
                {
                    currentTask = list.GetTaskAt(i);
                    if (i == CurrentTaskIndex)
                        Console.Write(">   ");
                    else
                        Console.Write("    ");
                    Console.WriteLine(currentTask.Title);
                }

            }
            else
            {
                Console.Write("You have no tasks! Press (O) to create a new task");
            }

            Navigate();

        }

        /// <summary>
        /// Handles the navigation, pretty much the primary class. Needs to be split into smaller methods.
        /// </summary>
        private void Navigate()
        {
            ConsoleKey key = Console.ReadKey().Key;
            Clear();
            Save();

            ////Remove the typed character
            //Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
            //Console.Write(" "); 

            ////Start Writing at the top of the console
            //Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop - list.Length);


            //Navigation for InList
            if(state == NavState.InList)
            {
                if (key == ConsoleKey.J)
                {
                    NextTask();
                    GoList();
                }
                else if (key == ConsoleKey.K)
                {
                    state = NavState.InList;
                    PrevoiusTask();
                    GoList();
                }
                else if (key == ConsoleKey.L)
                {
                    OpenTask();
                    Navigate();
                }
                else if (key == ConsoleKey.O)
                {
                    NavAddTask();
                    GoList();
                }
                else if (key == ConsoleKey.Q)
                {
                    return;
                }
                else if (key == ConsoleKey.W)
                {
                    Save();
                    GoList();
                }
                else if (key == ConsoleKey.D)
                {
                    state = NavState.PendingDelete;
                    GoList();
                }
                else
                {
                    state = NavState.InList;
                    PrintList();
                }
            }
            //Navigation for when a task is open
            else if(state == NavState.OpenTask)
            {
                if(key == ConsoleKey.Q ||key == ConsoleKey.H)
                {
                    GoList();
                }
                else if (key == ConsoleKey.I || key == ConsoleKey.A)
                {
                    //Edit
                    Navigate();
                }
                else
                {
                    GoList();
                }
            }

            
        }

        private void Save()
        {
            DataHandler.Save(list);
        }

        private void GoList()
        {
            state = NavState.InList;
            PrintList();
        }

        private void OpenTask()
        {
            BolTask currentTask = list.GetTaskAt(CurrentTaskIndex);
            Console.WriteLine("Title");
            Console.WriteLine("    " + currentTask.Title);
            Console.WriteLine("Description");
            Console.WriteLine("    " + currentTask.Description);
        }

        private void Clear()
        {
            Console.SetCursorPosition(0, 0);

            for (int i = 0; i < Console.WindowHeight; i++)
            {
                Console.Write(new string(' ', Console.BufferWidth - Console.CursorLeft));
            }
            Console.SetCursorPosition(0, 0);

        }

        private void NavAddTask()
        {
            state = NavState.AddingTask;
            Clear();
            Console.Write("Enter new title: ");
            string t = Console.ReadLine();
            Console.Write("Enter description (optional): ");
            string d = Console.ReadLine();
            list.AddTask(new BolTask(t, d));
        }

    }
}
