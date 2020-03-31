using FitbitApiLibrary.DataStructures;
using FitbitApiLibrary.Helpers;
using FitbitConnector.Enumerations;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace FitbitConnector.Helpers
{
	public static class MenuBuilderHelper
	{
		private static bool _CloseMenuBuilder { get; set; }
		private static MenuScreen _MenuScreen { get; set; }
		private static bool _OutputRedirected { get; set; }

		private static TextWriter _DefaultConsoleOutput { get; set; }
		private static StringWriter _StringWriter = new StringWriter();

		private static int? _SelectedUser { get; set; }

		public static void Init()
		{
			_CloseMenuBuilder = false;
			_MenuScreen = MenuScreen.StartMenu;
			_OutputRedirected = false;
			_SelectedUser = null;
			FitbitUserHelper.Init();
		}

		public static void ShowMenu(List<string> items, bool showBack = true)
		{
			Console.Clear();
			foreach (var item in items)
			{
				Console.WriteLine(item);
			}
			if (showBack)
			{
				Console.WriteLine("");
				Console.WriteLine("Druk op t om terug te gaan naar het vorige scherm.");
			}
			Console.WriteLine("");
			Console.WriteLine("x Sluit het programma af");
		}

		public static void AwaitAndProcessInput()
		{
			ToggleOutput();
			var pressedKey = Console.ReadKey();
			ToggleOutput();
			ProcessInput(pressedKey.KeyChar);
		}

		private static void ToggleOutput()
		{
			var currentOutput = Console.Out;
			if (_OutputRedirected)
			{
				Console.SetOut(_DefaultConsoleOutput);
			}
			else
			{
				_DefaultConsoleOutput = currentOutput;
				Console.SetOut(_StringWriter);
			}
			_OutputRedirected = !_OutputRedirected;
		}

		public static bool DisplayScreen()
		{
			if (_CloseMenuBuilder)
			{
				return false;
			}
			switch (_MenuScreen)
			{
				case MenuScreen.AddUserMenu:
					ShowAddUserMenu();
					break;
				case MenuScreen.DeleteExistingUserMenu:
					ShowDeleteExistingUserMenu();
					break;
				case MenuScreen.EndScreen:
					ShowEndScreen();
					break;
				case MenuScreen.ExistingUserMenu:
					ShowExistingUserMenu();
					break;
				case MenuScreen.ExistingUserProfileMenu:
					ShowExistingUserProfileMenu();
					break;
				case MenuScreen.ExistingUserBadgesMenu:
					ShowExistingUserBadgesMenu();
					break;
				case MenuScreen.ExistingUserTrackerStepsMenu:
					ShowExistingUserTrackerStepsMenu();
					break;
				case MenuScreen.SelectExistingUserMenu:
					ShowSelectExistingUserMenu();
					break;
				case MenuScreen.StartMenu:
					ShowStartMenu();
					break;
				default:
					ShowStartMenu();
					break;
			}
			return true;
		}

		#region screens


		private static void ShowAddUserMenu()
		{
			var menuItems = new List<string>();
			var allUsers = FitbitUserHelper.GetAllUsers().Select(rec => rec.number);
			var userNumber = 0;
			for (var k =1;k < 10; k++)
			{
				if (allUsers.Contains(k) == false)
				{
					userNumber = k;
					break;
				}
			}
			if (userNumber == 0)
			{
				menuItems.Add("Er kunnen maximaal 9 gebruikers aangemaakt worden.");
			}
			else
			{
				FitbitUserHelper.Init(userNumber);
				var message = FitbitUserHelper.AuthorizeApplication();
				if (string.IsNullOrEmpty(message) == false)
				{
					ShowMenu(new List<string>() { message, "", "", "" });
					Console.ReadKey();
					var code = FitbitUserHelper.GetCodeFromCodeFile();
					FitbitUserHelper.GetTokenForApplicationWithCode(code);
					_MenuScreen = MenuScreen.SelectExistingUserMenu;
				}
			}
			FitbitUserHelper.Init();
			DisplayScreen();
			AwaitAndProcessInput();
		}

		private static void ShowDeleteExistingUserMenu()
		{

		}


		private static void ShowEndScreen()
		{
			_CloseMenuBuilder = true;
			Console.Clear();
			Console.WriteLine("Het programma wordt nu beeindigd. Druk op een toets.");
			Console.ReadKey();
		}

		private static void ShowExistingUserMenu()
		{
			var profileData = FitbitUserHelper.GetData(FitbitDataType.Profile);
			ShowMenu(new List<string>() {
				$"Gegevens van {profileData.fullName}",
				"",
				"1 Toon profielgegevens",
				"2 Toon badges",
				"3 Toon tracker-data, stappen deze maand"
			});
			AwaitAndProcessInput();
		}

		private static void ShowExistingUserProfileMenu()
		{
			var profileData = FitbitUserHelper.GetData(FitbitDataType.Profile);
			var profileDataList = new List<string>();
			foreach(var item in profileData.GetAllProperties())
			{
				profileDataList.Add($"{item.Name}: {item.GetValue(profileData)}");
			}
			ShowMenu(profileDataList);
			AwaitAndProcessInput();
		}

		private static void ShowExistingUserBadgesMenu()
		{
			var badgesData = FitbitUserHelper.GetData(FitbitDataType.Badges) as Badge[];
			var badgesDataList = new List<string>();
			foreach (var badge in badgesData.OrderBy(rec => rec.dateTime))
			{
				foreach (var item in badgesData[0].GetAllProperties())
				{
					// we show only 3 fields
					switch(item.Name.ToLower())
					{
						case "datetime":
						case "description":
						case "earnedmessage":
							badgesDataList.Add($"{item.Name}: {item.GetValue(badge)}");
							break;
					}
				}
				badgesDataList.Add("================================");
			}
			ShowMenu(badgesDataList);
			AwaitAndProcessInput();
		}

		private static void ShowExistingUserTrackerStepsMenu()
		{
			// date can be a yyyy-MM-dd or 'today' 
			FitbitUserHelper.PrefillValuesForUrlReplacement(new List<KeyValuePair<string, string>>() { 
				new KeyValuePair<string, string>("startdate","today"),
				new KeyValuePair<string, string>("period","1m"),
			});
			var stepData = FitbitUserHelper.GetData(FitbitDataType.ActivityTrackerSteps) as ActivityTrackerStepsData[];
			var stepDataList = new List<string>();
			foreach (var stepLog in stepData.OrderBy(rec => rec.dateTime))
			{
				foreach (var item in stepData[0].GetAllProperties())
				{
					stepDataList.Add($"{item.Name}: {item.GetValue(stepLog)}");
				}
				stepDataList.Add("================================");
			}
			ShowMenu(stepDataList);
			AwaitAndProcessInput();
		}

		private static void ShowSelectExistingUserMenu()
		{
			var users = FitbitUserHelper.GetAllUsers();
			if (users.Count == 0)
			{
			ShowMenu(new List<string>() {
				"Er zijn nog geen gebruikers aangemaakt."
				});
			}
			else
			{
				List<string> items = new List<string>();
				foreach (var user in users.OrderBy(rec => rec.number))
				{
					items.Add($"{user.number} {user.user_id}");
				}
				MenuBuilderHelper.ShowMenu(items);
			}
			AwaitAndProcessInput();
		}

		private static void ShowStartMenu()
		{
			ShowMenu(new List<string>() {
				"1 Selecteer bestaande gebruiker",
				"2 Voeg gebruiker toe",
				"3 Verwijder bestaande gebruiker"
			}, false);
			AwaitAndProcessInput();
		}

		#endregion

		private static void ProcessInput(char pressedKey)
		{
			if (pressedKey == 't')
			{
				switch (_MenuScreen)
				{
					case MenuScreen.AddUserMenu:
						_MenuScreen = MenuScreen.SelectExistingUserMenu;
						break;
					case MenuScreen.DeleteExistingUserMenu:
						_MenuScreen = MenuScreen.SelectExistingUserMenu;
						break;
					case MenuScreen.ExistingUserBadgesMenu:
						_MenuScreen = MenuScreen.ExistingUserMenu;
						break;
					case MenuScreen.ExistingUserMenu:
						_MenuScreen = MenuScreen.SelectExistingUserMenu;
						break;
					case MenuScreen.ExistingUserProfileMenu:
						_MenuScreen = MenuScreen.ExistingUserMenu;
						break;
					case MenuScreen.ExistingUserTrackerStepsMenu:
						_MenuScreen = MenuScreen.ExistingUserMenu;
						break;
					case MenuScreen.SelectExistingUserMenu:
						_MenuScreen = MenuScreen.StartMenu;
						break;
					default:
						_MenuScreen = MenuScreen.StartMenu;
						break;
				}
			}
			else if (pressedKey == 'x')
			{
				_MenuScreen = MenuScreen.EndScreen;
			}
			else
			{
				var numericValue = 0;
				switch (_MenuScreen)
				{
					case MenuScreen.AddUserMenu:
						_MenuScreen = MenuScreen.StartMenu;
						break;
					case MenuScreen.DeleteExistingUserMenu:
						_MenuScreen = MenuScreen.StartMenu;
						break;
					case MenuScreen.ExistingUserMenu:
						if (int.TryParse(pressedKey.ToString(), out numericValue))
						{
							switch (numericValue)
							{
								case 1:
									_MenuScreen = MenuScreen.ExistingUserProfileMenu;
									break;
								case 2:
									_MenuScreen = MenuScreen.ExistingUserBadgesMenu;
									break;
								case 3:
									_MenuScreen = MenuScreen.ExistingUserTrackerStepsMenu;
									break;
							}
						}
						break;
					case MenuScreen.SelectExistingUserMenu:
						if (int.TryParse(pressedKey.ToString(), out numericValue))
						{
							FitbitUserHelper.Init(numericValue);
							_MenuScreen = MenuScreen.ExistingUserMenu;
						}
						else
						{
							if (pressedKey == 't')
							{
								_MenuScreen = MenuScreen.StartMenu;
							}
							else if (pressedKey == 'x')
							{
								_MenuScreen = MenuScreen.EndScreen;
							}
						}
						break;
					case MenuScreen.StartMenu:
						if (int.TryParse(pressedKey.ToString(), out numericValue))
						{
							switch (numericValue)
							{
								case 1:
									_MenuScreen = MenuScreen.SelectExistingUserMenu;
									break;
								case 2:
									_MenuScreen = MenuScreen.AddUserMenu;
									break;
								case 3:
									_MenuScreen = MenuScreen.DeleteExistingUserMenu;
									break;
							}
						}
						else
						{
							if (pressedKey == 'x')
							{
								_MenuScreen = MenuScreen.EndScreen;
							}
						}
						break;
				}
			}
			DisplayScreen();
		}

	}
}
