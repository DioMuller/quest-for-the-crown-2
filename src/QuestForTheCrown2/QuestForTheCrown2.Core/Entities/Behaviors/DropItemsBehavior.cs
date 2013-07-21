using Microsoft.Xna.Framework;
using QuestForTheCrown2.Base;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Entities.Objects;
using QuestForTheCrown2.Entities.Weapons;
using QuestForTheCrown2.Levels;
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

        List<Func<GameTime, Level, Entity>> _additionalItems = new List<Func<GameTime, Level, Entity>>();

        public bool AutomaticAllowWeapons { get; set; }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, Levels.Level level)
        {
            base.Update(gameTime, level);

            if (_itemsDropped)
                return;
            _itemsDropped = true;

            bool itemsDropped = false;

            var weapons = Entity.Weapons ?? Enumerable.Empty<Entity>();
            var items = Entity.Holding ?? Enumerable.Empty<Entity>();
            var additional = _additionalItems.Select(f => f(gameTime, level)).Where(i => i != null);

            foreach (var ent in weapons.Union(items).Union(additional))
            {
                var dropEntity = ent;
                if (ent is Weapon)
                {
                    var weaponName = ent.GetType().Name;
                    var allowedWeapons = GameStateManager.CurrentState.AllowWeapon;
                    var currentWeapons = GameStateManager.CurrentState.Player.Weapons;

                    if (AutomaticAllowWeapons && !allowedWeapons.Contains(weaponName))
                        allowedWeapons.Add(weaponName);

                    if (currentWeapons.Contains(weaponName))
                    {
                        if (CreateAmmo.ContainsKey(weaponName) && Random.NextDouble() > 0.1)
                            dropEntity = CreateAmmo[weaponName]();

                        continue;
                    }
                }

                Drop(level, dropEntity);
                itemsDropped = true;
            }

            Entity.Weapons = null;
            Entity.Holding = null;

            if (!itemsDropped && Random.NextDouble() < 0.1 && !GameStateManager.CurrentState.Player.Health.IsFull)
                Drop(level, new Health());
        }

        private void Drop(Levels.Level level, Base.Entity dropEntity)
        {
            var enemyPosition = Entity.CenterPosition;
            var dropDistance = VectorHelper.AngleToV2((float)(Random.NextDouble() * Math.PI - Math.PI / 2), 32);
            var dropPosition = enemyPosition + dropDistance;

            dropEntity.Parent = null;
            dropEntity.OverlapEntities = true;

            dropEntity.Position = dropPosition;
            if (level.Map.Collides(dropEntity.CollisionRect))
                dropEntity.Position = enemyPosition;
            level.AddEntity(dropEntity);
        }

        public void DropOnDeath(Func<GameTime, Level, Entity> createEntity)
        {
            _additionalItems.Add(createEntity);
        }
    }
}
