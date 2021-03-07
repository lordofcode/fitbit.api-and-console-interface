using System;
using System.Collections.Generic;
using System.Text;

namespace FitbitApiLibrary.DataStructures
{
	public static class Period
	{
		public static string Dag { get { return "1d"; } }
		public static string Week { get { return "1w"; } }
		public static string Maand { get { return "1m"; } }
		public static string Kwartaal { get { return "3m"; } }
		public static string HalfJaar { get { return "6m"; } }
		public static string Jaar { get { return "1y"; } }
	}
}