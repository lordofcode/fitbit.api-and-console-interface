using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FitbitApiLibrary.DataStructures
{
	public class ActivityTrackerStepsContainer
	{
		[JsonPropertyNameAttribute("activities-steps")]
		public ActivityTrackerStepsData[] activitiesLogSteps { get; set; }
	}

	public class ActivityTrackerStepsData : FitbitDataBaseClass
	{
		public string dateTime { get; set; }

		[JsonPropertyNameAttribute("value")]
		public string steps { get; set; }
	}
}
