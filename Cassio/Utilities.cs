using System;
using System.Linq;
using HesaEngine.SDK;
using HesaEngine.SDK.Enums;
using HesaEngine.SDK.GameObjects;
using SharpDX;

namespace Cassio
{
    public class Utilities
    {
        private static int _currentTick = 0;

        public static void Tick()
        {
            _currentTick++;
        }

        public static bool ShouldWork(int targetValue)
        {
            if (targetValue <= _currentTick)
            {
                _currentTick = 0;
                return true;
            }
            return false;
        }

        public static bool WillStunMinimum(Spell spell, int count, Vector3 target)
        {
            //Does not work as intended yet, need to count the actual facing targets
            return count <= spell.CountHits(ObjectManager.Heroes.Enemies.ToList<Obj_AI_Base>(), target)
                   && ObjectManager.Heroes.Enemies.Count(enemy => enemy.IsFacing(ObjectManager.Player)
                                                                  && enemy.Distance(ObjectManager.Player) <= spell.Range)
                   >= count;
        }

        public static AIHeroClient GetKillableTarget(Spell spell) => ObjectManager.Heroes.Enemies
            .FirstOrDefault(enemy => enemy.Distance(ObjectManager.Player) < spell.Range
                                     && enemy.Health < spell.GetDamage(enemy) && !enemy.IsInvulnerable);

        public static Obj_AI_Minion GetLastHittableMinion(Spell spell) => ObjectManager.MinionsAndMonsters.Enemy
            .FirstOrDefault(minion => minion.Distance(ObjectManager.Player) < spell.Range &&
                          spell.GetDamage(minion) > minion.Health);

        public static void CheckAndCastSpellAoe(Spell spell)
        {
            if (!spell.IsReady()) return;
            var target = TargetSelector.GetTarget(spell.Range, TargetSelector.DamageType.Magical,
                false);
            spell.Cast(target, aoe:true);
        }
        public static void CheckAndCastSpellTargeted(Spell spell)
        {
            if (!spell.IsReady()) return;
            var target = TargetSelector.GetTarget(spell.Range, TargetSelector.DamageType.Magical,
                false);
            spell.CastOnUnit(target);
        }

        public static bool PoisonConditions(AIHeroClient aiHeroClient)
        {
            return aiHeroClient.HasBuffOfType(BuffType.Poison);
        }
    }
}