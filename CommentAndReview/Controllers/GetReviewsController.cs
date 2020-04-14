using CommentAndReview.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
//System.IO.File;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using static CommentAndReview.Models.json2csharp;

namespace CommentAndReview.Controllers
{
    public class GetReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private Rootobject ExtractedResponse;

        public GetReviewsController()
        {
            _context = new ApplicationDbContext();
        }
       // GET: Comment
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetYoutubeReviews(Models.Comment Url)
        {
            //getting the url splitted in order to get the videoID
            var resp = Url.Submit;
            var resp1 = resp.Split('=');
            var SplittedUrl = resp1;
            var Video_ID = SplittedUrl[1];

            //Api key hardcoded in appsetting  
            var API_KEY = ConfigurationManager.AppSettings["key"].ToString();
           
            //consuming an external Api eg Youtube
            var client = new RestClient 
                ("https://www.googleapis.com/youtube/v3/");

            var request = new RestRequest($"commentThreads?part=snippet,replies&videoId={ Video_ID}&key={API_KEY}", Method.GET);

            var queryResult = client.Execute(request);
            if (queryResult.IsSuccessful)
            {
                //deserialize the response data
                var ResponseObject = JsonConvert.DeserializeObject<Rootobject>(queryResult.Content);

                ExtractedResponse = ResponseObject;
            }
            else
            {
                throw new Exception("An Error Occured...");  
            }

            // before Loop
            var Csvcontent = new StringBuilder();
            Csvcontent.AppendLine(" Username, Date, StarRating, ReviewOrComment, Link ");

            for (int i = 0; i == 0; i++)
            {
                for (int val = 0; val < ExtractedResponse.items.Length; val++)
                {
                    var AuthorDisplayName = ExtractedResponse.items[val].snippet.topLevelComment.snippet.authorDisplayName;
                    var AuthorChannelUrl = ExtractedResponse.items[val].snippet.topLevelComment.snippet.authorChannelUrl;
                    var UserComment = ExtractedResponse.items[val].snippet.topLevelComment.snippet.textDisplay;
                    var DatePublished = ExtractedResponse.items[val].snippet.topLevelComment.snippet.publishedAt;
                    string StarRating = "None";

                    //Inside the loop
                    var newLine = ($"{AuthorDisplayName},{DatePublished},{StarRating},{UserComment},{AuthorChannelUrl}");
                    Csvcontent.AppendLine(newLine);
                } 
            }

            //Database Acesss

            _context.Comments.Add(Url);
            _context.SaveChanges();


            //returns the extracted comments 
            return File(Encoding.UTF8.GetBytes(Csvcontent.ToString()), "text/csv", $"{DateTime.Now.Ticks}.csv");

            

            // return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> GetAmazonReviews()
        {
            var url = "https://www.amazon.com/Cracking-Coding-Interview-Programming-Questions/dp/0984782850/ref=sr_1_1?crid=1ZDVBSSDLN8Y7&keywords=data+structures+and+algorithms&qid=1580369934&sprefix=data+struct%2Caps%2C381&sr=8-1";


            //c# httpclient accept encoding gzip deflate
            //handling the type of request coming from amazon
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));

                //this sends a Get request to the specified Uri and return the response body as a string 
                var htmlContent = await client.GetStringAsync(url); 
                  
                //Regx expression to extaract the html, after inspecting the structure of the way the html is
                var pattern = "<span.*profile-name\">([\\w\\s\\.\\-]+)<\\/span\\>[\\w\\W\\s]*<span.*review-date\"\\>([\\w\\s\\.,\\-]+)<\\/span\\>[\\w\\W\\s]*<div.*reviewText.*content\"\\>[\\W]*<span\\>(.*)<\\/span>";
                var matches = Regex.Matches(htmlContent, pattern);

                //stringbuilder for a csv and append string
                var buffer = new StringBuilder("Profile Name,Date,Comment\n");

                //iterate over the matche and groups contained
                foreach(var match in matches.Cast<Match>())
                {
                    var profileName = match.Groups[1].Value;
                    var dateReviewed = match.Groups[2].Value;
                    var comment = match.Groups[3].Value;

                    buffer.AppendLine($"{profileName} {dateReviewed} {comment}");                    
                }

                //return the file to the browser as a csv
                return File(Encoding.UTF8.GetBytes(buffer.ToString()), "text/csv", $"{DateTime.Now.Ticks}.csv");
            }
        }
    }
}