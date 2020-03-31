using FitbitApiLibrary;
using FitbitApiLibrary.DataStructures;
using FitbitApiLibrary.Enumerations;
using FitbitApiLibrary.Helpers;
using FitbitConnector.Enumerations;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace FitbitConnector.Helpers
{
	public static class FitbitUserHelper
	{
		private static string _AuthorizationFileName = "authorization.txt";
		private static string _BadgesFilename = "badges.txt";
		private static string _CodeFileName = "code.txt";
		private static string _ProfileFileName = "profile.txt";
		private static string _TokenFileName = "token.txt";

		private static int _CacheInMinutes = 60;
		private static int _MaxBadgeDownloadCount = 5;
		private static string _DataFolder { get; set; }
		private static string _UserFolder { get; set; }
		private static TokenData? _TokenData = null;
		private static List<KeyValuePair<string, string>> _ValuesForUrlReplacement = null;
		private static int? _User = null;


		private static bool _TokenRefreshed { get; set; }

		public static void Init(int? user=null)
		{
			_DataFolder = ConfigurationManager.AppSettings["files_location"];
			_User = user;
			_TokenData = null;
			_TokenRefreshed = false;
			_ValuesForUrlReplacement = null;
			if (_User.HasValue)
			{
				_UserFolder = Path.Combine(_DataFolder, _User.Value.ToString());
				if (Directory.Exists(_UserFolder) == false)
				{
					Directory.CreateDirectory(_UserFolder);
				}
				var tokenFile = Path.Combine(_UserFolder, _TokenFileName);
				if (File.Exists(tokenFile))
				{
					_TokenData = TokenHelper.ParseTokenStringToObject(File.ReadAllText(tokenFile));
				}
			}
		}

		public static List<FitbitFileSystemUser> GetAllUsers()
		{
			List<FitbitFileSystemUser> result = new List<FitbitFileSystemUser>();
			var subfolders = Directory.EnumerateDirectories(_DataFolder);
			foreach (var subfolder in subfolders)
			{
				var userData = GetUserDataFromFolder(subfolder);
				if (userData != null)
				{
					result.Add(userData);
				}
			}
			return result;
		}

		private static FitbitFileSystemUser GetUserDataFromFolder (string folder)
		{
			FitbitFileSystemUser result = null;
			var directoryInfo = new DirectoryInfo(folder);
			int number = 0;
			if (int.TryParse(directoryInfo.Name, out number))
			{
				var clientName = GetClientNameFromFolderFiles(folder);
				if (string.IsNullOrEmpty(clientName) == false)
				{
					result = new FitbitFileSystemUser() { number = number, user_id = clientName };
				}
			}
			return result;
		}

		private static string GetClientNameFromFolderFiles(string folder)
		{
			string result = "[onbekend]";
			var profileFile = Path.Combine(folder, "profile.txt");
			var tokenFile = Path.Combine(folder, "token.txt");
			if (File.Exists(profileFile))
			{
				var userProfile = DataHelper.ConvertStringToUserProfileObject(File.ReadAllText(profileFile));
				result = userProfile.fullName;
			}
			else if (File.Exists(tokenFile))
			{
				var tokenObject = TokenHelper.ParseTokenStringToObject(File.ReadAllText(tokenFile));
				result = tokenObject.user_id;
			}
			return result;
		}

		public static string AuthorizeApplication()
		{
			var result = "";
			ScopeEnumeration[] enumerations = new ScopeEnumeration[] { ScopeEnumeration.activity, ScopeEnumeration.heartrate, ScopeEnumeration.location, ScopeEnumeration.nutrition, ScopeEnumeration.profile, ScopeEnumeration.settings, ScopeEnumeration.sleep, ScopeEnumeration.social, ScopeEnumeration.weight };
			var url = FitbitOauth2Handler.GetAuthorizeUrl(
				ConfigurationManager.AppSettings["clientid"],
				ResponseTypeEnumeration.code,
				enumerations,
				new Uri(ConfigurationManager.AppSettings["callback_url"]),
				ExpirationEnumeration.OneDay,
				new List<PromptEnumeration>() { PromptEnumeration.login },
				ConfigurationManager.AppSettings["staterandomstring"]);
			var authorizationFileFullPath = Path.Combine(_UserFolder, _AuthorizationFileName);
			var codeFileFullPath = Path.Combine(_UserFolder, _CodeFileName);
			File.WriteAllText(authorizationFileFullPath, url);
			File.WriteAllText(codeFileFullPath, "vervang deze tekst met de volledige URL.");
			result = $"Open het bestand {authorizationFileFullPath}, plak de URL in je browser, doorloop het aanmeldscherm en sla de uiteindelijke URL op in {codeFileFullPath}.\r\nDruk daarna op een toets.";
			return result;
		}

		public static string GetCodeFromCodeFile()
		{
			var codeFileFullPath = Path.Combine(_UserFolder, _CodeFileName);
			if (File.Exists(codeFileFullPath) == false)
			{
				throw new Exception("Je hebt de code niet opgeslagen in {codeFileFullPath}.");
			}
			string line = File.ReadAllText(codeFileFullPath);
			int trimPosition = line.IndexOf("code=");
			line = line.Substring(trimPosition + 5);
			int hashTagPosition = line.IndexOf("#_=_");
			if (hashTagPosition > 0)
			{
				line = line.Substring(0, hashTagPosition);
			}
			return line;
		}

		public static void GetTokenForApplicationWithCode(string code)
		{
			var result = FitbitOauth2Handler.GetToken(ConfigurationManager.AppSettings["clientid"], ConfigurationManager.AppSettings["client_secret"], code, new Uri(ConfigurationManager.AppSettings["callback_url"]));
			var tokenFileFullPath = Path.Combine(_UserFolder, _TokenFileName);
			File.WriteAllText(tokenFileFullPath, result);
		}

		public static void GetRefreshTokenForApplication()
		{
			var result = FitbitOauth2Handler.RefreshToken(ConfigurationManager.AppSettings["clientid"], ConfigurationManager.AppSettings["client_secret"], _TokenData.Value.refresh_token);
			var tokenFileFullPath = Path.Combine(_UserFolder, _TokenFileName);
			File.WriteAllText(tokenFileFullPath, result);
			Init(_User);
			_TokenRefreshed = true;
		}

		public static dynamic GetData(FitbitDataType fitbitDataType)
		{
			var result = "";
			if (_TokenData == null)
			{
				result = "Geen geldig token beschikbaar.";
			}
			else
			{
				try
				{
					return GetFunctionForFitbitDataType(fitbitDataType);
				}
				catch(WebException webException)
				{
					if (webException.Status == WebExceptionStatus.ProtocolError && _TokenRefreshed == false)
					{
						// looks like token expired
						GetRefreshTokenForApplication();
						return GetFunctionForFitbitDataType(fitbitDataType);
					}
				}
			}
			return result;
		}

		private static dynamic GetFunctionForFitbitDataType(FitbitDataType fitbitDataType)
		{
			switch (fitbitDataType)
			{
				case FitbitDataType.Badges:
					return GetBadges();
				case FitbitDataType.Profile:
					return GetProfile();
				case FitbitDataType.ActivityTrackerSteps:
					return GetActivityTrackerSteps();
			}
			return null;
		}

		public static void PrefillValuesForUrlReplacement(List<KeyValuePair<string,string>> predefinedValues)
		{
			_ValuesForUrlReplacement = new List<KeyValuePair<string, string>>();
			foreach (var item in predefinedValues)
			{
				_ValuesForUrlReplacement.Add(new KeyValuePair<string, string>(item.Key, item.Value));
			}
		}
		private static List<KeyValuePair<string,string>> GetValuesForUrlReplacement()
		{
			var result = new List<KeyValuePair<string, string>>();
			if (_ValuesForUrlReplacement != null)
			{
				foreach (var item in _ValuesForUrlReplacement)
				{
					result.Add(new KeyValuePair<string, string>(item.Key, item.Value));
				}
			}
			if (_TokenData.HasValue)
			{
				result.Add(new KeyValuePair<string, string>("user-id", _TokenData.Value.user_id));
			}
			return result;
		}

		private static UserProfile GetProfile()
		{
			var profileData = "";
			var profileFileFullPath = Path.Combine(_UserFolder, _ProfileFileName);
			var doSync = true;
			if (File.Exists(profileFileFullPath))
			{
				var fileInfo = new FileInfo(profileFileFullPath);
				if (fileInfo.LastWriteTime.AddMinutes(_CacheInMinutes) > DateTime.Now)
				{
					profileData = File.ReadAllText(profileFileFullPath);
					doSync = false;
				}
			}
			if (doSync)
			{
				profileData = FitbitDataConnector.GetProfile(_TokenData.Value.access_token, GetValuesForUrlReplacement());
				if (string.IsNullOrEmpty(profileData) == false)
				{
					File.WriteAllText(profileFileFullPath, profileData);
				}
			}
			return DataHelper.ConvertStringToUserProfileObject(profileData);
		}

		/// <summary>
		/// Fetch from the API the badges the user owns.
		/// </summary>
		/// <returns></returns>
		private static Badge[] GetBadges()
		{
			var badgeData = "";
			var badgesFileFullPath = Path.Combine(_UserFolder, _BadgesFilename);
			var doSync = true;
			if (File.Exists(badgesFileFullPath))
			{
				var fileInfo = new FileInfo(badgesFileFullPath);
				if (fileInfo.LastWriteTime.AddMinutes(_CacheInMinutes) > DateTime.Now)
				{
					badgeData = File.ReadAllText(badgesFileFullPath);
					doSync = false;
				}
			}
			if (doSync)
			{
				badgeData = FitbitDataConnector.GetBadges(_TokenData.Value.access_token, GetValuesForUrlReplacement());
			}
			if (string.IsNullOrEmpty(badgeData) == false)
			{
				File.WriteAllText(badgesFileFullPath, badgeData);
			}
			var badges = DataHelper.ConvertStringToBadgesObject(badgeData);
			SaveBadgeFiles(badges);
			return badges;
		}

		private static void SaveBadgeFiles(Badge[] badges)
		{
			int downloadCount = 0;

			foreach (var badge in badges.OrderByDescending(rec => rec.dateTime))
			{
				if (downloadCount >= _MaxBadgeDownloadCount)
				{
					break;
				}
				foreach (var prop in badge.GetAllProperties())
				{
					var val = prop.GetValue(badge);
					if (val != null && val.ToString().ToLower().EndsWith(".png"))
					{
						var folderForImage = Path.Combine(_UserFolder, "badges");
						if (Directory.Exists(folderForImage) == false)
						{
							Directory.CreateDirectory(folderForImage);
						}
						folderForImage = Path.Combine(folderForImage, prop.Name);
						if (Directory.Exists(folderForImage) == false)
						{
							Directory.CreateDirectory(folderForImage);
						}
						var fullFileName = Path.Combine(folderForImage, Path.GetFileName(val.ToString()));
						if (File.Exists(fullFileName) == false)
						{
							using (var webClient = new WebClient())
							{
								downloadCount++;
								webClient.DownloadFile(val.ToString(), fullFileName);
							}
						}
					}
				}
			}
		}

		private static ActivityTrackerStepsData[] GetActivityTrackerSteps()
		{
			var trackerStepsData = FitbitDataConnector.GetActivityTrackerSteps(_TokenData.Value.access_token, GetValuesForUrlReplacement());
			return DataHelper.ConvertStringToActivityTrackerStepsObject(trackerStepsData);
		}
	}


	public class FitbitFileSystemUser
	{
		public int number { get; set; }
		public string user_id { get; set; }
	}
}
