using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace RelatedWordsAPI.RelatedWordsProcessor.TaskHolders
{
    public class TaskHolder
    {
        public Task Task { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }

        public TaskHolder () { }

        public TaskHolder(Task task, CancellationTokenSource cancellationTokenSource)
        {
            Task = task;
            CancellationTokenSource = cancellationTokenSource;
        }
    }
}
