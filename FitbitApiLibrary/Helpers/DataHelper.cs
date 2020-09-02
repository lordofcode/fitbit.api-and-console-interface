using FitbitApiLibrary.DataStructures;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace FitbitApiLibrary.Helpers
{
	public static class DataHelper
	{
		public static UserProfile ConvertStringToUserProfileObject(string profileData)
		{
			return JsonSerializer.Deserialize<UserProfileContainer>(profileData).user;
		}

		public static Badge[] ConvertStringToBadgesObject(string badgeData)
		{
			return JsonSerializer.Deserialize<BadgeContainer>(badgeData).badges;
		}

		public static ActivitiesData ConvertStringToActivitiesObject(string activitiesData)
		{
			return JsonSerializer.Deserialize<ActivitiesData>(activitiesData);
		}

		public static ActivityTrackerStepsData[] ConvertStringToActivityTrackerStepsObject(string activityTrackerStepsData)
		{
			return JsonSerializer.Deserialize<ActivityTrackerStepsContainer>(activityTrackerStepsData).activitiesLogSteps;
		}

		public static ActivityTrackerDistanceData[] ConvertStringToActivityTrackerDistanceObject(string activityTrackerDistanceData)
		{
			return JsonSerializer.Deserialize<ActivityTrackerDistanceContainer>(activityTrackerDistanceData).activitiesLogDistance;
		}
	}
}
