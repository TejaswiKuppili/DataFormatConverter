using DataFormatConverter.Application.Services;
using DataFormatConverter.Domain.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace DataFormatConverter.API;

/// <summary>
/// Azure Function for converting data between formats using DI and logging.
/// </summary>
public class ConvertDataFunction
{
    private readonly ConversionService _service;
    private readonly ILogger<ConvertDataFunction> _logger;

    public ConvertDataFunction(ConversionService service, ILogger<ConvertDataFunction> logger)
    {
        _service = service;
        _logger = logger;
    }

    [Function("ConvertData")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        _logger.LogInformation("Received request: {RequestBody}", requestBody);

        ConversionRequest request;

        request = JsonSerializer.Deserialize<ConversionRequest>(requestBody);

        // Validate required fields
        if (request == null ||
            string.IsNullOrWhiteSpace(request.inputFormat) ||
            string.IsNullOrWhiteSpace(request.outputFormat) ||
            string.IsNullOrWhiteSpace(request.data))
        {
            var badResp = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResp.WriteStringAsync("InputFormat, OutputFormat, and Data cannot be null or empty.");
            return badResp;
        }

        try
        {
            string result = _service.Convert(request.data, request.inputFormat, request.outputFormat);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync(result);
            return response;
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Conversion error: {Message}", ex.Message);
            var badResp = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResp.WriteStringAsync(ex.Message);
            return badResp;
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Invalid JSON payload.");
            var badResp = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResp.WriteStringAsync("Invalid request payload. JSON could not be parsed.");
            return badResp;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during conversion.");
            var errorResp = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResp.WriteStringAsync("Internal Server Error. Please check logs.");
            return errorResp;
        }
    }
}