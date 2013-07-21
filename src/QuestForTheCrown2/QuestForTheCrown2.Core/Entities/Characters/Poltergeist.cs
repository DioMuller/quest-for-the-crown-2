using Microsoft.Xna.Framework;
using QuestForTheCrown2.Base;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Entities.Weapons;
using System;

namespace QuestForTheCrown2.Entities.Characters
{
    class Poltergeist : Entity
    {
        public Poltergeist()
            : base("sprites/characters/oldman.png", 4, 1)
        {
            TimeSpan stoppedFrameDuration = TimeSpan.FromMilliseconds(300);
            SpriteSheet.AddAnimation("stopped", "down", frameIndexes: new int[] { 0 }, frameDuration: stoppedFrameDuration);
            SpriteSheet.AddAnimation("stopped", "left", frameIndexes: new int[] { 1 }, frameDuration: stoppedFrameDuration);
            SpriteSheet.AddAnimation("stopped", "right", frameIndexes: new int[] { 2 }, frameDuration: stoppedFrameDuration);
            SpriteSheet.AddAnimation("stopped", "top", frameIndexes: new int[] { 3 }, frameDuration: stoppedFrameDuration);

            Padding = new Rectangle(0, 11, 0, 0);

            Health = new Container(10);
            Look(new Vector2(0, 1), true);
        }

        public override void Hit(Entity attacker, GameTime gameTime, Levels.Level level, Vector2 direction)
        {
            var previewEnemy = PreviewEnemyLocation(gameTime, level, attacker.Parent, attacker.Speed);

            //TODO: Projectile base class.
            var arrow = attacker as Arrow;
            if (arrow != null)
            {
                attacker.CurrentDirection = previewEnemy.Position.Normalized();
                attacker.Parent = this;
                arrow.HitEntity = null;
                return;
            }

            var fireBall = attacker as FireBall;
            if (fireBall != null)
            {
                attacker.CurrentDirection = previewEnemy.Position.Normalized();
                attacker.Parent = this;
                fireBall.HitEntity = null;
                return;
            }

            if (attacker is Boomerang)
                base.Hit(attacker, gameTime, level, direction);
        }
    }
}
