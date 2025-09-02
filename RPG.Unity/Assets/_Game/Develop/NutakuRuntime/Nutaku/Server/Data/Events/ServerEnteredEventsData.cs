using System;
using System.Collections.Generic;

namespace _Game.Scripts.Systems.Server.Data.Events
{
	[Serializable]
	public sealed class ServerEnteredEventsData
	{
		public List<ServerEnteredEventData> data;
	}
}