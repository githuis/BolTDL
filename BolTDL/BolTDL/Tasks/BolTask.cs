using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BolTDL
{
    public class BolTask
    {
		[JsonProperty("title")]
        public string Title { get; private set; }

		[JsonProperty("description")]
        public string Description { get; private set; }
        public TaskPriority Priority { get; private set; }

        public BolTask(string title, string description)
        {
            Title = title;
            Description = description;
        }

        [JsonConstructor]
        public BolTask(string title, string description, TaskPriority priority)
        {
            Title = title;
            Description = description;
            Priority = priority;
        }

        public void UpdateTitle(string newTitle)
        {
            //Call Validate on title
            Title = newTitle;
        }

        public void UpdateDescripton(string newDescription)
        {
            //Call Validate on desc
            Description = newDescription;
        }

        private bool ValidateTitle(string newTitle)
        {
            throw new NotImplementedException("Task.cs/ValidateTitle: Throw in what requirements the title has.");
        } 

        private bool ValidateDescription(string newDescription)
        {
            throw new NotImplementedException("Task.cs/ValidateDescription: Throw in what requirements the description has.");
        }



    }
}
