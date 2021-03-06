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

namespace QuestForTheCrown2.Entities.Characters
{
    class Zombie : Entity
    {
        const string spriteSheetPath = @"sprites\Characters\zombie.png";

        public Zombie()
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

            Health = new Container(3);

            Look(new Vector2(0, 1), true);

            AddBehavior(
                new HitOnTouchBehavior(e => e.Category == "Player"),
                new FollowBehavior("Player", 5) { MaxDistance = 32 * 10 },
                new WalkAroundBehavior { MaxStoppedTime = TimeSpan.Zero }
            );
        }
    }
}
