using System;
using System.Collections.Generic;
using UnityEditor;

namespace Assets.Scripts.Vsts.Models
{
	public enum ItemType
	{
		PBI,
		Bug,
		Unknown
	}

	public enum State
	{
		New,
		Approved,
		Committed,
		InProgress,
		ReadyForCR,
		ReadyForTest,
		POCheck,
		Done,
		Closed,
		ReadyForRelease,
		Unknown
	}
}
