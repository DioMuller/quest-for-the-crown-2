using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Entities.Characters;

namespace QuestForTheCrown2.Entities.Objects
{
    /// <summary>
    /// Dungeon entrance class.
    /// </summary>
    class Entrance : Entity
    {
        public const string Teleportable = "Player";

        public int Dungeon { get; private set; }

        public Entrance(int dungeon) : base(@"sprites/Empty.png", new Point(32, 32))
        {
            IsInvisible = true;
            Dungeon = dungeon;  
            OverlapEntities = true; 
            Health = null;
            SpriteSheet.AddAnimation("stopped", "down", line: 0, count: 1, frameDuration: TimeSpan.FromDays(1)); 
        }

        public override void Update(GameTime gameTime, Levels.Level level)
        {
            foreach( Entity en in level.CollidesWith(this.CollisionRect) )
            {
                //TODO: Remove en is Player
                if( Teleportable.Split(';').Contains(en.Category)) 
                {
                    level.GoToDungeon(en, Dungeon);
                }
            }
            //base.Update(gameTime, level);
        }
    }
}
