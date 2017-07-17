using UnityEngine;
using System.Collections;

public enum ChartType
{
	BurndownStoryPoints,
	BurndownItems,
	BurnupStoryPoints,
	BurnupItems
}

public enum ChartStateEnum
{
	Startup,
	FetchingSprintData,
	FinishedFetchingSprintData,
	FetchingWorkItemData,
	FinishedFetchingWorkItemData,
	CreatingObjects,
	UpdatingObjects,
	Main,
	Error,
	Settings
}
