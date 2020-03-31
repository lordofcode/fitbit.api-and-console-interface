using FitbitApiLibrary.Enumerations;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace FitbitApiLibrary.Helpers
{
	public static class TokenHelper
	{
		public static TokenData ParseTokenStringToObject(string token)
		{
			return JsonSerializer.Deserialize<TokenData>(token);
		}
	}

	public struct TokenData
	{
		public string access_token { get; set; }
		public int expires_in { get; set; }
		public string refresh_token { get; set; }
		public string scope { get; set; }
		public ScopeEnumeration[] GetScopeItems()
		{
			var items = new List<ScopeEnumeration>();
			var stringItems = scope.Split(new char[] { ' ' });
			var enumItems = Enum.GetValues(typeof(ScopeEnumeration));
			foreach (var item in stringItems)
			{
				foreach (var enumItem in enumItems)
				{
					if (item == enumItem.ToString())
					{
						items.Add((ScopeEnumeration)enumItem);
						break;
					}
				}
			}
			return items.ToArray();
		}
		public string token_type { get; set; }
		public string user_id { get; set; }
	}
}
