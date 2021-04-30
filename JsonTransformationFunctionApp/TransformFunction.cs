using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using JUST;

namespace JsonTransformationFunctionApp
{
    public static class TransformFunction
    {
        [FunctionName("TransformFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "transform/{mappingFile}")] HttpRequest req, string mappingFile,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            //read input from JSON file
            var content = await new StreamReader(req.Body).ReadToEndAsync();

            //read the transformer from a JSON file

            BlobServiceClient blobServiceClient = new BlobServiceClient(System.Environment.GetEnvironmentVariable("AzureWebJobsStorage"));

            //Create a unique name for the container
            string containerName = "mapping-files";

            // Create the container and return a container client object
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient($"{mappingFile}.json");

            // Download the blob's contents and save it to a file
            BlobDownloadInfo download = await blobClient.DownloadAsync();

            string mappingjson;
            using (MemoryStream downloadmemoryStream = new MemoryStream())
            {
                await download.Content.CopyToAsync(downloadmemoryStream);
                downloadmemoryStream.Position = 0;
                mappingjson = await new StreamReader(downloadmemoryStream).ReadToEndAsync();
            }

            // do the actual transformation [equal to new JsonTransformer<JsonPathSelectable>(...) for backward compatibility]
            string transformedString = new JsonTransformer().Transform(mappingjson, content);

            return new OkObjectResult(transformedString);
        }
    }
}
