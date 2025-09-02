using System;
using System.Collections.Generic;

namespace _Game.Scripts.Systems.Server.Data.Events
{
	[Serializable]
	public sealed class ServerLeaderboardData
	{
		public List<RoomUserData> data;
		public RoomUserData me;
		public int limit;
		public int page;
		public int total;
	}
}