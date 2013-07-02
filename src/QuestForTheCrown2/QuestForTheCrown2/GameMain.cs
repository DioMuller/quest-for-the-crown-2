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

        Base.Input input = new Base.Input(Base.InputType.Controller);
        Base.Input input2 = new Base.Input(Base.InputType.Keyboard);

        static internal SpriteSheet MainCharacterSpriteSheet;
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

        #region Content Management

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

            #region MainCharacter
            var texture = Content.Load<Texture2D>(@"C:\Users\joao\Desktop\Lucas_RPG_Maker_Spritesheet_by_Boonzeet.png");

            TimeSpan mainCharacterWalkFrameDuration = TimeSpan.FromMilliseconds(100);
            MainCharacterSpriteSheet = new QuestForTheCrown2.Entities.Base.SpriteSheet(texture, new Point(45, 57));
            MainCharacterSpriteSheet.AddAnimation("stopped", "down", line: 0, count: 1, frameDuration: mainCharacterWalkFrameDuration);
            MainCharacterSpriteSheet.AddAnimation("stopped", "left", line: 1, count: 1, frameDuration: mainCharacterWalkFrameDuration);
            MainCharacterSpriteSheet.AddAnimation("stopped", "right", line: 2, count: 1, frameDuration: mainCharacterWalkFrameDuration);
            MainCharacterSpriteSheet.AddAnimation("stopped", "up", line: 3, count: 1, frameDuration: mainCharacterWalkFrameDuration);

            MainCharacterSpriteSheet.AddAnimation("walking", "down", line: 0, frameDuration: mainCharacterWalkFrameDuration);
            MainCharacterSpriteSheet.AddAnimation("walking", "left", line: 1, frameDuration: mainCharacterWalkFrameDuration);
            MainCharacterSpriteSheet.AddAnimation("walking", "right", line: 2, frameDuration: mainCharacterWalkFrameDuration);
            MainCharacterSpriteSheet.AddAnimation("walking", "up", line: 3, frameDuration: mainCharacterWalkFrameDuration);
            #endregion

            // Teste
            mainCharacter = new MainCharacter();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        #endregion

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (input.QuitButton || input2.QuitButton)
                Exit();

            // TODO: Add your update logic here

            // Teste
            mainCharacter.Update(gameTime);

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
            foreach (Layer layer in map.Layers)
            {
                for (int y = 0; y < layer.Size.Y; y++)
                {
                    for (int x = 0; x < layer.Size.X; x++)
                    {
                        spriteBatch.Draw(tilesetTest,
                            new Rectangle(x * map.TileSize.X, y * map.TileSize.Y, map.TileSize.X, map.TileSize.Y),
                            map.Tilesets[0].GetRect(layer.GetData(x, y)),
                            Color.White);
                    }
                }
            }

            // Teste
            spriteBatch.Draw(mainCharacter.CurrentFrame.Texture,
                mainCharacter.Location,
                mainCharacter.CurrentFrame.Rectangle, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
