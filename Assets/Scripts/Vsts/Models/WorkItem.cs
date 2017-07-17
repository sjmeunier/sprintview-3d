using System;
using System.Collections.Generic;
using UnityEditor;

namespace Assets.Scripts.Vsts.Models
{
	[Serializable]
	public class WorkItem
	{
		public string Id { get; set; }
		public ItemType ItemType { get; set; }
		public State State { get; set; }
		public int Effort { get; set; }

	}
}
