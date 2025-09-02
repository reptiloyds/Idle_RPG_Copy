using System;

namespace _Game.Scripts.Systems.Server.Data.Events
{
	[Serializable]
	public sealed class ServerEventRewardData
	{
		public string event_name;
		public int milestones;
		public double points;
		public int position;
		public int user_numeric_id;
	}
}