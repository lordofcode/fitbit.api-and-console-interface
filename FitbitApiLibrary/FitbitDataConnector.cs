using FitbitApiLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace FitbitApiLibrary
{
	public static class FitbitDataConnector
	{
		private static string DoWebRequest(string accessToken, string url)
		{
			return WebRequestHelper.GetDataRequestWebRequest(url, accessToken);
		}

		public static string GetProfile(string accessToken, List<KeyValuePair<string, string>> values)
		{
			return DoWebRequest(accessToken, ReplaceUrlWithVariables(FitbitConstants.GetUserProfileUrl,values));
		}

		public static string GetBadges(string accessToken, List<KeyValuePair<string, string>> values)
		{
			return DoWebRequest(accessToken, ReplaceUrlWithVariables(FitbitConstants.GetUserBadgesUrl,values));
		}

		public static string GetActivityTrackerSteps(string accessToken, List<KeyValuePair<string, string>> values)
		{
			return DoWebRequest(accessToken, ReplaceUrlWithVariables(FitbitConstants.GetActivityTrackerStepsUrl, values));
		}

		public static string GetActivityTrackerDistance(string accessToken, List<KeyValuePair<string, string>> values)
		{
			return DoWebRequest(accessToken, ReplaceUrlWithVariables(FitbitConstants.GetActivityTrackerDistanceUrl, values));
		}

		public static string GetActivityTrackerCalories(string accessToken, List<KeyValuePair<string, string>> values)
		{
			return DoWebRequest(accessToken, ReplaceUrlWithVariables(FitbitConstants.GetActivityTrackerCaloriesUrl, values));
		}

		private static string ReplaceUrlWithVariables(string url, List<KeyValuePair<string,string>> values)
		{
			var matches = Regex.Matches(url, @"\[([a-z\-]+)\]");
			foreach (var match in matches)
			{
				var matchName = match.ToString().TrimStart(new char[] { '[' }).TrimEnd(new char[] { ']'});
				var matchReplacement = values.Where(rec => rec.Key == matchName).FirstOrDefault();
				if (matchReplacement.Key != null)
				{
					url = url.Replace(match.ToString(), matchReplacement.Value);
				}
			}
			return url;
		}
	}
}
