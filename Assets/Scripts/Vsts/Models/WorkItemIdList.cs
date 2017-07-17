using System;
using System.Collections.Generic;
using UnityEditor;

namespace Assets.Scripts.Vsts.Models
{
	[Serializable]
	public class WorkItemIdList
	{
		public List<string> Ids { get; set; }
		public string AsOf { get; set; }
		public List<string> Fields { get; set; }

		public WorkItemIdList()
		{
			Ids = new List<string>();
			Fields = new List<string>();
		}

		public static WorkItemIdList CreateFromJson(string json)
		{
			WorkItemIdList workItemIdList = new WorkItemIdList();

			var jsonObject = (Dictionary<string, object>)MiniJSON.Json.Deserialize(json);
			workItemIdList.AsOf = (string) jsonObject["asOf"];

			var columns = (List<object>)jsonObject["columns"];
			foreach (var column in columns)
			{

				workItemIdList.Fields.Add((string)((Dictionary<string, object>)column)["referenceName"]);
			}

			var workItems = (List<object>)jsonObject["workItems"];
			foreach (var workItem in workItems)
			{
				workItemIdList.Ids.Add((((Dictionary<string, object>)workItem)["id"]).ToString());
			}
			
			return workItemIdList;
		}
	}
}
