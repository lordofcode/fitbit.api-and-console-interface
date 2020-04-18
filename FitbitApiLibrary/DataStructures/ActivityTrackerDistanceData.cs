using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FitbitApiLibrary.DataStructures
{
	public class ActivityTrackerDistanceContainer
	{
		[JsonPropertyNameAttribute("activities-distance")]
		public ActivityTrackerDistanceData[] activitiesLogDistance { get; set; }
	}

	public class ActivityTrackerDistanceData : FitbitDataBaseClass
	{
		public string dateTime { get; set; }

		[JsonPropertyNameAttribute("value")]
		public string distance { get; set; }
	}
}
