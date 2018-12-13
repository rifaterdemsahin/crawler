#r "Newtonsoft.Json"

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static async Task<IActionResult> Run(HttpRequest req, ILogger log)
{
    log.LogInformation("C# HTTP trigger function processed a request.");

 //   string name = req.Query["name"];

   string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
     log.LogInformation("requestBody received");
   //dynamic data = JsonConvert.DeserializeObject(requestBody);
     log.LogInformation(requestBody);

  //  name = name ?? data?.name;

    // return name != null
    //     ? (ActionResult)new OkObjectResult($"Hello, {name}")
    //     : new BadRequestObjectResult("Please pass a name on the query string or in the request body");

        var links = LinkFinder.Find(requestBody);
        foreach (LinkItem i in links)
        {
            log.LogInformation(i.ToString());
        }

            return (ActionResult)new OkObjectResult(requestBody);
          //  return (ActionResult)new OkObjectResult($"Completed the process2");
}

public struct LinkItem
{
    public string Href;
    public string Text;

    public override string ToString()
    {
        return Href + "\n\t" + Text;
    }
}

static class LinkFinder
{
    public static List<LinkItem> Find(string file)
    {
        List<LinkItem> list = new List<LinkItem>();

        // 1.
        // Find all matches in file.
        MatchCollection m1 = Regex.Matches(file, @"(<a.*?>.*?</a>)",
            RegexOptions.Singleline);

        // 2.
        // Loop over each match.
        foreach (Match m in m1)
        {
            string value = m.Groups[1].Value;
            LinkItem i = new LinkItem();

            // 3.
            // Get href attribute.
            Match m2 = Regex.Match(value, @"href=\""(.*?)\""",
                RegexOptions.Singleline);
            if (m2.Success)
            {
                i.Href = m2.Groups[1].Value;
            }

            // 4.
            // Remove inner tags from text.
            string t = Regex.Replace(value, @"\s*<.*?>\s*", "",
                RegexOptions.Singleline);
            i.Text = t;

            list.Add(i);
        }
        return list;
    }
}
