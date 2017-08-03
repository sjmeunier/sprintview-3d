using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Vsts.Models;

namespace Assets.Scripts
{
	public class CommonData
	{
		public static bool LoadedData = false;

		public static Team Team;
		public static Project Project;
		public static BurndownIterationCollection Iterations;
	}
}
