//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace CommentAndReview.Models
//{
//    public class Logic
//    {
//        private IRestResponse Send(dynamic RequestBody)

//        {

//            string Url = Gadget.GetConfigurationSetting("ClickatellApiBaseUrl") + "/wa/messages";  //  /wa/messages is the POST URL to send msgs 

//            string ApiKey = Gadget.GetConfigurationSetting("ClickatellApiKey");

//            IRestResponse resp = null;

//            try

//            {

//                var client = new RestSharp.RestClient(Url);

//                var request = new RestRequest(Method.POST);

//                request.AddHeader("ContentType", "application/json");

//                request.AddHeader("Authorization", ApiKey);

//                request.RequestFormat = DataFormat.Json;

//                request.AddJsonBody(RequestBody);

//                //Task.Factory.StartNew(() => Mylogger.Info("Sent msg to::"+RequestBody.messages[0].to + JsonConvert.SerializeObject(RequestBody)));

//                Mylogger.Info("Sent msg to::" + JsonConvert.SerializeObject(RequestBody));

//                resp = client.Execute(request);

//                return resp;

//            }

//            catch (Exception ex)

//            {

//                Mylogger.Info("Error while exe-ing send msg::" + ex);

//                return resp;

//            }

//        }

//    }
//}