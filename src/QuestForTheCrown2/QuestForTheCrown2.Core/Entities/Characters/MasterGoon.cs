﻿using Microsoft.Xna.Framework;
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
    class MasterGoon : Entity
    {
        const string spriteSheetPath = @"sprites\Characters\master-goon.png";

        public MasterGoon()
            : base(spriteSheetPath, new Point(96, 96))
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

            Speed = 32 * 3.5f;

            Health = new Container(8);

            Look(new Vector2(0, 1), true);

            AddBehavior(
                new HitOnTouchBehavior(e => e.Category == "Player"),
                new AvoidBehavior(e => e.Category == "Player", 32 * 4),
                new BowAttackBehavior("Player", shootDistance: 32 * 30, maxDistance: 32 * 32),
                new WalkAroundBehavior()
            );
            AddWeapon(new Bow());

            GetBehavior<DropItemsBehavior>().AutomaticAllowWeapons = true;

            Arrows = new Container(20);
        }

        public override void Hit(Entity attacker, GameTime gameTime, Levels.Level level, Vector2 direction)
        {
            if (attacker is Arrow)
                Health.Quantity -= 2;

            base.Hit(attacker, gameTime, level, direction);
        }
    }
}
