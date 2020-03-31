using FitbitApiLibrary.Enumerations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace FitbitApiLibrary
{
	public static class FitbitOauth2Handler
	{
		/// <summary>
		/// Return URL of the authorizationpage
		/// </summary>
		/// <returns></returns>
		public static string GetAuthorizeUrl(string clientId, ResponseTypeEnumeration responseType, ScopeEnumeration[] scope, Uri redirectUrl, ExpirationEnumeration expires_in, List<PromptEnumeration> prompt, string state, string codeChallenge = "", CodeChallengeMethodEnumeration? codeChallengeMethod = null)
		{
			return $"{FitbitConstants.AuthorizationUrl}?response_type={responseType.ToString()}&client_id={clientId}&redirect_uri={Uri.EscapeDataString(redirectUrl.ToString())}&scope={Uri.EscapeDataString(BuildScopeDataString(scope))}&expires_in={Convert.ToInt32(expires_in)}&code_challenge_method={(codeChallengeMethod != null ? codeChallengeMethod.ToString() : "")}&code_challenge={codeChallenge}";
		}

		/// <summary>
		/// Concat items to a space-seperated string
		/// </summary>
		/// <param name="scope"></param>
		/// <returns></returns>
		private static string BuildScopeDataString(ScopeEnumeration[] scope)
		{
			var scopeParameter = "";
			foreach (var item in scope)
			{
				scopeParameter += item + " ";
			}
			return scopeParameter.Trim();
		}

		public static string GetToken(string clientId, string clientSecret, string code, Uri redirectUrl)
		{
			var dataToSend = $"client_id={clientId}&grant_type=authorization_code&redirect_uri={Uri.EscapeDataString(redirectUrl.ToString())}&code={code}";

			var result = "";
			HttpWebRequest webRequest = WebRequest.CreateHttp(FitbitConstants.GetTokenUrl);
			byte[] credentialsAsBytes = UTF8Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}");
			webRequest.Headers.Add(HttpRequestHeader.Authorization, $"Basic {System.Convert.ToBase64String(credentialsAsBytes)}");
			webRequest.ContentType = "application/x-www-form-urlencoded";
			webRequest.ProtocolVersion = HttpVersion.Version10;
			webRequest.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg";
			webRequest.Referer = "https://www.fitbit.com/";
			webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.0; rv:9.0.1) Gecko/20100101 Firefox/9.0.1";
			webRequest.Method = "POST";
			var streamWriter = new StreamWriter(webRequest.GetRequestStream());
			streamWriter.Write(dataToSend);
			streamWriter.Close();
			var webResponse = webRequest.GetResponse();
			var streamReader = new StreamReader(webResponse.GetResponseStream());
			result = streamReader.ReadToEnd();
			streamReader.Close();
			webResponse.Dispose();
			return result;
		}

		public static string RefreshToken(string clientId, string clientSecret, string refreshToken, ExpirationEnumeration expires_in = ExpirationEnumeration.OneDay)
		{
			var dataToSend = $"grant_type=refresh_token&refresh_token={refreshToken}";

			var result = "";
			HttpWebRequest webRequest = WebRequest.CreateHttp(FitbitConstants.RefreshTokenUrl);
			byte[] credentialsAsBytes = UTF8Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}");
			webRequest.Headers.Add(HttpRequestHeader.Authorization, $"Basic {System.Convert.ToBase64String(credentialsAsBytes)}");
			webRequest.ContentType = "application/x-www-form-urlencoded";
			webRequest.ProtocolVersion = HttpVersion.Version10;
			webRequest.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg";
			webRequest.Referer = "https://www.fitbit.com/";
			webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.0; rv:9.0.1) Gecko/20100101 Firefox/9.0.1";
			webRequest.Method = "POST";
			var streamWriter = new StreamWriter(webRequest.GetRequestStream());
			streamWriter.Write(dataToSend);
			streamWriter.Close();
			var webResponse = webRequest.GetResponse();
			var streamReader = new StreamReader(webResponse.GetResponseStream());
			result = streamReader.ReadToEnd();
			streamReader.Close();
			webResponse.Dispose();
			return result;
		}
	}
}
