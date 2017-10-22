namespace BolTDLCore.NetStandard
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
