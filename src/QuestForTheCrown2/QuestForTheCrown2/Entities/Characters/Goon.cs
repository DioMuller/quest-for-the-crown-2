using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using QuestForTheCrown2.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestForTheCrown2.Levels.Mapping;
using QuestForTheCrown2.Entities.Behaviors;
using QuestForTheCrown2.Entities.Weapons;

namespace QuestForTheCrown2.Entities.Characters
{
    class Goon : Entity
    {
        const string spriteSheetPath = @"sprites\Characters\Goon.png";

        public Goon()
            : base(spriteSheetPath, new Point(64, 64))
        {
            Category = "Enemy";

            TimeSpan walkFrameDuration = TimeSpan.FromMilliseconds(100);
            SpriteSheet.AddAnimation("stopped", "up", line: 8, count: 1, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("stopped", "left", line: 9, count: 1, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("stopped", "down", line: 10, count: 1, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("stopped", "right", line: 11, count: 1, frameDuration: walkFrameDuration);

            SpriteSheet.AddAnimation("walking", "up", line: 8, count: 6, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("walking", "left", line: 9, count: 6, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("walking", "down", line: 10, count: 6, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("walking", "right", line: 11, count: 6, frameDuration: walkFrameDuration);

            Padding = new Rectangle(28, 36, 28, 2);

            Speed = new Vector2(32 * 3);

            Health = 4;

            Look(new Vector2(0, 1), true);

            AddBehavior(
                new HitOnTouchBehavior(e => e.Category == "Player"),
                new BlinkBehavior(TimeSpan.FromSeconds(0.5)),
                new FollowBehavior("Player", 5) { MaxDistance = 32 * 6 },
                new WalkAroundBehavior()
            );
        }

        public override void Hit(Entity attacker, Levels.Level level, Vector2 direction)
        {
            if (attacker is Arrow)
                Health--;

            base.Hit(attacker, level, direction);
        }
    }
}
