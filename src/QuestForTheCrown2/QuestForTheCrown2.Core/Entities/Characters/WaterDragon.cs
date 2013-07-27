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
using QuestForTheCrown2.Base;
using QuestForTheCrown2.Entities.Weapons;
using QuestForTheCrown2.Entities.Objects;

namespace QuestForTheCrown2.Entities.Characters
{
    class WaterDragon : Entity
    {
        const string spriteSheetPath = @"sprites\Characters\water-dragon.png";
        float _baseSpeed;

        public WaterDragon()
            : base(spriteSheetPath, new Point(149, 129))
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

            Padding = new Rectangle(23, 50, 23, 20);

            _baseSpeed = 32 * 4;
            Speed = 32 * 4;

            Health = new Container(10);
            Magic = new Container(1);

            Health.ValueChanged += Health_ValueChanged;
            Magic.ValueChanged += Magic_ValueChanged;

            Look(new Vector2(0, 1), true);

            AddBehavior(
                new HitOnTouchBehavior(e => e.Category == "Player"),
                new FireWandAttackBehavior("Player", 32 * 5, 32 * 10),
                new WalkAroundBehavior { MaxStoppedTime = TimeSpan.Zero, SpeedMultiplier = 1 }
            );
            AddWeapon(new FireWand { MaxFireBallFlyTime = TimeSpan.FromSeconds(2) });

            Hold(new HealthContainer("WaterDragon"));
        }

        void Magic_ValueChanged(object sender, EventArgs e)
        {
            if (Magic.Quantity <= 0)
                Magic.Quantity = 1;
        }

        void Health_ValueChanged(object sender, EventArgs e)
        {
            Speed = _baseSpeed * (2 - (float)Health.Quantity / Health.Maximum.Value);
        }

        public override void Update(GameTime gameTime, Levels.Level level)
        {
            base.Update(gameTime, level);
        }
    }
}
