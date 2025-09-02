using System.Reflection;
using NUnit.Framework;
using PleasantlyGames.BigDouble.Runtime;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Stats.Model;
using PleasantlyGames.RPG.Runtime.Core.Stats.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Config;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Save;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;

namespace Tests
{
    public class StatModifierTests
    {
        [Test]
        public void Test1()
        {
            var statData = new StatData(UnitStatType.Damage, 1, true);
            var config = new UnitStatConfig()
            {
                MaxLevel = 2,
                StatType = UnitStatType.Damage,
            };
            var valueFormula = new BaseValueFormula(new BigDouble(1, 2));
            
            var stat = new UnitStat(statData, config, null, valueFormula);

            var stuffMod = new StatModifier[]
            {
                new(new BigDouble(20, -2), StatModType.GroupKAdditive, "Knife", GroupOrder.Stuff),
                new(new BigDouble(30, -2), StatModType.GroupKAdditive, "Gun", GroupOrder.Stuff),
            };
            foreach (var mod in stuffMod) 
                stat.AddModifier(mod);

            var skillMod = new StatModifier[]
            {
                new(new BigDouble(100, -2), StatModType.GroupKAdditive, "Rage", GroupOrder.Skill),
            };
            foreach (var mod in skillMod) 
                stat.AddModifier(mod);
            
            var collectionMod = new StatModifier[]
            {
                new(new BigDouble(100, -2), StatModType.KAdditive, "Dog", GroupOrder.None),
                new(new BigDouble(100, -2), StatModType.KAdditive, "Car", GroupOrder.None),
            };
            foreach (var mod in collectionMod) 
                stat.AddModifier(mod);

            int expected = 600;
            var field = typeof(UnitStat).BaseType!.GetField("isDirty", BindingFlags.NonPublic | BindingFlags.Instance);
            field!.SetValue(stat, true);
            var result = stat.Value;

            Assert.AreEqual(expected, (int)result.ToDouble());
        }
        
        [Test]
        public void Test2()
        {
            var statData = new StatData(UnitStatType.Damage, 1, true);
            var config = new UnitStatConfig()
            {
                MaxLevel = 2,
                StatType = UnitStatType.Damage,
            };
            var valueFormula = new BaseValueFormula(new BigDouble(1, 2));
            
            var stat = new UnitStat(statData, config, null, valueFormula);

            var stuffMod = new StatModifier[]
            {
                new(new BigDouble(20, -2), StatModType.GroupKAdditive, "Knife", GroupOrder.Stuff),
                new(new BigDouble(30, -2), StatModType.GroupKAdditive, "Gun", GroupOrder.Stuff),
            };
            foreach (var mod in stuffMod) 
                stat.AddModifier(mod);

            var skillMod = new StatModifier[]
            {
                new(new BigDouble(100, -2), StatModType.GroupKAdditive, "Rage", GroupOrder.Skill),
            };
            foreach (var mod in skillMod) 
                stat.AddModifier(mod);

            int expected = 300;
            var field = typeof(UnitStat).BaseType!.GetField("isDirty", BindingFlags.NonPublic | BindingFlags.Instance);
            field!.SetValue(stat, true);
            var result = stat.Value;

            Assert.AreEqual(expected, (int)result.ToDouble());
        }

        [Test]
        public void Test3()
        {
            var statData = new StatData(UnitStatType.Damage, 1, true);
            var config = new UnitStatConfig()
            {
                MaxLevel = 2,
                StatType = UnitStatType.Damage,
            };
            var valueFormula = new BaseValueFormula(new BigDouble(1, 2));
            
            var stat = new UnitStat(statData, config, null, valueFormula);

            var stuffMod = new StatModifier[]
            {
                new(new BigDouble(0, 0), StatModType.GroupKAdditive, "Knife", GroupOrder.Stuff),
                new(new BigDouble(0, 0), StatModType.GroupKAdditive, "Gun", GroupOrder.Stuff),
            };
            foreach (var mod in stuffMod) 
                stat.AddModifier(mod);

            var skillMod = new StatModifier[]
            {
                new(new BigDouble(0, 0), StatModType.GroupKAdditive, "Rage", GroupOrder.Skill),
            };
            foreach (var mod in skillMod) 
                stat.AddModifier(mod);
            
            var collectionMod = new StatModifier[]
            {
                new(new BigDouble(0, 0), StatModType.KAdditive, "Dog", GroupOrder.None),
                new(new BigDouble(0, 0), StatModType.KAdditive, "Car", GroupOrder.None),
            };
            foreach (var mod in collectionMod) 
                stat.AddModifier(mod);

            int expected = 100;
            var field = typeof(UnitStat).BaseType!.GetField("isDirty", BindingFlags.NonPublic | BindingFlags.Instance);
            field!.SetValue(stat, true);
            var result = stat.Value;

            Assert.AreEqual(expected, (int)result.ToDouble());
        }
        
        [Test]
        public void Test4()
        {
            var statData = new StatData(UnitStatType.Damage, 1, true);
            var config = new UnitStatConfig()
            {
                MaxLevel = 2,
                StatType = UnitStatType.Damage,
            };
            var valueFormula = new BaseValueFormula(new BigDouble(1, 2));
            
            var stat = new UnitStat(statData, config, null, valueFormula);

            var stuffMod = new StatModifier[]
            {
                new(new BigDouble(0, 0), StatModType.GroupKAdditive, "Knife", GroupOrder.Stuff),
                new(new BigDouble(0, 0), StatModType.GroupKAdditive, "Gun", GroupOrder.Stuff),
            };
            foreach (var mod in stuffMod) 
                stat.AddModifier(mod);

            var skillMod = new StatModifier[]
            {
                new(new BigDouble(0, 0), StatModType.GroupKAdditive, "Rage", GroupOrder.Skill),
            };
            foreach (var mod in skillMod) 
                stat.AddModifier(mod);
            
            var collectionMod = new StatModifier[]
            {
                new(new BigDouble(100, -2), StatModType.KAdditive, "Dog", GroupOrder.None),
                new(new BigDouble(100, -2), StatModType.KAdditive, "Car", GroupOrder.None),
            };
            foreach (var mod in collectionMod) 
                stat.AddModifier(mod);

            int expected = 300;
            var field = typeof(UnitStat).BaseType!.GetField("isDirty", BindingFlags.NonPublic | BindingFlags.Instance);
            field!.SetValue(stat, true);
            var result = stat.Value;

            Assert.AreEqual(expected, (int)result.ToDouble());
        }
        
        [Test]
        public void Test5()
        {
            var statData = new StatData(UnitStatType.Damage, 1, true);
            var config = new UnitStatConfig()
            {
                MaxLevel = 2,
                StatType = UnitStatType.Damage,
            };
            var valueFormula = new BaseValueFormula(new BigDouble(1, 2));
            
            var stat = new UnitStat(statData, config, null, valueFormula);

            var stuffMod = new StatModifier[]
            {
                new(new BigDouble(40, -2), StatModType.GroupKAdditive, "Knife", GroupOrder.Stuff),
                new(new BigDouble(10, -2), StatModType.GroupKAdditive, "Gun", GroupOrder.Stuff),
            };
            foreach (var mod in stuffMod) 
                stat.AddModifier(mod);

            var skillMod = new StatModifier[]
            {
                new(new BigDouble(100, -2), StatModType.GroupKAdditive, "Rage", GroupOrder.Skill),
            };
            foreach (var mod in skillMod) 
                stat.AddModifier(mod);
            
            var collectionMod = new StatModifier[]
            {
                new(new BigDouble(0, 0), StatModType.KAdditive, "Dog", GroupOrder.None),
                new(new BigDouble(0, 0), StatModType.KAdditive, "Car", GroupOrder.None),
            };
            foreach (var mod in collectionMod) 
                stat.AddModifier(mod);

            int expected = 300;
            var field = typeof(UnitStat).BaseType!.GetField("isDirty", BindingFlags.NonPublic | BindingFlags.Instance);
            field!.SetValue(stat, true);
            var result = stat.Value;

            Assert.AreEqual(expected, (int)result.ToDouble());
        }
    }
}