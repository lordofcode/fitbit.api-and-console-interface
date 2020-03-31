using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace FitbitApiLibrary.DataStructures
{
    public class UserProfileContainer
    {
        public UserProfile user { get; set; }
    }
	public class UserProfile : FitbitDataBaseClass
    {
        public int age { get; set; }
        public string aboutMe { get; set; }
        public string avatar { get; set; }
        public string avatar150 { get; set; }
        public string avatar640 { get; set; }
        public string city { get; set; }
        public string clockTimeDisplayFormat { get; set; }
        public string country { get; set; }
        public string dateOfBirth { get; set; }
        public string displayName { get; set; }
        public string distanceUnit { get; set; }
        public string encodedId { get; set; }
        public string foodsLocale { get; set; }
        public string fullName { get; set; }
        public string gender { get; set; }
        public string glucoseUnit { get; set; }
        public int height { get; set; }
        public string heightUnit { get; set; }
        public string locale { get; set; }
        public string memberSince { get; set; }
        public int offsetFromUTCMillis { get; set; }
        public string startDayOfWeek { get; set; }
        public string state { get; set; }
        public double strideLengthRunning { get; set; }
        public double strideLengthWalking { get; set; }
        public string timezone { get; set; }
        public string waterUnit { get; set; }
        public int weight { get; set; }
        public string weightUnit { get; set; }

    }
}
