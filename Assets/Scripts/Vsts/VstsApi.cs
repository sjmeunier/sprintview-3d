using System;
using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Vsts.Models;

namespace Assets.Scripts.Vsts
{
	public class VstsApi
	{
		private readonly string _username;
		private readonly string _token;
		private readonly string _baseUrl;

		public VstsApi(string username, string token, string account)
		{
			this._username = username;
			this._token = token;
			this._baseUrl = string.Format("https://{0}.visualstudio.com/", account);
		}

		public WWW BuildGetProjectsRequest()
		{
			Dictionary<string, string> headers = new Dictionary<string, string>();
			headers.Add("Authorization",
				"Basic " + System.Convert.ToBase64String(
					System.Text.Encoding.ASCII.GetBytes(string.Format("{0}:{1}", this._username, this._token))));
			headers.Add("Content-Type", "application/json");

			WWW www = new WWW(string.Format("{0}{1} ", this._baseUrl, VstsEndpoints.ProjectsEndPoint), null, headers);

			return www;
		}

		public WWW BuildGetTeamsRequest(Project project)
		{
			Dictionary<string, string> headers = new Dictionary<string, string>();
			headers.Add("Authorization",
				"Basic " + System.Convert.ToBase64String(
					System.Text.Encoding.ASCII.GetBytes(string.Format("{0}:{1}", this._username, this._token))));
			headers.Add("Content-Type", "application/json");

			var endpoint = string.Format(VstsEndpoints.TeamsEndPoint, project.Name);
			WWW www = new WWW(string.Format("{0}{1} ", this._baseUrl, endpoint), null, headers);

			return www;
		}

		public WWW BuildGetIterationsRequest(Project project, Team team)
		{
			Dictionary<string, string> headers = new Dictionary<string, string>();
			headers.Add("Authorization",
				"Basic " + System.Convert.ToBase64String(
					System.Text.Encoding.ASCII.GetBytes(string.Format("{0}:{1}", this._username, this._token))));
			headers.Add("Content-Type", "application/json");

			var endpoint = string.Format(VstsEndpoints.IterationsEndPoint, project.Id, team.Id);
			WWW www = new WWW(string.Format("{0}{1} ", this._baseUrl, endpoint), null, headers);

			return www;
		}

		public WWW BuildGetWorkItemIdsAsPerDateRequest(Project project, Team team, BurndownIteration iteration, DateTime specifiedDate, string fields)
		{
			var query = string.Format(
				"SELECT {4} FROM WorkItems WHERE System.AreaPath = '{0}\\\\{1}' AND System.IterationPath= '{2}' AND System.WorkItemType = 'Product Backlog Item' AND System.State <> 'Removed' ASOF '{3}'",
				project.Name, team.Name, iteration.Path.Replace("\\", "\\\\"), specifiedDate.ToString("MM/dd/yyyy 23:59"), fields);

			var jsonString = string.Format("{{ \"query\": \"{0}\" }}", query);

			Dictionary<string, string> headers = new Dictionary<string, string>();
			headers.Add("Authorization",
				"Basic " + System.Convert.ToBase64String(
					System.Text.Encoding.ASCII.GetBytes(string.Format("{0}:{1}", this._username, this._token))));
			headers.Add("Content-Type", "application/json");
			headers.Add("Content-Length", jsonString.Length.ToString());

			var endpoint = string.Format(VstsEndpoints.WorkItemsQueryEndPoint, project.Id);
			WWW www = new WWW(string.Format("{0}{1} ", this._baseUrl, endpoint), System.Text.Encoding.ASCII.GetBytes(jsonString), headers);

			return www;
		}

		public WWW BuildGetWorkItemsAsPerDateRequest(WorkItemIdList workItemIdList)
		{
			Dictionary<string, string> headers = new Dictionary<string, string>();
			headers.Add("Authorization",
				"Basic " + System.Convert.ToBase64String(
					System.Text.Encoding.ASCII.GetBytes(string.Format("{0}:{1}", this._username, this._token))));
			headers.Add("Content-Type", "application/json");

			var endpoint = string.Format(VstsEndpoints.WorkItemsEndPoint, string.Join(",", workItemIdList.Ids.ToArray()), string.Join(",",workItemIdList.Fields.ToArray()), workItemIdList.AsOf);
			WWW www = new WWW(string.Format("{0}{1} ", this._baseUrl, endpoint), null, headers);

			return www;
		}
	}
}
