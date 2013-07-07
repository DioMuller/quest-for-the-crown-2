using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using QuestForTheCrown2.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestForTheCrown2.Levels.Mapping;
using QuestForTheCrown2.Levels;

namespace QuestForTheCrown2.Entities.Characters
{
    public class Player : Entity
    {
        #region Constants
        /// <summary>
        /// Spritesheet path
        /// </summary>
        const string spriteSheetPath = @"sprites\MainCharacter.png";
        #endregion Constants

        #region Attributes
        private int _playerNumber;
        #endregion Attributes

        #region Properties
        /// <summary>
        /// Player number (1-4)
        /// </summary>
        public int PlayerNumber 
        { 
            get
            {
                return _playerNumber;
            }   
            set
            {
                if( value > 0 && value < 5 )
                {
                    _playerNumber = value;
                }
            }
        }

        /// <summary>
        /// Current dungeon the player is in.
        /// </summary>
        public int CurrentDungeon { get; set; }

        /// <summary>
        /// Current level the player is in.
        /// </summary>
        public int CurrentLevel { get; set; }

        /// <summary>
        /// Indicates if the current entity is transitioning between levels.
        /// </summary>
        public Direction LevelTransitionDirection { get; set; }
        /// <summary>
        /// Gets or sets the level in which the current entity is transitioning to.
        /// </summary>
        public int TransitioningToLevel { get; set; }

        /// <summary>
        /// Indicates in which state the current level transition is at.
        /// Value range from 0 to 1.
        /// </summary>
        public float LevelTransitionPercent { get; set; }
        #endregion Properties

        #region Constructor
        /// <summary>
        /// Builds main character with its base spritesheet and animations.
        /// </summary>
        public Player()
            : base(spriteSheetPath, new Point(22, 28))
        {
            Category = "Player";

            TimeSpan walkFrameDuration = TimeSpan.FromMilliseconds(100);
            SpriteSheet.AddAnimation("stopped", "down", line: 0, count: 1, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("stopped", "left", line: 1, count: 1, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("stopped", "right", line: 2, count: 1, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("stopped", "up", line: 3, count: 1, frameDuration: walkFrameDuration);

            SpriteSheet.AddAnimation("walking", "down", line: 0, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("walking", "left", line: 1, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("walking", "right", line: 2, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("walking", "up", line: 3, frameDuration: walkFrameDuration);

            Padding = new Rectangle(2, 10, 2, 2);

            Speed = new Vector2(96);

            Health = 5;
        }
        #endregion Constructor
    }
}
