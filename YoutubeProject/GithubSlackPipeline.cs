using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http;
using YoutubeProject.Models;

namespace YoutubeProject
{
    public static class GithubSlackPipeline
    {
        [FunctionName("GithubSlackPipeline")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
           
              log.LogInformation("Github-Slack pipeline has been initiated");

              string requestBody = await new StreamReader(req.Body).ReadToEndAsync();   

              var data = JsonConvert.DeserializeObject<GithubHook>(requestBody); 

              var messageToSlack = $"New commit has been pushed. Commit Id {data.commits[0].id} Message: {data.commits[0].message}";
               
              await SendSlackMessage(messageToSlack);

              log.LogInformation("Github-Slack pipekine has completed successfully");

              return new OkObjectResult(messageToSlack);
        }
    
        //Slack hook
        public static async Task<string> SendSlackMessage(string message)
        {
            try
            {            

               var urlWebHook = "https://hooks.slack.com/services/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" ;
           
                using(var client = new HttpClient())
                {
                    var jsonBody = "{'text': '" + message + "'}";

                    var data = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(urlWebHook, data);

                    var result = await response.Content.ReadAsStringAsync();

                    return result;
                    
                }
            }         
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }
        }
    }
}
