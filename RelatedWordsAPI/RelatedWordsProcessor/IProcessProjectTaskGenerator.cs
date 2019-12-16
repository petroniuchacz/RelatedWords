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
    interface IProcessProjectTaskGenerator
    {
        Task ProcessProjectTaskGenerate(Project p, RelatedWordsContext context, HttpEngine httpEngine, CancellationToken cancellationToken);
    }
}
