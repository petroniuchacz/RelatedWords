using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Knyaz.Optimus;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.ScriptExecuting.Jint;
using RelatedWordsAPI.App;


namespace RelatedWordsAPI.Services

{
    public interface IHttpEngine
    {
        Engine Engine { get; }

        Task<string> GetAsync(string url, CancellationToken cancellationToken);

    }
    public class HttpEngine : IHttpEngine
    {

        public Engine Engine
        {
            get;
            private set;
        }

        public HttpEngine(IOptions<AppSettings> appSettings)
        {
            var userAgent = appSettings.Value.HttpUserAgent;

            Engine = EngineBuilder.New()
            .ConfigureResourceProvider(x => x.Http().Notify(
                request => { request.Headers["User-Agent"] = userAgent; },
                response => { /*handle response */}))
            .UseJint()
            .Build();
        }

        public async Task<string> GetAsync(string url, CancellationToken cancellationToken)
        {

            var page = await Engine.OpenUrl(url).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            var document = page.Document;
            return document.Body.OuterHTML;
        }
    }
}
