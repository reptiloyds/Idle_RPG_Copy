using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;

namespace PleasantlyGames.RPG.Runtime.Core.TooltipsFeature.Keys
{
    public static class ResourceTooltipKeys
    {
        private const string SoftDescription = "tooltip_soft_description";
        private const string HardDescription = "tooltip_hard_description";
        private const string ExpShardDescription = "tooltip_exp_shard_description";
        private const string EvolveShardDescription = "tooltip_evolve_shard_description";
        private const string BossRushKeyDescription = "tooltip_boss_rush_key_description";
        private const string SoftRushKeyDescription = "tooltip_soft_rush_key_description";
        private const string SlotRushKeyDescription = "tooltip_slot_rush_key_description";
        private const string ChatTicketDescription = "tooltip_chat_ticket_description";
        
        private const string SoftName = "Soft";
        private const string HardName = "Hard";
        private const string ExpShardName = "ExpShard";
        private const string EvolveShardName = "EvolveShard";
        private const string BossRushKeyName = "BossRushKey";
        private const string SoftRushKeyName = "SoftRushKey";
        private const string SlotRushKeyName = "SlotRushKey";
        private const string ChatTicketName = "ChatTicket";
        
        public static readonly Dictionary<ResourceType, ResourceTokens> Resources = new()
        {
            [ResourceType.Soft] = new ResourceTokens(SoftName, SoftDescription),
            [ResourceType.Hard] = new ResourceTokens(HardName, HardDescription),
            [ResourceType.ExpShard] = new ResourceTokens(ExpShardName, ExpShardDescription),
            [ResourceType.EvolveShard] = new ResourceTokens(EvolveShardName, EvolveShardDescription),
            [ResourceType.BossRushKey] = new ResourceTokens(BossRushKeyName, BossRushKeyDescription),
            [ResourceType.SoftRushKey] = new ResourceTokens(SoftRushKeyName, SoftRushKeyDescription),
            [ResourceType.SlotRushKey] = new ResourceTokens(SlotRushKeyName, SlotRushKeyDescription),
            [ResourceType.ChatTicket] = new ResourceTokens(ChatTicketName, ChatTicketDescription),
        };
        
        public readonly struct ResourceTokens
        {
            public readonly string NameToken;
            public readonly string DescriptionToken;

            public ResourceTokens(string name, string description)
            {
                NameToken = name;
                DescriptionToken = description;
            }
        }
    }
}