using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Vsts.Models
{
	[Serializable]
	public class DaySummary
	{
		public DateTime Date { get; set; }
		public List<WorkItem> WorkItems { get; set; }

		public DaySummary()
		{
			WorkItems = new List<WorkItem>();
		}

		public int GetTotalStoryPoints()
		{
			return WorkItems.Where(x => x.State != State.New && x.State != State.Approved).Sum(x => x.Effort);
		}

		public int GetOpenStoryPoints()
		{
			return WorkItems.Where(x => x.State == State.Committed || x.State == State.InProgress || x.State == State.ReadyForCR).Sum(x => x.Effort);
		}

		public int GetTotalItems()
		{
			return WorkItems.Where(x => x.State != State.New && x.State != State.Approved).Count();
		}

		public int GetOpenItems()
		{
			return WorkItems.Where(x => x.State == State.Committed || x.State == State.InProgress || x.State == State.ReadyForCR).Count();
		}

		private static ItemType GetWorkItemType(string itemTypeString)
		{
			ItemType itemType = ItemType.Unknown;

			switch (itemTypeString)
			{
				case "Product Backlog Item":
					itemType = ItemType.PBI;
					break;
				case "Bug":
					itemType = ItemType.Bug;
					break;
			}
			return itemType;
		}

		private static State GetWorkItemState(string stateString)
		{
			State state = State.Unknown;

			switch (stateString)
			{
				case "New":
					state = State.New;
					break;
				case "Approved":
					state = State.Approved;
					break;
				case "Closed":
					state = State.Closed;
					break;
				case "Committed":
					state = State.Committed;
					break;
				case "Done":
					state = State.Done;
					break;
				case "In Progress":
					state = State.InProgress;
					break;
				case "PO Check":
					state = State.POCheck;
					break;
				case "Ready for Code Review":
					state = State.ReadyForCR;
					break;
				case "Ready for test":
					state = State.ReadyForTest;
					break;
				case "Ready for Release":
					state = State.ReadyForRelease;
					break;
			}
			return state;
		}

		public static DaySummary CreateFromJson(DateTime date, string json)
		{
			DaySummary daySummary = new DaySummary();
			daySummary.Date = date;

			var jsonObject = (Dictionary<string, object>)MiniJSON.Json.Deserialize(json);
			
			var workItems = (List<object>)jsonObject["value"];
			foreach (var workItem in workItems)
			{
				var fields = ((Dictionary<string, object>)((Dictionary<string, object>)workItem)["fields"]);

				ItemType itemType = fields.ContainsKey("System.WorkItemType")
					? GetWorkItemType(fields["System.WorkItemType"].ToString())
					: ItemType.Unknown;

				State state = fields.ContainsKey("System.State")
					? GetWorkItemState(fields["System.State"].ToString())
					: State.Unknown;

				if (state != State.Unknown && itemType != ItemType.Unknown)
				{
					daySummary.WorkItems.Add(new WorkItem()
					{
						Id = fields.ContainsKey("System.Id") ? fields["System.Id"].ToString() : "",
						ItemType = itemType,
						State = state,
						Effort = int.Parse(fields.ContainsKey("Microsoft.VSTS.Scheduling.Effort") ? fields["Microsoft.VSTS.Scheduling.Effort"].ToString() : "0")
					});
				}
			}
			
			return daySummary;
		}
	}

}
