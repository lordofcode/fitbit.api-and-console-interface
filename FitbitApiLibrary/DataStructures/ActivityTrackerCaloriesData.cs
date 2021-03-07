using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FitbitApiLibrary.DataStructures
{
	public class ActivityTrackerCaloriesContainer
	{
		[JsonPropertyNameAttribute("activities-calories")]
		public ActivityTrackerCaloriesData[] activitiesLogCalories { get; set; }
	}

	public class ActivityTrackerCaloriesData :FitbitDataBaseClass
	{
		public string dateTime { get; set; }

		[JsonPropertyNameAttribute("value")]
		public string calories { get; set; }
	}
}
