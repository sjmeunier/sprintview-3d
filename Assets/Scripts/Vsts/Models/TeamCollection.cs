using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Vsts.Models
{
	[Serializable]
	public class TeamCollection
	{
		public List<Team> Teams { get; set; }

		public TeamCollection()
		{
			Teams = new List<Team>();
		}
		public static TeamCollection CreateFromJson(string json)
		{
			TeamCollection teamCollection = new TeamCollection();

			var jsonObject = (Dictionary<string, object>)MiniJSON.Json.Deserialize(json);
			var teams = (List<object>)jsonObject["value"];
			foreach (var team in teams)
			{
				var currentTeam = (Dictionary<string, object>)team;
				teamCollection.Teams.Add(new Team()
				{
					Id = (string)currentTeam["id"],
					Name = (string)currentTeam["name"]
				});
			}
			return teamCollection;
		}

	}
}
