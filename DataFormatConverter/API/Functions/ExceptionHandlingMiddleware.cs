using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataFormatConverter.API.Functions
{
    /// <summary>
    /// Middleware to handle unhandled exceptions globally.
    /// </summary>
    public class ExceptionHandlingMiddleware : IFunctionsWorkerMiddleware
    {
        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            var logger = context.GetLogger<ExceptionHandlingMiddleware>();
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception occurred.");

                var httpRequest = await context.GetHttpRequestDataAsync();

                if (httpRequest != null)
                {
                    var response = httpRequest.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
                    await response.WriteStringAsync("Internal Server Error. Please check logs for details.");
                    context.GetInvocationResult().Value = response;
                }
            }
        }
    }
}
