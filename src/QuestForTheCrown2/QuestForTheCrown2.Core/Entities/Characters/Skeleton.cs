﻿using Microsoft.Xna.Framework;
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
    class Skeleton : Entity
    {
        const string spriteSheetPath = @"sprites\Characters\skeleton.png";

        public Skeleton()
            : base(spriteSheetPath, new Point(32, 64))
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

            Health = new Container(5);

            CurrentDirection = new Vector2(0, 1);

            AddBehavior(
                new HitOnTouchBehavior(e => e.Category == "Player"),
                new SwordAttackBehavior("Player") { MaxDistance = 300 },
                new FollowBehavior("Player", 5) { MaxDistance = float.MaxValue }//,
                //new WalkAroundBehavior { MaxStoppedTime = TimeSpan.FromSeconds(2) }
            );
            AddWeapon(new Sword());
        }

        public override void Hit(Entity attacker, GameTime gameTime, Levels.Level level, Vector2 direction)
        {
            if (attacker is FireBall)
                Health.Quantity -= 2;

            base.Hit(attacker, gameTime, level, direction);
        }
    }
}
