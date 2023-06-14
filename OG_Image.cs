using System.Net;
using System.Runtime.InteropServices;
using System.Text.Encodings.Web;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using static System.Net.Mime.MediaTypeNames;


namespace og_image
{
    public class OG_Image
    {
        private readonly ILogger _logger;

        public OG_Image(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<OG_Image>();
        }

        [Function("OG_Image")]
        async public Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            var input = "World      ";
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
         //   response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            

            string html = @$"<!DOCTYPE html>
<html>
    <meta charset=""utf-8"">
    <title>Generated Image</title>
    <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
    <style>
      *{@"{
box-sizing: border-box;
margin: 0;
padding: 0;
}"}

.container{@"{
display: flex;
flex-direction: column;
justify-content: center;
align-items: center;
height: 100vh;
width: 100;
background-color: pink;
color: black;
}"}



    </style>
    <body><div class='container'> <h1>Hello {input}</h1><p>This is dynamically generated.</p></div></body></html>";


            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync();
            var page = await browser.NewPageAsync();
            await page.SetContentAsync(html);
            await page.SetViewportSizeAsync(800, 200);
            var bytes = await page.ScreenshotAsync();

            var converted = Convert.ToBase64String(bytes);

         
            response.Headers.Add("Content-Type", $"image/png");
            response.Headers.Add("Cache-Control", "public, immutable, no-transform, s-maxage=31536000, max-age=31536000");
            response.WriteBytes(bytes);
            // response.statusCode = 200;
            //  res.setHeader('Content-Type', `image /${ fileType}`);
            //  res.setHeader('Cache-Control', `public, immutable, no-transform, s-maxage=31536000, max-age=31536000`);
            // res.end(file);

           // response.WriteString(converted);

            return response;
        }
    }
}
