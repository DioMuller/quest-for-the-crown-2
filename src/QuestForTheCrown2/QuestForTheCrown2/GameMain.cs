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

using QuestForTheCrown2.External.Tiled;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Entities.Characters;
using QuestForTheCrown2.Entities.Behaviors;

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
        Texture2D tilesetTest;

        Base.Input input = new Base.Input();

        // Teste
        MainCharacter mainCharacter;

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

            map = MapLoader.LoadMap("Content\\maps\\Overworld01.tmx");
            tilesetTest = Content.Load<Texture2D>(map.Tilesets[0].Source);

            // Teste
            mainCharacter = new MainCharacter { Position = new Vector2(32 * 4, 32 * 4) };
            mainCharacter.AddBehavior(new InputWalkBehavior(input));
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
            foreach (Layer layer in map.Layers)
            {
                for (int y = 0; y < layer.Size.Y; y++)
                {
                    for (int x = 0; x < layer.Size.X; x++)
                    {
                        spriteBatch.Draw(tilesetTest,
                            new Rectangle((int)(x * map.TileSize.X - camera.X + Window.ClientBounds.Width / 2),
                                (int)(y * map.TileSize.Y - camera.Y + Window.ClientBounds.Height / 2),
                                map.TileSize.X,
                                map.TileSize.Y),
                            map.Tilesets[0].GetRect(layer.GetData(x, y)),
                            Color.White);
                    }
                }
            }

            spriteBatch.Draw(mainCharacter.CurrentFrame.Texture,
                new Vector2(
                    mainCharacter.Position.X - camera.X + Window.ClientBounds.Width / 2,
                    mainCharacter.Position.Y - camera.Y + Window.ClientBounds.Height / 2),
                mainCharacter.CurrentFrame.Rectangle, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
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

            camera = new Vector2(newX, newY);
            return camera;
        }
    }
}
