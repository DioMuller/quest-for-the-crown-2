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
        Weapon _currentWeapon;

        int _timeSinceLastMpRegen;
        TimeSpan _lastSavedPositionTime;
        Vector2? _lastSavedPosition;
        int _lastSavesPositionLevel;

        string _currentView, _nextView;
        int _directionFrameCount;
        #endregion

        #region Properties
        public Entity Parent { get; set; }

        /// <summary>
        /// Indicates in which category this entity is at.
        /// Values could be: "Player", "Enemy", "Item" and etc.
        /// </summary>
        public string Category { get; set; }

        #region Containers

        public bool IsDead
        {
            get { return Health != null && Health <= 0; }
            set { Health = new Container(0, 0); }
        }

        public Container Health
        {
            get { return Containers.GetOrDefault("Health"); }
            set
            {
                if (Behaviors == null || GetBehavior<BlinkBehavior>() == null)
                    AddBehavior(new BlinkBehavior(TimeSpan.FromSeconds(0.5)), new DropItemsBehavior());
                if (GetBehavior<DropItemsBehavior>() == null)
                    AddBehavior(new DropItemsBehavior());

                Containers["Health"] = value;
            }
        }

        public Container Magic
        {
            get { return Containers.GetOrDefault("Magic"); }
            set { Containers["Magic"] = value; }
        }

        public Container Arrows
        {
            get { return Containers.GetOrDefault("Arrows"); }
            set { Containers["Arrows"] = value; }
        }
        #endregion

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
        public List<Weapon> Weapons { get; set; }

        /// <summary>
        /// Reference to the current entity's weapon.
        /// </summary>
        public Weapon CurrentWeapon
        {
            get { return _currentWeapon; }
        }

        /// <summary>
        /// Status of the entity's ammo, health and magic.
        /// </summary>
        public Dictionary<string, Container> Containers { get; set; }

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
        public virtual string CurrentAnimation { get; set; }

        /// <summary>
        /// Indicates the current animation view.
        /// Examples are: "left", "bottom", "right" and "down".
        /// </summary>
        public string CurrentView
        {
            get { return _currentView; }
            set
            {
                if (_currentView == null)
                {
                    _currentView = value;
                    return;
                }

                if (value != _nextView)
                {
                    _directionFrameCount = 0;
                    _nextView = value;
                }

                if (++_directionFrameCount > 1)
                    _currentView = value;
            }
        }

        #endregion

        #region Position
        public Vector2 Position { get; set; }

        /// <summary>
        /// Entity speed, in pixels per second.
        /// </summary>
        public float Speed { get; set; }
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
        public float Angle { get; set; }

        /// <summary>
        /// The draw point of the entity, the default is (0,0) which represents the upper-left corner.
        /// </summary>
        public Vector2 Origin { get; protected set; }

        /// <summary>
        /// Collision Rectangle.
        /// </summary>
        public virtual Rectangle CollisionRect
        {
            get
            {
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

        #region Events
        public event EventHandler AnimationEnded;
        #endregion

        #region Constructors
        private Entity()
        {
            Containers = new Dictionary<string, Container>();
        }

        public Entity(string spriteSheetPath, int columns, int lines)
            : this()
        {
            var texture = GameContent.LoadContent<Texture2D>(spriteSheetPath);
            SpriteSheet = new SpriteSheet(texture, new Point(texture.Width / columns, texture.Height / lines));
            _framesPerLine = columns;

            Speed = SpriteSheet.FrameSize.X;
        }

        public Entity(string spriteSheetPath, Point? frameSize)
            : this()
        {
            SpriteSheet = new SpriteSheet(GameContent.LoadContent<Texture2D>(spriteSheetPath), frameSize);
            _framesPerLine = SpriteSheet.Texture.Width / SpriteSheet.FrameSize.X;

            Speed = SpriteSheet.FrameSize.X;
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
        /// Finds the behavior of the specified type.
        /// </summary>
        /// <typeparam name="TBehavior"></typeparam>
        /// <returns></returns>
        public TBehavior GetBehavior<TBehavior>() where TBehavior : EntityUpdateBehavior
        {
            return Behaviors.SelectMany(b => b.Value.OfType<TBehavior>()).FirstOrDefault();
        }

        /// <summary>
        /// Attach weapons to the current entity.
        /// </summary>
        /// <param name="weapons"></param>
        public void AddWeapon(params Weapon[] weapons)
        {
            if (Weapons == null)
                Weapons = new List<Weapon>();

            Weapons.AddRange(weapons);

            foreach (var weapon in weapons)
                weapon.Parent = this;
        }

        public void RemoveWeapon(Weapon weapon)
        {
            if (Weapons == null)
                return;

            Weapons.Remove(weapon);
            weapon.Parent = null;
        }

        public virtual void Look(Vector2 direction, bool updateDirection)
        {
            if (direction == Vector2.Zero)
                return;

            if (Math.Abs(direction.X) >= Math.Abs(direction.Y))
            {
                if (direction.X > 0)
                {
                    if (++_directionFrameCount > 1)
                        CurrentView = "right";
                }
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

        public virtual void ChangeWeapon(Weapon nextWeapon, Level level)
        {
            if (nextWeapon == _currentWeapon)
                return;

            var oldWeapon = _currentWeapon;
            if (oldWeapon != null)
                oldWeapon.Unequiped(level);
            _currentWeapon = nextWeapon;
            if (_currentWeapon != null)
                _currentWeapon.Equiped(level);
        }

        public virtual Vector2 PreviewLocation(GameTime gameTime, Level level, TimeSpan time)
        {
            if (_lastSavedPosition == null || _lastSavesPositionLevel != level.Id)
                return CenterPosition;

            var distanceWalked = CenterPosition - _lastSavedPosition.Value;
            var timePassed = gameTime.TotalGameTime - _lastSavedPositionTime;

            if (timePassed == TimeSpan.Zero)
                return CenterPosition;

            var ratio = time.TotalSeconds / timePassed.TotalSeconds;
            return CenterPosition + distanceWalked * (float)ratio;
        }

        public virtual Vector2 PreviewEnemyLocation(GameTime gameTime, Level level, Entity enemy, float? speed = null)
        {
            Vector2 relativePosition = enemy.CenterPosition - CenterPosition;
            var dist = relativePosition.Length();

            for (int i = 0; i < 2; i++)
            {
                if (double.IsNaN(dist))
                    return enemy.CenterPosition;

                var enemyPos = enemy.PreviewLocation(gameTime, level, TimeSpan.FromSeconds(dist / (speed ?? Speed)));
                relativePosition = enemyPos - CenterPosition;
                dist = relativePosition.Length();
            }

            return relativePosition;
        }

        #region Containers
        public int? ContainerMaximum(string containerName)
        {
            if (!Containers.ContainsKey(containerName))
                return null;
            return Containers[containerName].Maximum;
        }

        public int? ContainerQuantity(string containerName)
        {
            if (!Containers.ContainsKey(containerName))
                return null;
            return Containers[containerName].Quantity;
        }

        public bool IncreaseQuantity(string containerName, int byQuantity = 1)
        {
            Container container;
            if (!Containers.TryGetValue(containerName, out container))
                return false;

            if (container.Quantity >= container.Maximum)
                return false;

            container.Quantity = (int)Math.Min(container.Maximum.Value, container.Quantity + byQuantity);
            return true;
        }

        public bool DecreaseQuantity(string containerName, int byQuantity = 1)
        {
            Container container;
            if (!Containers.TryGetValue(containerName, out container))
                return false;

            if (container.Quantity < byQuantity)
                return false;

            container.Quantity = container.Quantity - byQuantity;
            return true;
        }
        #endregion
        #endregion

        #region Update
        public virtual void Update(GameTime gameTime, Level level)
        {
            if (_lastSavedPositionTime + TimeSpan.FromSeconds(0.3) < gameTime.TotalGameTime)
            {
                _lastSavedPosition = CenterPosition;
                _lastSavedPositionTime = gameTime.TotalGameTime;
                _lastSavesPositionLevel = level.Id;
            }

            if (Behaviors != null)
            {
                var activeBehaviors = new List<EntityUpdateBehavior>();

                foreach (var behaviorGroup in Behaviors)
                {
                    if (behaviorGroup.Key == string.Empty)
                        foreach (var behavior in behaviorGroup.Value.Where(b => b.IsActive(gameTime, level)))
                            activeBehaviors.Add(behavior);
                    else
                    {
                        var bh = behaviorGroup.Value.Where(b => b.IsActive(gameTime, level)).FirstOrDefault();
                        if (bh != null)
                            activeBehaviors.Add(bh);
                    }
                }

                foreach (var bh in Behaviors.SelectMany(b => b.Value))
                {
                    if (activeBehaviors.Contains(bh))
                        bh.Update(gameTime, level);
                    else
                        bh.Deactivated(gameTime, level);
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
                _currentFrameIndex++;
                if (_currentFrameIndex >= curAnimation.FrameIndexes.Length)
                {
                    if (AnimationEnded != null)
                        AnimationEnded(this, EventArgs.Empty);
                    _currentFrameIndex = curAnimation.ResetToFrame;
                }
                _lastFrameStartTime = gameTime.TotalGameTime;
            }

            if (Magic != null)
            {
                if (_timeSinceLastMpRegen > 1000)
                {
                    if (!Magic.IsFull) Magic.Quantity++;
                    _timeSinceLastMpRegen = 0;
                }
                else
                {
                    _timeSinceLastMpRegen += gameTime.ElapsedGameTime.Milliseconds;
                }
            }
        }

        /// <summary>
        /// Hit this entity in the desired angle.
        /// </summary>
        /// <param name="level">Current level</param>
        /// <param name="angle">Projectile angle</param>
        public virtual void Hit(Entity attacker, Level level, Vector2 direction)
        {
            if (Health == null)
                return;

            if (!IsBlinking)
            {
                SoundManager.PlaySound("hit");
                Health.Quantity--;
            }

            var oldPos = Position;
            Position += direction;
            if (level.CollidesWith(CollisionRect).Any(e => e != this) || level.Map.Collides(CollisionRect))
                Position = oldPos;
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
            try
            {
                bestAnimation = bestAnimations[CurrentView];
            }
            catch
            {
                bestAnimation = bestAnimations["default"];
            }

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
