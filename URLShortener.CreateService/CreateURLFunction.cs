using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using URLShortener.CreateService.Services;

namespace URLShortener.CreateService
{
  public class CreateURLFunction
  {
    private ICreateURLService _createURLService;
    private ILogger<CreateURLFunction> _logger;
    public CreateURLFunction(ICreateURLService createURLService, ILogger<CreateURLFunction> logger)
    {
      _createURLService = createURLService;
      _logger = logger;
    }

    [Function("CreateURLFunction")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
    {
      try
      {
        //log.LogInformation("C# HTTP trigger function processed a request.");

        string url = req.Query["url"];

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        url = url ?? data?.url;

        var redirectUrl = _createURLService.CreateURL(url);

        return new OkObjectResult(redirectUrl);
      }
      catch (Exception e)
      {
        _logger.LogError(e.ToString());
        return new OkObjectResult(e.ToString());
      }
    }
  }
}