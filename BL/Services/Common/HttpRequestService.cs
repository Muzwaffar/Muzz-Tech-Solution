using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Common
{
    public class HttpRequestService
    {
        public string Call(string method, string url, object data, Dictionary<string, string> headers = null, bool ignoreHttps = false)
        {//generic api call method

            string responseString = string.Empty;
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            request.Timeout = 5 * 60 * 1000;
            request.Method = method;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:2.0) Gecko/20100101 Firefox/4.0";
            request.Accept = "*/*";
            request.KeepAlive = true;
            if (ignoreHttps) request.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true; //force validation through invalid certificate

            if (method.ToUpper() != "GET")
                request.ContentType = "application/json";

            //add headers
            if (headers != null)
            {
                foreach (var val in headers)
                {
                    request.Headers.Add(val.Key, val.Value);
                }
            }

            if (data != null && method.ToUpper() != "GET")
            {
                //var req = new { userToken = "", oneTimeToken = "", data = data};
                String jsData = JsonConvert.SerializeObject(data);
                byte[] body = Encoding.UTF8.GetBytes(jsData);

                request.ContentLength = body.Length;

                //Sending
                using (System.IO.Stream stream = request.GetRequestStream())
                {
                    stream.Write(body, 0, body.Length);
                }

            }
            else
            {
                request.ContentLength = 0;
            }

            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream());
                responseString = reader.ReadToEnd();

            }



            return responseString;



        }
        public string ApiCall(string method, string url, dynamic data, dynamic headers = null)
        {

            string responseString = string.Empty;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;



            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:2.0) Gecko/20100101 Firefox/4.0";
            request.Accept = "*/*";
            request.KeepAlive = true;

            if (method.ToUpper() != "GET")
                request.ContentType = "application/json";


            if (headers != null)
            {

                if (headers.Count == 1)
                    request.Headers.Add("SESSION_TYPE", headers[0]);
                else
                {
                    request.Headers.Add("USER_TOKEN", headers[0]);
                    request.Headers.Add("ONE_TIME_TOKEN", headers[1]);
                    request.Headers.Add("SESSION_TYPE", headers[2]);
                }

            }

            if (data != null)
            {
                String jsData = JsonConvert.SerializeObject(data);
                byte[] body = Encoding.ASCII.GetBytes(jsData);

                request.ContentLength = body.Length;

                //Sending
                using (System.IO.Stream stream = request.GetRequestStream())
                {
                    stream.Write(body, 0, body.Length);
                }

            }
            else
            {
                request.ContentLength = 0;
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream());
                responseString = reader.ReadToEnd();

            }



            return responseString;



        }
    }
}
