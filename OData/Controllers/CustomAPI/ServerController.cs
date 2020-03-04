using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace OData.Controllers.CustomAPI
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ServerController : ControllerBase
    {
        public ActionResult Update()
        {
            try
            {
                ServerInfo = new UpdateServerInfo { Message = "Updating..." };
                string ip = GetLiveIp();
                bool isUpdated = UpdateFirebase(ip);
                ServerInfo.LastUpdated = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                return Ok(new { result = ServerInfo.Message });
            }
            catch (Exception e)
            {
                return Ok(new { result = e.Message });
            }
            
        }
        public static string GetLiveIp()
        {
            using (var httpClient = new HttpClient())
            {
                return httpClient.GetStringAsync("https://api.ipify.org").GetAwaiter().GetResult();
            }
        }
        public static bool UpdateFirebase(string ip)
        {
            string url = "https://amcrud-69b6c.firebaseio.com/active_server.json";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json; charset=utf-8";
            httpWebRequest.Method = HttpMethods.Put;
            string body = ip;
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(body);

                streamWriter.Write(json);
                streamWriter.Flush();
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            var responseText = "";
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                responseText = streamReader.ReadToEnd();
            }
            ServerInfo.Message = responseText;
            return responseText.Equals(ip);
        }

        public static UpdateServerInfo ServerInfo { get; set; }
        
        public ActionResult Status()
        {
            return Ok(ServerInfo);
        }
    }
    public class UpdateServerInfo
    {
        public string LastUpdated { get; set; }
        public string Message { get; set; }
    }
}