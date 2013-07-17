using Microsoft.Xna.Framework;
using QuestForTheCrown2.Base;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Entities.Objects;
using QuestForTheCrown2.Entities.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Behaviors
{
    class DropItemsBehavior : DieBehavior
    {
        bool _itemsDropped = false;
        Dictionary<string, Func<Entity>> CreateAmmo = new Dictionary<string, Func<Entity>>
        {
            { "Bow", () => new Arrow { PickupCount = 10 } }
        };

        public bool AutomaticAllowWeapons { get; set; }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, Levels.Level level)
        {
            base.Update(gameTime, level);

            if (_itemsDropped)
                return;
            _itemsDropped = true;

            bool itemsDropped = false;

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
                            itemsDropped = true;

                            weapon.Parent = null;
                            weapon.Position = dropPosition;
                            level.AddEntity(weapon);
                        }
                        else if (CreateAmmo.ContainsKey(weaponName))
                        {
                            if (Random.NextDouble() < 0.1 && !GameStateManager.CurrentState.Player.Health.IsFull)
                            {
                                itemsDropped = true;

                                var ammo = CreateAmmo[weaponName]();
                                ammo.Position = dropPosition;
                                level.AddEntity(ammo);
                            }
                        }
                    }
                }
            }

            if (!itemsDropped && Random.NextDouble() < 0.1)
            {
                var dropDistance = VectorHelper.AngleToV2((float)(Random.NextDouble() * Math.PI - Math.PI / 2), 32);
                var dropPosition = Entity.CenterPosition + dropDistance;

                var health = new Health();
                health.Position = dropPosition;
                level.AddEntity(health);
            }
        }
    }
}
