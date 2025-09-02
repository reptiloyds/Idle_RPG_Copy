using System;

namespace _Game.Scripts.Systems.Server.Data.Events
{
	[Serializable]
	public sealed class ServerEventData
	{
		public int id;
		public string name;
		public string start_at;
		public string end_at;
		public bool is_entered;
	}
}