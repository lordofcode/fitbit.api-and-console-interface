using System;
using System.Collections.Generic;
using System.Text;

namespace FitbitApiLibrary.DataStructures
{
	public class DailyActivityData
	{
        public string activityId;
        public int activityParentId;
        public int calories;
        public string description;
        public double distance;
        public int duration;
        public bool hasStartTime;
        public bool isFavorite;
        public int logId;
        public string name;
        public string startTime;
        public int steps;
    }
}
