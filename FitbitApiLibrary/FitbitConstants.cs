using System;
using System.Collections.Generic;
using System.Text;

namespace FitbitApiLibrary
{
	public static class FitbitConstants
	{
		public static string AuthorizationUrl => "https://www.fitbit.com/oauth2/authorize";
		public static string GetTokenUrl => "https://api.fitbit.com/oauth2/token";
		public static string RefreshTokenUrl => "https://api.fitbit.com/oauth2/token";
		public static string GetUserProfileUrl => "https://api.fitbit.com/1/user/[user-id]/profile.json";
		public static string GetUserBadgesUrl => "https://api.fitbit.com/1/user/[user-id]/badges.json";
		public static string GetActivityTrackerStepsUrl => "https://api.fitbit.com/1/user/[user-id]/activities/steps/date/[startdate]/[period].json";
	}
}
