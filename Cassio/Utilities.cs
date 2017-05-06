using System.Linq;
using HesaEngine.SDK;
using HesaEngine.SDK.Enums;
using HesaEngine.SDK.GameObjects;

namespace Cassio
{
    public class Utilities
    {
        public static AIHeroClient GetKillableTarget(Spell spell) => ObjectManager.Heroes.Enemies
            .FirstOrDefault(enemy => enemy.Distance(ObjectManager.Player) < spell.Range
                                     && enemy.Health < spell.GetDamage(enemy));

        public static Obj_AI_Minion GetLastHittableMinion(Spell spell) => ObjectManager.MinionsAndMonsters.Enemy
            .FirstOrDefault(minion => minion.Distance(ObjectManager.Player) < spell.Range &&
                          spell.GetDamage(minion) > minion.Health);

        public static void CheckAndCastSpellAoe(Spell spell)
        {
            if (spell.IsReady())
            {
                var target = TargetSelector.GetTarget(spell.Range, TargetSelector.DamageType.Magical,
                    false);
                spell.Cast(target, aoe:true);
            }
        }
        public static void CheckAndCastSpellTargeted(Spell spell)
        {
            if (spell.IsReady())
            {
                var target = TargetSelector.GetTarget(spell.Range, TargetSelector.DamageType.Magical,
                    false);
                spell.CastOnUnit(target);
            }
        }

        public static bool PoisonConditions(AIHeroClient aiHeroClient)
        {
            return aiHeroClient.HasBuffOfType(BuffType.Poison);
        }
    }
}