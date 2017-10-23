using System;
using System.Collections.Generic;
using BolTDLCore.NetStandard;
using BolTDLCore.NetStandard.Tasks;

namespace BolTDLConsole.NetCore
{
    internal class ClListNavigator : IListNavigator
    {
        private int _currentTab;
        private const string _noticeString = "Unsaved changes - ";

        private string _oldTitle;

        private BolTdlConsoleSettings _settings;
        private NavState _state;

        public List<ToDoList> ListTabs;

        public ClListNavigator(ToDoList todolist)
        {
            list = todolist;
            _state = NavState.InList;
            ListTabs = new List<ToDoList> {todolist};
            _currentTab = 0;
        }

        public ClListNavigator(List<ToDoList> todolists)
        {
            if (todolists.Count > 0)
                list = todolists[0];
            _state = NavState.InList;
            ListTabs = todolists;
            _currentTab = 0;
        }

        private int CurrentTabCount => ListTabs.Count;
        private bool ListIsPopulated => list.Length > 0;

        public string GetUsername => _settings.Username;
        public string GetPassword => _settings.Password;
        public string GetWebHost => _settings.WebAddress;
        public int CurrentTaskIndex { get; set; }

        public ToDoList list { get; set; }

        public void FindTask()
        {
            throw new NotImplementedException();
        }

        public void NextTask()
        {
            if (CurrentTaskIndex + 1 < list.Length)
                CurrentTaskIndex++;
        }

        public void PrevoiusTask()
        {
            if (CurrentTaskIndex > 0)
                CurrentTaskIndex--;
        }

        public void NextTab()
        {
            _currentTab++;
            if (_currentTab >= CurrentTabCount)
                _currentTab = 0;

            list = ListTabs[_currentTab];
        }

        public void GotoNewTab()
        {
            _currentTab = CurrentTabCount - 1;
            list = ListTabs[_currentTab];
        }

        public void PrintList()
        {
            Clear();

            if (list.Length > 0)
            {
                WriteTabs();
                for (var i = 0; i < list.Length; i++)
                {
                    var currentTask = list.GetTaskAt(i);
                    if (i == CurrentTaskIndex)
                        Console.Write(">   ");
                    else
                        Console.Write("    ");
                    Console.WriteLine(currentTask.Title);
                }
            }
            else
            {
                WriteTabs();
                Console.Write("You have no tasks! Press (O) to create a new task");
            }

            Navigate();
        }

        /// <summary>
        ///     Handles the navigation, pretty much the primary class. Needs to be split into smaller methods.
        /// </summary>
        private void Navigate()
        {
            var key = Console.ReadKey().Key;
            Clear();
            Save();

            //Navigation for InList
            if (_state == NavState.InList)
            {
                switch (key)
                {
                    case ConsoleKey.J:
                        NextTask();
                        GoList();
                        break;
                    case ConsoleKey.K:
                        _state = NavState.InList;
                        PrevoiusTask();
                        GoList();
                        break;
                    case ConsoleKey.L when ListIsPopulated:
                        OpenTask();
                        Navigate();
                        break;
                    case ConsoleKey.O:
                        NavAddTask();
                        GoList();
                        break;
                    case ConsoleKey.Q:
                        return;
                    case ConsoleKey.W:
                        if (GetWebHost != null && GetPassword != null && GetUsername != null)
                        {
                            _state = NavState.DisplayingMessage;
                            WebSave();
                            SavedChanges();
                            DisplayMessage("Synced list!");
                        }
                        else
                        {
                            _state = NavState.DisplayingMessage;
                            DisplayMessage(
                                "Failed to sync list, did you configure username, host, password and useWeb? Did you create an account at your host?");
                        }
                        //GoList();
                        break;
                    case ConsoleKey.D:
                        _state = NavState.PendingDelete;
                        PrintList();
                        break;
                    case ConsoleKey.C when ListIsPopulated:
                        var name = list.GetTaskAt(CurrentTaskIndex).Title;
                        list.DeleteTaskAt(CurrentTaskIndex);
                        NavAddTask(name);
                        GoList();
                        break;
                    case ConsoleKey.N:
                        ListTabs.Add(new ToDoList("New unnamed tab"));
                        GotoNewTab();
                        RenameTab();
                        GoList();
                        break;
                    case ConsoleKey.T when _state != NavState.PendingDelete:
                        CurrentTaskIndex = 0;
                        NextTab();
                        GoList();
                        break;
                    case ConsoleKey.R:
                        RenameTab();
                        GoList();
                        break;
                    default:
                        _state = NavState.InList;
                        PrintList();
                        break;
                }
            }
            //Navigation for when a task is open
            else if (_state == NavState.OpenTask)
            {
                if (key == ConsoleKey.Q || key == ConsoleKey.H)
                {
                    GoList();
                }
                else if (key == ConsoleKey.I || key == ConsoleKey.A)
                {
                    //Edit
                    Navigate();
                }
                else if (key == ConsoleKey.J || key == ConsoleKey.K)
                {
                    //Do nothing
                    OpenTask();
                    Navigate();
                }
                else
                {
                    GoList();
                }
            }
            //Navigation for when pending a delete
            else if (_state == NavState.PendingDelete)
            {
                if (key == ConsoleKey.D && ListIsPopulated)
                {
                    DeleteCurrentTask();
                    GoList();
                }
                if (key == ConsoleKey.T)
                {
                    DataHandler.TryDeleteSave(list.Name);
                    ListTabs.RemoveAt(_currentTab);
                    CurrentTaskIndex = 0;
                    NextTab();
                    GoList();
                }
                else
                {
                    GoList();
                }
            }
            //Navigation for when displaying a message
            else if (_state == NavState.DisplayingMessage)
            {
                GoList();
            }
        }

        private void Save()
        {
            DataHandler.ListSave(ListTabs);
        }

        private void WebSave()
        {
            DataHandler.ListSaveWeb(GetWebHost, GetUsername, GetPassword, ListTabs);
        }

        private void GoList()
        {
            _state = NavState.InList;
            PrintList();
        }

        private void OpenTask()
        {
            _state = NavState.OpenTask;
            var currentTask = list.GetTaskAt(CurrentTaskIndex);
            Console.WriteLine("Title");
            Console.WriteLine("    " + currentTask.Title);

            if (_settings.UseDescriptions)
            {
                Console.WriteLine("Description");
                Console.WriteLine("    " + currentTask.Description);
            }
        }

        private void Clear()
        {
            //Windows
            Console.SetCursorPosition(0, 0);

            for (var i = 0; i < Console.WindowHeight; i++)
                Console.Write(new string(' ', Console.WindowWidth - Console.CursorLeft));
            Console.SetCursorPosition(0, 0);
        }

        private void NavAddTask()
        {
            _state = NavState.AddingTask;
            UnsavedChanges();
            Clear();
            Console.Write("Enter new title: ");
            var t = Console.ReadLine();
            if (_settings.UseDescriptions)
            {
                Console.Write("Enter description (optional): ");
                var d = Console.ReadLine();
                if (t == "")
                    return;
                list.AddTask(new BolTask(t, d));

                return;
            }

            list.AddTask(new BolTask(t, ""));
        }

        private void NavAddTask(string oldTaskName)
        {
            _state = NavState.AddingTask;
            UnsavedChanges();
            Clear();
            Console.WriteLine("Enter new title for task: " + oldTaskName);
            var t = Console.ReadLine();
            if (_settings.UseDescriptions)
            {
                Console.Write("Enter description (optional): ");
                var d = Console.ReadLine();
                if (t == "")
                    return;
                list.AddTask(new BolTask(t, d));

                return;
            }

            list.AddTask(new BolTask(t, ""));
        }

        private void DeleteCurrentTask()
        {
            list.DeleteTaskAt(CurrentTaskIndex);
            UnsavedChanges();
            CurrentTaskIndex = 0;
        }

        private void RenameTab()
        {
            _state = NavState.RenamingTab;
            Clear();

            Console.WriteLine("Enter new name for tab " + list.Name);
            var newName = Console.ReadLine();
            if (newName == "")
                newName = "Unnamed tab";
            DataHandler.TryDeleteSave(list.Name);
            UnsavedChanges();
            list.SetName(newName);
            Save();
        }

        private void WriteTabs()
        {
            var oldForeground = Console.ForegroundColor;
            var oldBackground = Console.BackgroundColor;

            for (var i = 0; i < CurrentTabCount; i++)
                if (i == _currentTab)
                {
                    if (_settings.UseColors)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.Write("> ");
                    }

                    Console.Write(ListTabs[i].Name + "   ");

                    if (_settings.UseColors)
                    {
                        Console.ForegroundColor = oldForeground;
                        Console.BackgroundColor = oldBackground;
                    }
                }
                else
                {
                    Console.Write(ListTabs[i].Name + "   ");
                }
            Console.WriteLine("");
        }

        private void DisplayMessage(string message)
        {
            Clear();
            Console.WriteLine(message + "\n\nPress any key to continue...");
            _state = NavState.DisplayingMessage;
            Navigate();
        }

        /// <summary>
        ///     Loads the settings
        /// </summary>
        /// <returns><c>true</c>, if settings was contains userWebSync=true, <c>false</c> otherwise.</returns>
        public bool LoadSettings()
        {
            _settings = BolTdlConsoleSettings.SettingsFromJson(
                DataHandler.ImportSettings(BolTdlConsoleSettings.FileName));
            _settings.ExportSettings();

            return _settings.UserWebSync;
        }

        private void UnsavedChanges()
        {
            if (!_settings.UserWebSync)
                return;

            Console.Title = _noticeString + _oldTitle;
        }

        private void SavedChanges()
        {
            Console.Title = _oldTitle;
        }

        public void SetOldTitle(string old)
        {
            _oldTitle = old;
        }

        private enum NavState
        {
            InList,
            OpenTask,
            AddingTask,
            PendingDelete,
            RenamingTab,
            DisplayingMessage
        }
    }
}