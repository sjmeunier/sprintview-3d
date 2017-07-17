using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Vsts.Models
{
	[Serializable]
	public class ProjectCollection
	{
		public List<Project> Projects { get; set; }

		public ProjectCollection()
		{
			Projects = new List<Project>();
		}
		public static ProjectCollection CreateFromJson(string json)
		{
			ProjectCollection projectCollection = new ProjectCollection();

			var jsonObject = (Dictionary<string, object>)MiniJSON.Json.Deserialize(json);
			var projects = (List<object>)jsonObject["value"];
			foreach (var project in projects)
			{
				var currentProject = (Dictionary<string, object>) project;
				projectCollection.Projects.Add(new Project()
				{
					Id = (string)currentProject["id"],
					Name = (string)currentProject["name"]
				});
			}
			return projectCollection;
		}

	}
}
