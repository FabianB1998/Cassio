using System;
using System.Linq;
using HesaEngine.SDK;
using HesaEngine.SDK.Enums;

namespace Cassio
{
    public class Cassio : IScript
    {
        private readonly Spell _q = new Spell(SpellSlot.Q, true);
        private readonly Spell _w = new Spell(SpellSlot.W, true);
        private readonly Spell _e = new Spell(SpellSlot.E, true);
        private readonly Spell _r = new Spell(SpellSlot.R, true);

        private Menu _rootMenu;

        public void OnInitialize()
        {
            Game.OnGameLoaded += GameOnOnGameLoaded;
            Game.OnTick += GameOnOnTick;
        }

        public string Name => "Cassio";
        public string Version => "0.1.0";
        public string Author => "Fabian";

        private void GameOnOnGameLoaded()
        {
            Chat.Print("Loading Cassiopeia");
            _rootMenu = Menu.AddMenu("Cassiopeia");
            _rootMenu.Add(new MenuSlider("limitOnTick", "Limit ticks (Higher limit has better performance)",
                0, 30, 0));
            var comboMenu = _rootMenu.AddSubMenu("Combo");
            var laneClearAndLasthitMenu = _rootMenu.AddSubMenu("Laneclear & Lasthit");
            comboMenu.Add(new MenuCheckbox("useQCombo", "Use Q in Combo"));
            comboMenu.Add(new MenuCheckbox("useWCombo", "Use W in Combo"));
            comboMenu.Add(new MenuCheckbox("useECombo", "Use E in Combo"));
            comboMenu.Add(new MenuCheckbox("useRCombo", "Use R in Combo"));
            comboMenu.Add(new MenuSlider("useRMinimum", "Use R if it will stun atleast", 0, 5, 2));
            comboMenu.Add(new MenuCheckbox("focusPoisioned", "Focus poisioned enemies"));
            laneClearAndLasthitMenu.Add(new MenuCheckbox("useELasthit", "Use E in Laneclears"));
        }

        private void GameOnOnTick()
        {
            Utilities.Tick();
            var targetTicks = _rootMenu.Get<MenuSlider>("limitOnTick").CurrentValue;
            if (!Utilities.ShouldWork(targetTicks)) return;
            //Check if anyone is killable with a single e and try to get the kill
            var killableTarget = Utilities.GetKillableTarget(_e);
            if (killableTarget != null && _e.CanCast(killableTarget))
            {
                _e.CastOnUnit(killableTarget);
            }

            var firstOrDefault = Orbwalker.OrbwalkerInstance.Instances.FirstOrDefault();
            if (firstOrDefault != null && firstOrDefault
                    .ActiveMode.HasFlag(Orbwalker.OrbwalkingMode.Combo))
            {
                Combo();
            }
            else if (firstOrDefault != null && (firstOrDefault
                                                    .ActiveMode.HasFlag(Orbwalker.OrbwalkingMode.LaneClear) ||
                                                firstOrDefault.ActiveMode.HasFlag(Orbwalker.OrbwalkingMode
                                                    .LastHit)))
            {
                LaneClear();
            }
        }

        private void LaneClear()
        {

            var target = Utilities.GetLastHittableMinion(_e);
            if (target == null) return;
            if (_e.CanCast(target))
            {
                _e.CastOnUnit(target);
            }
        }

        private void Combo()
        {

            //Check if you can ult targets
            foreach (var hero in ObjectManager.Heroes.Enemies)
            {
                if (_r.IsReady() && Utilities.WillStunMinimum(_r,
                        _rootMenu.SubMenu("Combo").Get<MenuSlider>("useRMinimum").CurrentValue, hero.Position))
                {
                    _r.Cast(hero);
                }
            }

            if (_rootMenu.SubMenu("Combo").Get<MenuCheckbox>("focusPoisioned").Checked)
                            {
                                if (_e.IsReady() && _rootMenu.SubMenu("Combo").Get<MenuCheckbox>("useECombo").Checked)
                                {
                                    var target = TargetSelector.GetTarget(_e.Range, TargetSelector.DamageType.Magical,
                                        false, conditions: Utilities.PoisonConditions);
                                    if (target != null)
                                    {

                                        _e.CastOnUnit(target);
                                    }
                                    else
                                    {
                                        Utilities.CheckAndCastSpellTargeted(_e);
                                    }
                                }
                                else
                                {
                                    if (_rootMenu.SubMenu("Combo").Get<MenuCheckbox>("useQCombo").Checked)
                                    {
                                        Utilities.CheckAndCastSpellAoe(_q);
                                    }
                                    if (_rootMenu.SubMenu("Combo").Get<MenuCheckbox>("useWCombo").Checked)
                                    {
                                        Utilities.CheckAndCastSpellAoe(_w);
                                    }
                                }

                            }
                            else
                            {
                                if (_rootMenu.SubMenu("Combo").Get<MenuCheckbox>("useECombo").Checked)
                                {
                                    Utilities.CheckAndCastSpellTargeted(_e);
                                }
                                if (_rootMenu.SubMenu("Combo").Get<MenuCheckbox>("useQCombo").Checked)
                                {
                                    Utilities.CheckAndCastSpellAoe(_q);
                                }
                                if (_rootMenu.SubMenu("Combo").Get<MenuCheckbox>("useWCombo").Checked)
                                {
                                    Utilities.CheckAndCastSpellAoe(_w);
                                }
                            }
        }
    }
}
