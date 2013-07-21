using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using QuestForTheCrown2.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestForTheCrown2.Levels.Mapping;
using QuestForTheCrown2.Entities.Weapons;
using QuestForTheCrown2.Entities.Behaviors;

namespace QuestForTheCrown2.Entities.Characters
{
    class BoomerangSkeleton : Entity
    {
        const string spriteSheetPath = @"sprites\Characters\boomerang-skeleton.png";

        public BoomerangSkeleton()
            : base(spriteSheetPath, 3, 4)
        {
            Category = "Enemy";

            TimeSpan walkFrameDuration = TimeSpan.FromMilliseconds(100);
            SpriteSheet.AddAnimation("stopped", "down", line: 0, count: 1, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("stopped", "left", line: 1, count: 1, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("stopped", "right", line: 2, count: 1, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("stopped", "up", line: 3, count: 1, frameDuration: walkFrameDuration);

            SpriteSheet.AddAnimation("walking", "down", line: 0, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("walking", "left", line: 1, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("walking", "right", line: 2, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("walking", "up", line: 3, frameDuration: walkFrameDuration);

            Padding = new Rectangle(4, 38, 4, 2);

            Speed = 32 * 3;

            Health = new Container(10);
            Magic = new Container(10);

            CurrentDirection = new Vector2(0, 1);

            AddBehavior(
                new HitOnTouchBehavior(e => e.Category == "Player"),
                new SwordAttackBehavior("Player") { MaxDistance = 32 * 2 },
                new AvoidBehavior(e => e.Category == "Player", 32 * 4),
                new BoomerangAttackBehavior("Player", shootDistance: 32 * 30, maxDistance: 32 * 32),// { MinDistance = 5 * 32 },
                new FollowBehavior("Player", 5) { MaxDistance = float.MaxValue }
            );
            AddWeapon(new Boomerang());
            AddWeapon(new Sword());

            GetBehavior<DropItemsBehavior>().AutomaticAllowWeapons = true;
        }

        public override void Hit(Entity attacker, GameTime gameTime, Levels.Level level, Vector2 direction)
        {
            if (attacker is FireBall)
                Health.Quantity -= 2;

            base.Hit(attacker, gameTime, level, direction);
        }
    }
}
