using System;
using System.Collections.Generic;

namespace _Game.Scripts.Systems.Server.Data.Events
{
	[Serializable]
	public sealed class ServerEventsData
	{
		public List<ServerEventData> data;
		public int limit;
		public int page;
		public int total;
	}
}