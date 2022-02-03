using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml.Serialization;
using System.IO;

public static class GameManager
{
	public static int CurrentScore { get; set; }

	public static int CurrentLives { get; set; }

	public static List<Score> Scoreboard { get; set; }

	public static bool IsRunning { get; set; }

	public static string PlayerName { get; set; }


	public static void LoadScoreboard()
	{
		if (File.Exists(Application.persistentDataPath + "/saves/scoreboard.xml"))
		{
			XmlSerializer serializer = new XmlSerializer(typeof(List<Score>));
			FileStream stream = new FileStream(Application.persistentDataPath + "/saves/scoreboard.xml", FileMode.Open);

			Scoreboard = serializer.Deserialize(stream) as List<Score>;

			stream.Dispose();
		}
		else
		{
			Scoreboard = new List<Score>();
		}
	}

	private static void SaveScoreboard()
	{
		if (!Directory.Exists(Application.persistentDataPath + "/saves/"))
		{
			Directory.CreateDirectory(Application.persistentDataPath + "/saves/");
		}

		XmlSerializer serializer = new XmlSerializer(typeof(List<Score>));
		FileStream stream = new FileStream(Application.persistentDataPath + "/saves/scoreboard.xml", FileMode.Create);
		serializer.Serialize(stream, Scoreboard);
		stream.Close();
	}

	public static void AddScoreToScoreboard()
	{
		LoadScoreboard();

		if(PlayerName == "" || PlayerName == null)
		{
			PlayerName = "Player";
		}

		Scoreboard.Add(new Score(PlayerName, CurrentScore));

		SortScoreboard();

		SaveScoreboard();
	}

	private static void SortScoreboard()
	{
		Scoreboard.Sort((Score x, Score y) => y.score.CompareTo(x.score));
	}
}
