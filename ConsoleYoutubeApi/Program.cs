using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static ConsoleYoutubeApi.Json;

namespace ConsoleYoutubeApi
{
    class Program
    {
        private static Rootobject ExtractedResponse;

        static void Main(string[] args)
        {
            //WebClient access = new WebClient();
            //access.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials;

            Console.WriteLine("Input a youtube Url below" + Environment.NewLine + "Ürl: ");
            var Url = Console.ReadLine();
            var response = Url.Split('=');
            var splitedurl = response[1];

            //Api Implementation

            var API_KEY = ConfigurationManager.AppSettings["key"].ToString();

            var client = new RestClient
                ("https://www.googleapis.com/youtube/v3");

            var request = new RestRequest($"commentThreads?part=snippet,replies&videoId={splitedurl}&key={API_KEY}", Method.GET);

            var queryResult = client.Execute(request);
            if (queryResult.IsSuccessful)
            {
                //deserialize the response data
                var ResponseObject = JsonConvert.DeserializeObject<Rootobject>(queryResult.Content);

                ExtractedResponse = ResponseObject;

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

                Console.WriteLine("Type in the name of the desired csv file(make sure it ends with .csv)");
                string csv_name = Console.ReadLine();
                //after your loop
                File.WriteAllText($"E:\\{csv_name}", Csvcontent.ToString());
                Console.WriteLine(" File written to Csv file Succesfully ");
                

                //foreach (var x in ResponseObject.items)
                //{
                //    string AuthorChannelUrl = x.replies.comments[0].snippet.authorChannelUrl;
                //    string AuthorDisplayName = x.replies.comments[0].snippet.authorDisplayName;
                //    var DatePublished = x.replies.comments[0].snippet.publishedAt;
                //    string UserComment = x.replies.comments[0].snippet.textOriginal;
                //}

                //Console.WriteLine("AuthorDisplayName = {0}", AuthorDisplayName);
                //Console.WriteLine("AuthorChannelUrl = {0}", AuthorChannelUrl);
                //Console.WriteLine("UserComment = {0}", UserComment);
                //Console.WriteLine("DatePublished = {0}", DatePublished);

                Console.ReadLine();
            }
        }
    }
}
