#r "Newtonsoft.Json"

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static async Task<IActionResult> Run(HttpRequest req, ILogger log)
{
     string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
     log.LogInformation("requestBody received");
     log.LogInformation(requestBody);
     
     var links = LinkFinder.Find(requestBody);
     List<LinkItem> myResult = new List<LinkItem>();

         foreach (LinkItem i in links)
        {
            if(i.Href.StartsWith("https://github.com"))
            {
               var newItem = new LinkItem{Href=i.Href,Text=i.Text};
               myResult.Add(newItem);
            }
       
        }
        string output = JsonConvert.SerializeObject(myResult);

            return (ActionResult)new OkObjectResult(output);
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
