using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using RelatedWordsAPI.RelatedWordsProcessor;
using RelatedWordsAPI.RelatedWordsProcessor.TaskHolders;
using RelatedWordsAPI.Models;
using RelatedWordsAPI.Services;
using RelatedWordsAPI.App;

namespace RelatedWordsAPI.Services
{
    public interface IRelatedWordsProcessorService
    {
        public bool TryStartProcessing(Project project);
        public bool GetProjectTaskStatus(Project project, out TaskStatus taskStatus);
        public bool TryCancelProjectTask(Project project);
    }

    public class RelatedWordsProcessorService : IRelatedWordsProcessorService
    {

        private readonly AppSettings _appSettings;
        private readonly IServiceProvider _serviceProvider;

        public RelatedWordsProcessorService(IOptions<AppSettings> appSettings, IServiceProvider serviceProvider)
        {
            _appSettings = appSettings.Value;
            _serviceProvider = serviceProvider;
        }

        public bool TryStartProcessing(Project project)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            IProcessProjectTaskGenerator processProjectTaskGenerator = new ProcessProjectTaskGenerator(project, _serviceProvider);
            Task<Task> task = new Task<Task>(async () => await processProjectTaskGenerator.ProcessProjectTaskRunAsync(cancellationToken));
            bool created = ProjectTaskCollection.Instance.TryAddOrReplaceProjectTask(project, task, cancellationTokenSource);

            if (created)
            {
                TaskStart(task, processProjectTaskGenerator).ConfigureAwait(false);
            }

            return created;
        }



        public bool GetProjectTaskStatus(Project project, out TaskStatus taskStatus)
        {
            TaskHolder taskHolder = ProjectTaskCollection.Instance.GetProjectTaskOrNull(project);

            if (taskHolder == null)
            {
                taskStatus = TaskStatus.WaitingForActivation;
                return false;
            }

            taskStatus = taskHolder.Task.Result.Status;
            return true;
        }

        public bool TryCancelProjectTask(Project project)
        {
            TaskHolder taskHolder = ProjectTaskCollection.Instance.GetProjectTaskOrNull(project);

            if (taskHolder == null)
            {
                return false;
            }

            taskHolder.CancellationTokenSource.Cancel();

            return true;
        }

        private async Task TaskStart(Task task, IProcessProjectTaskGenerator processProjectTaskGenerator)
        {
            task.Start();
        }
    }
}
