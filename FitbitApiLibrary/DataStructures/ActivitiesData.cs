using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FitbitApiLibrary.DataStructures
{
    public class ActivitiesData : FitbitDataBaseClass
    {
        public ActivitiesActivityData[] activities { get; set; }
        public ActivitiesGoalData goals { get; set; }
        public ActivitiesSummaryData summary { get; set; }
    }

    public class ActivitiesActivityData : FitbitDataBaseClass
    {
        public int activityId { get; set; }
        public int activityParentId { get; set; }
        public int calories { get; set; }
        public string description { get; set; }
        public double distance { get; set; }
        public int duration { get; set; }
        public bool hasStartTime { get; set; }
        public bool isFavorite { get; set; }
        public long logId { get; set; }
        public string name { get; set; }
        public string startTime { get; set; }
        public int steps { get; set; }
    }

    public class ActivitiesGoalData : FitbitDataBaseClass
    {
        public int activeMinutes { get; set; }
        public int caloriesOut { get; set; }
        public double distance { get; set; }
        public int floors { get; set; }
        public int steps { get; set; }
    }

    public class ActivitiesSummaryData : FitbitDataBaseClass
    {
        public int activityCalories { get; set; }
        public int caloriesBMR { get; set; }
        public int caloriesOut { get; set; }
        public ActivitiesSummaryDistanceData[] distances { get; set; }
        public double elevation { get; set; }
        public int fairlyActiveMinutes { get; set; }
        public int floors { get; set; }
        public int lightlyActiveMinutes { get; set; }
        public int marginalCalories { get; set; }
        public int sedentaryMinutes { get; set; }
        public int steps { get; set; }
        public int veryActiveMinutes { get; set; }
        public decimal TotalDistance
        {
            get
            {
                var total = distances.Where(rec => rec.activity.Equals("total")).FirstOrDefault();
                if (total != null)
                {
                    return total.distance;
                }
                return 0;
            }
        }
        public int FairlyAndVeryActiveMinutes
        {
            get { return fairlyActiveMinutes + veryActiveMinutes; }
        }
    }

    public class ActivitiesSummaryDistanceData : FitbitDataBaseClass
    {
        public string activity { get; set; }
        public decimal distance { get; set; }
    }
}
