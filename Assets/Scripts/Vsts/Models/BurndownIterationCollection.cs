using System;
using System.Collections.Generic;

namespace Assets.Scripts.Vsts.Models
{
	[Serializable]
	public class BurndownIterationCollection
	{
		public List<BurndownIteration> Iterations { get; set; }

		public BurndownIterationCollection()
		{
			Iterations = new List<BurndownIteration>();
		}
		public static BurndownIterationCollection CreateFromJson(string json)
		{
			BurndownIterationCollection iterationCollection = new BurndownIterationCollection();

			var jsonObject = (Dictionary<string, object>)MiniJSON.Json.Deserialize(json);
			var iterations = (List<object>)jsonObject["value"];
			foreach (var iteration in iterations)
			{
				var currentIteration = (Dictionary<string, object>) iteration;
				var iterationAttributes = (Dictionary<string, object>) currentIteration["attributes"];

				iterationCollection.Iterations.Add(new BurndownIteration()
				{
					Id = (string)currentIteration["id"],
					Name = (string)currentIteration["name"],
					Path = (string)currentIteration["path"],
					StartDate = DateTime.Parse((string)iterationAttributes["startDate"]),
					FinishDate = DateTime.Parse((string)iterationAttributes["finishDate"])
				});
			}
			return iterationCollection;
		}

	}
}
