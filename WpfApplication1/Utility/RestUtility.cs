using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace FutureApplication.Utility
{
    class RestUtility
    {
        String url;
        HttpWebRequest httpRequest;
        HttpWebResponse httpResponse;

        public RestUtility()
        {
            
        }

        public String getTXCurPrice()
        {
            String curPrice = "";
            url = "http://www.taifex.com.tw/quotesapi/getQuotes.aspx?objId=2";
            try
            {
                httpRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                httpRequest.Method = "GET";

                
                httpResponse = (HttpWebResponse)httpRequest.GetResponse();

                StreamReader reader = new StreamReader(httpResponse.GetResponseStream(), Encoding.UTF8);

                JArray jsonArray = JArray.Parse(reader.ReadToEnd());
                
                foreach(JObject jsonObject in jsonArray){
                    
                    if (((String)jsonObject["contract"]).StartsWith("TX"))
                    {
                        curPrice = (String)jsonObject["price"];
                        break;
                    }
                }
                curPrice = curPrice.Replace(",", "");
            }
            catch (Exception e)
            {
                return e.Message;
            }
            finally
            {
                httpRequest.Abort();
                httpResponse.Close();
            }
            return curPrice;
        }
    }
}
