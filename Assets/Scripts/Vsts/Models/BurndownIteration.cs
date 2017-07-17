using System;
using System.Collections.Generic;

namespace Assets.Scripts.Vsts.Models
{
	[Serializable]
	public class BurndownIteration
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Path { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime FinishDate { get; set; }

		public List<DaySummary> DailySummary { get; set; }

		public BurndownIteration()
		{
			DailySummary = new List<DaySummary>();
		}
	}
}
