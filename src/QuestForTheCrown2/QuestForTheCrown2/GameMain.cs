#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

using QuestForTheCrown2.Levels.Mapping;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Entities.Characters;
using QuestForTheCrown2.Entities.Behaviors;
using QuestForTheCrown2.Base;
using QuestForTheCrown2.Entities.Weapons;

namespace QuestForTheCrown2
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameMain : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Map map;
        Levels.LevelCollection overworld;

        Base.Input input = new Base.Input();

        // Teste
        Entity mainCharacter, enemy1;

        public GameMain()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            GameContent.Initialize(Content);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            map = MapLoader.LoadMap("Content\\maps\\Overworld-01.tmx");
            overworld = MapLoader.LoadLevels("Content/dungeons/Overworld.qfc");

            // Teste
            mainCharacter = new Enemy1 { Position = new Vector2(32 * 4, 32 * 4) };
            mainCharacter.AddBehavior(
                new InputBehavior(Base.InputType.Controller),
                new InputBehavior(Base.InputType.Keyboard)
            );
            mainCharacter.AddWeapon(new Sword());

            enemy1 = new MainCharacter { Position = new Vector2(32 * 8, 32 * 8) };
            enemy1.AddBehavior(
                new FollowBehavior { Following = mainCharacter }
            );
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (input.QuitButton)
                Exit();

            // TODO: Add your update logic here

            // Teste
            mainCharacter.Update(gameTime, map);
            enemy1.Update(gameTime, map);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            var camera = GetCameraPosition(mainCharacter, map.PixelSize, Window.ClientBounds);

            spriteBatch.Begin();

            map.Draw(gameTime, spriteBatch, camera);

            Draw(mainCharacter, camera);
            Draw(enemy1, camera);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void Draw(Entity entity, Vector2 camera)
        {
            var frame = entity.CurrentFrame;
            spriteBatch.Draw(frame.Texture,
                new Vector2(
                    entity.Position.X - camera.X,
                    entity.Position.Y - camera.Y),
                frame.Rectangle, Color.White);
        }

        static Vector2 GetCameraPosition(Entity entity, Point mapSize, Rectangle screenSize)
        {
            var camera = entity.Position;
            var newX = entity.Position.X;
            var newY = entity.Position.Y;

            if (newX < screenSize.Width / 2)
                newX = screenSize.Width / 2;
            else if (newX > mapSize.X - screenSize.Width / 2)
                newX = mapSize.X - screenSize.Width / 2;

            if (newY < screenSize.Height / 2)
                newY = screenSize.Height / 2;
            else if (newY > mapSize.Y - screenSize.Height / 2)
                newY = mapSize.Y - screenSize.Height / 2;

            camera = new Vector2(
                newX - screenSize.Width / 2,
                newY - screenSize.Height / 2);
            return camera;
        }
    }
}
