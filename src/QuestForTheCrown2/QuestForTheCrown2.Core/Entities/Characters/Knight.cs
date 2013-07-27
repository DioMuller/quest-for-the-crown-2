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
using QuestForTheCrown2.Entities.Objects;

namespace QuestForTheCrown2.Entities.Characters
{
    class Knight : Entity
    {
        const string spriteSheetPath = @"sprites/Characters/knight.png";

        public Knight()
            : base(spriteSheetPath, 9, 13)
        {
            Category = "Enemy";

            TimeSpan walkFrameDuration = TimeSpan.FromMilliseconds(100);
            SpriteSheet.AddAnimation("stopped", "up", line: 0, count: 1, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("stopped", "left", line: 1, count: 1, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("stopped", "down", line: 2, count: 1, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("stopped", "right", line: 3, count: 1, frameDuration: walkFrameDuration);


            SpriteSheet.AddAnimation("walking", "up", line: 0, skipFrames: 1, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("walking", "left", line: 1, skipFrames: 1, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("walking", "down", line: 2, skipFrames: 1, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("walking", "right", line: 3, skipFrames: 1, frameDuration: walkFrameDuration);

            SpriteSheet.AddAnimation("dying", "default", line: 12, count: 6, frameDuration: walkFrameDuration, repeat: false);

            Padding = new Rectangle(4, 20, 4, 2);

            Speed = 32 * 2.5f;

            Look(new Vector2(0, 1), true);

            AddBehavior(
                new HitOnTouchBehavior(e => e.Category == "Player"),
                new SwordAttackBehavior("Player") { MaxDistance = 300 },
                new WalkAroundBehavior()
            );
            AddWeapon(new Sword());

            Hold(new HealthContainer("Knight"));
        }

        public override void Hit(Entity attacker, GameTime gameTime, Levels.Level level, Vector2 direction)
        {
            var oldHealth = Health.Quantity;
            base.Hit(attacker, gameTime, level, direction);

            if (level.GetEntities<Poltergeist>().Any())
                Health.Quantity = oldHealth;
        }

        public override void Update(GameTime gameTime, Levels.Level level)
        {
            if (Health == null)
                Health = level.GetEntities<Poltergeist>().Select(p => p.Health).FirstOrDefault() ?? new Container(7);

            base.Update(gameTime, level);
        }
    }
}
