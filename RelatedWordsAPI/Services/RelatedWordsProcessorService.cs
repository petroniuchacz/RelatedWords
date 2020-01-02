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

    public class RelatedWordsProcessorService : BackgroundService, IRelatedWordsProcessorService
    {

        private readonly AppSettings _appSettings;
        private readonly RelatedWordsContext _context;
        private readonly IHttpEngine _httpEngine;

        public RelatedWordsProcessorService(IOptions<AppSettings> appSettings, RelatedWordsContext context, IHttpEngine httpEngine)
        {
            _appSettings = appSettings.Value;
            _context = context;
            _httpEngine = httpEngine;
        }

        public bool TryStartProcessing (Project project)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            IProcessProjectTaskGenerator processProjectTaskGenerator = new ProcessProjectTaskGenerator(project, _context, _httpEngine);
            Task task = new Task(async () => processProjectTaskGenerator.ProcessProjectTaskRun(cancellationToken).ConfigureAwait(false));
            bool created = ProjectTaskCollection.Instance.TryAddOrReplaceProjectTask(project, task, cancellationTokenSource);

            if (created)
            {
                TaskStart(task).ConfigureAwait(false);
            }

            return created;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }

        public bool GetProjectTaskStatus(Project project, out TaskStatus taskStatus)
        {
            TaskHolder taskHolder = ProjectTaskCollection.Instance.GetProjectTaskOrNull(project);

            if (taskHolder == null) 
            {
                taskStatus = TaskStatus.WaitingForActivation;
                return false;
            }

            taskStatus = taskHolder.Task.Status;
            return false;
        }

        public bool TryCancelProjectTask(Project project)
        {
            TaskHolder taskHolder = ProjectTaskCollection.Instance.GetProjectTaskOrNull(project);

            if(taskHolder == null)
            {
                return false;
            }

            taskHolder.CancellationTokenSource.Cancel();

            return true;
        }

        private async Task TaskStart (Task task)
        {
            task.Start();
            await task;
        }
    }
}
