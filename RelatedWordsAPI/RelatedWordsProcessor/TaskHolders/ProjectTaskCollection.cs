using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using RelatedWordsAPI.Models;

namespace RelatedWordsAPI.RelatedWordsProcessor.TaskHolders
{
    public class ProjectTaskCollection
    {
        private ConcurrentDictionary<int, TaskHolder> _dict;
        private static readonly ProjectTaskCollection _instance = new ProjectTaskCollection();
        // Explicit static constructor to tell C# compiler  
        // not to mark type as beforefieldinit  
        static ProjectTaskCollection()
        {
        }
        private ProjectTaskCollection()
        {
            _dict = new ConcurrentDictionary<int, TaskHolder>();
        }
        public static ProjectTaskCollection Instance
        {
            get
            {
                return _instance;
            }
        }

        public TaskHolder GetProjectTaskOrNull(Project project)
        {
            TaskHolder taskHolder;
            return _dict.TryGetValue(project.ProjectId, out taskHolder) ? taskHolder : null;
        }

        public bool ContainsProjectTask(Project project)
        {
            return _dict.ContainsKey(project.ProjectId);
        }

        public bool TryAddProjectTask(Project project, TaskHolder taskHolder)
        {
            return _dict.TryAdd(project.ProjectId, taskHolder);
        }


        public bool TryAddOrReplaceProjectTask(Project project, Task<Task> newTask, CancellationTokenSource cancellationTokenSource)
        {
            lock(_instance)
            {
                TaskHolder taskHolder = GetProjectTaskOrNull(project);

                if (taskHolder == null)
                {
                    TaskHolder newTaskHolder = new TaskHolder(newTask, cancellationTokenSource);
                    TryAddProjectTask(project, newTaskHolder);
                    return true;
                }
                    
                switch(taskHolder.Task.Result.Status)
                {
                    case TaskStatus.WaitingForActivation:
                        return false;
                    case TaskStatus.Running:
                        return false;
                    default:
                        TaskHolder newTaskHolder = new TaskHolder(newTask, cancellationTokenSource);
                        TryAddProjectTask(project, newTaskHolder);
                        return true;
                }
            }
        }
    }
}
