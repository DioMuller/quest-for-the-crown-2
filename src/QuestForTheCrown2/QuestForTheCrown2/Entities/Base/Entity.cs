using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestForTheCrown2.Levels.Mapping;
using QuestForTheCrown2.Base;
using QuestForTheCrown2.Entities.Weapons;
using QuestForTheCrown2.Levels;
using QuestForTheCrown2.Entities.Behaviors;

namespace QuestForTheCrown2.Entities.Base
{
    public class Entity
    {
        #region Attributes
        int _currentFrameIndex;
        readonly int _framesPerLine;
        Animation _lastAnimation;
        TimeSpan _lastFrameStartTime;
        //private int _playerNumber;
        #endregion

        #region Properties
        public Entity Parent { get; set; }

        /// <summary>
        /// Indicates in which category this entity is at.
        /// Values could be: "Player", "Enemy", "Item" and etc.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// The current entity's health.
        /// Null if it can't be destroyed.
        /// </summary>
        public int? Health { get; set; }

        /// <summary>
        /// Indicates if the current entity cannot be hit.
        /// </summary>
        public bool IsBlinking { get; set; }

        /// <summary>
        /// Contains all the entities behavior, that will be used during the entity's update logic.
        /// </summary>
        public Dictionary<string, List<EntityUpdateBehavior>> Behaviors { get; private set; }

        /// <summary>
        /// A list of all weapons available to this entity.
        /// </summary>
        public List<IWeapon> Weapons { get; private set; }

        #region Draw

        public SpriteSheet SpriteSheet { get; private set; }

        public Frame CurrentFrame
        {
            get
            {
                Animation animation = SelectAnimation();
                var index = animation.FrameIndexes[_currentFrameIndex];

                return new Frame
                {
                    Texture = SpriteSheet.Texture,
                    Rectangle = new Rectangle(
                        (index % _framesPerLine) * SpriteSheet.FrameSize.X,
                        (index / _framesPerLine) * SpriteSheet.FrameSize.Y,
                        SpriteSheet.FrameSize.X,
                        SpriteSheet.FrameSize.Y)
                };
            }
        }

        /// <summary>
        /// Indicates if the current entity will not be drawn.
        /// </summary>
        public bool IsInvisible { get; set; }

        /// <summary>
        /// Indicates the current entity animation.
        /// Examples are: "walking", "stopped" and "running".
        /// </summary>
        public string CurrentAnimation { get; set; }

        /// <summary>
        /// Indicates the current animation view.
        /// Examples are: "left", "bottom", "right" and "down".
        /// </summary>
        public string CurrentView { get; set; }

        #endregion

        #region Position
        public Vector2 Position { get; set; }
        public Vector2 Speed { get; set; }
        public Vector2 CurrentDirection { get; set; }

        /// <summary>
        /// Entity size
        /// </summary>
        public Point Size
        {
            get { return SpriteSheet.FrameSize; }
        }

        /// <summary>
        /// Gets the central position of the entity, in relation to the level.
        /// </summary>
        public Vector2 CenterPosition
        {
            get
            {
                return new Vector2(
                    x: Position.X + (Size.X - Padding.X - Padding.Width) / 2 + Padding.X,
                    y: Position.Y + (Size.Y - Padding.Y - Padding.Height) / 2 + Padding.Y);
            }
        }

        /// <summary>
        /// Padding Rectangle. The Width and Height properties are actually the right and bottom margins.
        /// </summary>
        public Rectangle Padding { get; protected set; }

        /// <summary>
        /// The current entity rotation angle.
        /// This is used during the Draw method, in conjunction with the RotationCenter.
        /// </summary>
        public float Angle { get; protected set; }

        /// <summary>
        /// The draw point of the entity, the default is (0,0) which represents the upper-left corner.
        /// </summary>
        public Vector2 Origin { get; protected set; }

        /// <summary>
        /// Collision Rectangle.
        /// </summary>
        public Rectangle CollisionRect
        {
            get
            {
                if (Angle != 0)
                    throw new NotImplementedException();

                return new Rectangle(
                    x: (int)(Position.X + Padding.X),
                    y: (int)(Position.Y + Padding.Y),
                    width: Size.X - Padding.X - Padding.Width,
                    height: Size.Y - Padding.Y - Padding.Height);
            }
        }

        /// <summary>
        /// Indicates if the current entity can overlap another entity.
        /// </summary>
        public bool OverlapEntities { get; set; }

        #endregion

        #region Level
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
        #endregion
        #endregion

        #region Constructors
        public Entity(string spriteSheetPath, Point? frameSize)
        {
            SpriteSheet = new SpriteSheet(GameContent.LoadContent<Texture2D>(spriteSheetPath), frameSize);
            _framesPerLine = SpriteSheet.Texture.Width / SpriteSheet.FrameSize.X;

            Speed = new Vector2(SpriteSheet.FrameSize.X, SpriteSheet.FrameSize.X);

            AddBehavior(new BlinkBehavior());
        }
        #endregion

        #region Methods

        /// <summary>
        /// Attach update methods to this entity.
        /// </summary>
        /// <param name="behaviors">Behaviors to be executed during this entity's update method.</param>
        public void AddBehavior(params EntityUpdateBehavior[] behaviors)
        {
            if (Behaviors == null)
                Behaviors = new Dictionary<string, List<EntityUpdateBehavior>>();

            foreach (var behavior in behaviors)
            {
                var group = behavior.Group ?? string.Empty;
                behavior.Entity = this;
                if (Behaviors.ContainsKey(group))
                    Behaviors[group].Add(behavior);
                else
                    Behaviors[group] = new List<EntityUpdateBehavior> { behavior };
            }
        }

        /// <summary>
        /// Attach weapons to the current entity.
        /// </summary>
        /// <param name="weapons"></param>
        public void AddWeapon(params IWeapon[] weapons)
        {
            if (Weapons == null)
                Weapons = new List<IWeapon>();

            Weapons.AddRange(weapons);
        }

        public virtual void Look(Vector2 direction, bool updateDirection)
        {
            if (direction == Vector2.Zero)
                return;

            if (Math.Abs(direction.X) >= Math.Abs(direction.Y))
            {
                if (direction.X > 0)
                    CurrentView = "right";
                else if (direction.X < 0)
                    CurrentView = "left";
            }
            else
            {
                if (direction.Y < 0)
                    CurrentView = "up";
                else if (direction.Y > 0)
                    CurrentView = "down";
            }

            if (updateDirection)
                CurrentDirection = direction;
        }

        #endregion

        #region Update
        public virtual void Update(GameTime gameTime, Level level)
        {
            if (Behaviors != null)
            {
                foreach (var behaviorGroup in Behaviors)
                {
                    if (behaviorGroup.Key == string.Empty)
                        foreach (var behavior in behaviorGroup.Value.Where(b => b.IsActive(gameTime, level)))
                            behavior.Update(gameTime, level);
                    else
                    {
                        var bh = behaviorGroup.Value.Where(b => b.IsActive(gameTime, level)).FirstOrDefault();
                        if (bh != null)
                            bh.Update(gameTime, level);
                    }
                }
            }

            var curAnimation = SelectAnimation();
            if (curAnimation != _lastAnimation)
            {
                _lastFrameStartTime = gameTime.TotalGameTime;
                _lastAnimation = curAnimation;
                _currentFrameIndex = 0;
            }
            else if (gameTime.TotalGameTime > _lastFrameStartTime + curAnimation.FrameDuration)
            {
                _currentFrameIndex = (_currentFrameIndex + 1) % curAnimation.FrameIndexes.Length;
                _lastFrameStartTime = gameTime.TotalGameTime;
            }
        }
        #endregion

        #region Draw
        /// <summary>
        /// Selects the best animation based on the current view and animation name.
        /// </summary>
        /// <returns>The animation matching the entity's current view and animation</returns>
        Animation SelectAnimation()
        {
            Animation bestAnimation;
            Dictionary<string, Animation> bestAnimations;

            if (CurrentAnimation == null || !SpriteSheet.Animations.ContainsKey(CurrentAnimation))
                CurrentAnimation = SpriteSheet.Animations.First().Key;
            bestAnimations = SpriteSheet.Animations[CurrentAnimation];

            if (CurrentView == null || !bestAnimations.ContainsKey(CurrentView))
                CurrentView = bestAnimations.First().Key;
            bestAnimation = bestAnimations[CurrentView];

            return bestAnimation;
        }
        /// <summary>
        /// Draws the entity
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        /// <param name="spriteBatch">Sprite batch</param>
        /// <param name="camera">Current camera.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 camera)
        {
            var frame = CurrentFrame;
            spriteBatch.Draw(frame.Texture,
                new Vector2(
                    Position.X - camera.X,
                    Position.Y - camera.Y),
                frame.Rectangle, Color.White, Angle, Origin, 1, SpriteEffects.None, 1);
        }
        #endregion Draw
    }
}
