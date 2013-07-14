using Microsoft.Xna.Framework;
using QuestForTheCrown2.Base;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Entities.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Behaviors
{
    class DropWeaponsBehavior : DieBehavior
    {
        Dictionary<string, Func<Entity>> CreateAmmo = new Dictionary<string, Func<Entity>>
        {
            { "Bow", () => new Arrow { PickupCount = 10 } }
        };

        public bool AutomaticAllowWeapons { get; set; }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, Levels.Level level)
        {
            base.Update(gameTime, level);

            if (Entity.Weapons != null)
            {
                foreach (var weapon in Entity.Weapons)
                {
                    var weaponName = weapon.GetType().Name;
                    var allowedWeapons = GameStateManager.CurrentState.AllowWeapon;
                    var currentWeapons = GameStateManager.CurrentState.Player.Weapons;

                    if (AutomaticAllowWeapons && !allowedWeapons.Contains(weaponName))
                        allowedWeapons.Add(weaponName);

                    if (allowedWeapons.Contains(weaponName) || currentWeapons.Contains(weaponName))
                    {
                        var dropDistance = VectorHelper.AngleToV2((float)(Random.NextDouble() * Math.PI - Math.PI / 2), 32);
                        var dropPosition = Entity.CenterPosition + dropDistance;
                        if (!currentWeapons.Contains(weaponName))
                        {
                            weapon.Parent = null;
                            weapon.Position = dropPosition;
                            level.AddEntity(weapon);
                        }
                        else if (CreateAmmo.ContainsKey(weaponName))
                        {
                            if (Random.NextDouble() < 0.1)
                            {
                                var ammo = CreateAmmo[weaponName]();
                                ammo.Position = dropPosition;
                                level.AddEntity(ammo);
                            }
                        }
                    }
                }
            }
        }
    }
}
