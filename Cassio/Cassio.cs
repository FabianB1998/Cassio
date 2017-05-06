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
            var comboMenu = _rootMenu.AddSubMenu("Combo");
            var laneClearAndLasthitMenu = _rootMenu.AddSubMenu("Laneclear & Lasthit");
            comboMenu.Add(new MenuCheckbox("useQCombo", "Use Q in Combo"));
            comboMenu.Add(new MenuCheckbox("useWCombo", "Use W in Combo"));
            comboMenu.Add(new MenuCheckbox("useECombo", "Use E in Combo"));
            comboMenu.Add(new MenuCheckbox("useRCombo", "Use R in Combo"));
            comboMenu.Add(new MenuCheckbox("focusPoisioned", "Focus poisioned enemies"));
            laneClearAndLasthitMenu.Add(new MenuCheckbox("useELasthit", "Use E in Laneclears"));
        }

        private void GameOnOnTick()
        {
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
                         firstOrDefault.ActiveMode.HasFlag(Orbwalker.OrbwalkingMode.LastHit)))
            {
                LaneClear();
            }

        }

        private void LaneClear()
        {
            var target = Utilities.GetLastHittableMinion(_e);
            if (target != null)
            {
                if (_e.CanCast(target))
                {
                    _e.CastOnUnit(target);
                }
            }
        }

        private void Combo()
        {
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
