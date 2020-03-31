using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace FitbitApiLibrary.Helpers
{
	internal class WebRequestHelper
	{
		internal static string GetDataRequestWebRequest(string url, string accessToken)
		{
			string result = "";
			HttpWebRequest webRequest = WebRequest.CreateHttp(url);
			webRequest.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {accessToken}");
			webRequest.ContentType = "application/x-www-form-urlencoded";
			webRequest.ProtocolVersion = HttpVersion.Version10;
			webRequest.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg";
			webRequest.Referer = "https://www.fitbit.com/";
			webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.0; rv:9.0.1) Gecko/20100101 Firefox/9.0.1";
			webRequest.Method = "GET";
			var webResponse = webRequest.GetResponse();
			var streamReader = new StreamReader(webResponse.GetResponseStream());
			result = streamReader.ReadToEnd();
			streamReader.Close();
			webResponse.Dispose();
			return result;
		}
	}
}
