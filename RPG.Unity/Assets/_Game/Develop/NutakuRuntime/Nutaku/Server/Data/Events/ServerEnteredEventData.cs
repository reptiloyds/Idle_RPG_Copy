using System;

namespace _Game.Scripts.Systems.Server.Data.Events
{
	[Serializable]
	public sealed class ServerEnteredEventData
	{
		public int id;
		public string name;
		public int milestones;
		public double points;
	}
}