using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RelatedWordsAPI.Models;
using System.Net.Http;
using RelatedWordsAPI.Services;

namespace RelatedWordsAPI.RelatedWordsProcessor
{
    public interface IProcessProjectTaskGenerator
    {
        Task ProcessProjectTaskRunAsync(CancellationToken cancellationToken);
    }
}
