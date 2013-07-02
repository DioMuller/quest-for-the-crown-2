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
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            map = MapLoader.LoadMap("Content\\maps\\Overworld01.tmx");
            tilesetTest = Content.Load<Texture2D>(map.Tilesets[0].Source);
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            //Bounding Box test!
            Rectangle rect = new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 32, 32);
            bool test = map.Collides(rect);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            foreach( Layer layer in map.Layers )
            {
                for( int y = 0; y < layer.Size.Y; y++ )
                {
                    for( int x = 0; x < layer.Size.X; x++ )
                    {
                        spriteBatch.Draw(tilesetTest, new Rectangle( x * map.TileSize.X, y * map.TileSize.Y, map.TileSize.X, map.TileSize.Y), map.Tilesets[0].GetRect( layer.GetData(x,y) ), Color.White);
                    }
                } 
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
