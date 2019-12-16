using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using RelatedWordsAPI.Models;

namespace RelatedWordsAPI.RelatedWordsProcessor.TaskHolders
{
    public class ProjectTaskCollection
    {
        private ConcurrentDictionary<int, Task> _dict;
        private static readonly ProjectTaskCollection instance = new ProjectTaskCollection();
        // Explicit static constructor to tell C# compiler  
        // not to mark type as beforefieldinit  
        static ProjectTaskCollection()
        {
        }
        private ProjectTaskCollection()
        {
            _dict = new ConcurrentDictionary<int, Task>();
        }
        public static ProjectTaskCollection Instance
        {
            get
            {
                return instance;
            }
        }

        public Task GetProjectTaskOrNull(Project project)
        {
            Task task;
            return _dict.TryGetValue(project.ProjectId, out task) ? task : null;
        }

        public bool ContainsProjectTask(Project project)
        {
            return _dict.ContainsKey(project.ProjectId);
        }

        public bool TryAddProjectTask(Project project, Task task)
        {
            return _dict.TryAdd(project.ProjectId, task);
        }
    }
}
