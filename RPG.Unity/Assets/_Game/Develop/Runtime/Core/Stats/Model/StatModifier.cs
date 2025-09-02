using PleasantlyGames.RPG.Runtime.Core.Stats.Type;

namespace PleasantlyGames.RPG.Runtime.Core.Stats.Model
{
	public class StatModifier
	{
		
		public readonly BigDouble.Runtime.BigDouble Value;
		public readonly StatModType Type;
		public readonly int Order;
		public readonly object Source;
		public readonly bool Temporary;

		public StatModifier(BigDouble.Runtime.BigDouble value, StatModType type, int order, object source, bool temporary = false)
		{
			Value = value;
			Type = type;
			if (type == StatModType.GroupKAdditive && order >= 0)
				Order = order;
			else
				Order = (int)type;
			Source = source;
			Temporary = temporary;
		}

		public StatModifier(BigDouble.Runtime.BigDouble value, StatModType type) : this(value, type, (int)type, null) { }

		public StatModifier(BigDouble.Runtime.BigDouble value, StatModType type, GroupOrder order) : this(value, type, (int)order, null) { }
		
		public StatModifier(BigDouble.Runtime.BigDouble value, StatModType type, object source, GroupOrder order) : this(value, type, (int)order, source) { }
	}
}
