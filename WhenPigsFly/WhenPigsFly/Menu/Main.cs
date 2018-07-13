// ------------------------------
//
// Author: Zack Meadows
// Project: When Pigs Fly
//
// Created 11/27/2015
//
// ------------------------------
// Main // Game Class
// Created 11/27/2015
// ------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace WhenPigsFly
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Main : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState key_cache;

        // --------- Menu Animations -------------
        private Song menu_song;
        private WorldParallax Menu_Parallax;
        private Texture2D Title;
        private List<Texture2D> Menu_Animation = new List<Texture2D>();

        private int animation_index = 0;
        private int animation_delay = 7;
        private int animation_counter = 0;

        // --------- Scene Declaration -------------
        private Splashscreen splashscreen;
        public MainMenu mainMenu;
        private ControlsScene controlScene;

        private PlayScene playScene;
            public DeadScene deadScene;

        private AboutScene aboutScene;
        private CreditScene creditScene;
        // -----------------------------------------

        public void HideAllScenes()
        {
            Scene gs = null;
            foreach (GameComponent item in Components)
            {
                if (item is Scene)
                {
                    gs = (Scene)item;
                    gs.Hide();
                }
            }
        }
        public Main()
        {
            key_cache = new KeyboardState();
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = (int)Shared.RESOLUTION.X;
            graphics.PreferredBackBufferHeight = (int)Shared.RESOLUTION.Y;

            Shared.GfxDevice = GraphicsDevice;

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
            this.IsMouseVisible = true;
            base.Initialize();
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // --------- Defaults  -------------
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // --------- Store Globals  -------------
            Shared.Batch = spriteBatch;
            Shared.Main = this;

            // --------- Load Assets -------------
            Shared.IMG_Interface = AssetLoader.LoadContent<Texture2D>(Content, "Images/Interface");
            Shared.IMG_Items = AssetLoader.LoadContent<Texture2D>(Content, "Images/Items");
            Shared.IMG_Sprites = AssetLoader.LoadContent<Texture2D>(Content, "Images/Sprites");
            Shared.IMG_Worlds = AssetLoader.LoadContent<Texture2D>(Content, "Images/Worlds");

            Shared.SFX_Sounds = AssetLoader.LoadContent<SoundEffect>(Content, "Audio/SFX");
            Shared.AUDIO_Music = AssetLoader.LoadContent<Song>(Content, "Audio/Music");

            // --------- Load & Generate Particles & Effects -------------
            Shared.IMG_Animations = AssetLoader.LoadContent<Texture2D>(Content, "Images/Effects/Animations");
            Shared.IMG_Emotes = AssetLoader.LoadContent<Texture2D>(Content, "Images/Effects/Emotes");
            Shared.IMG_Projectiles = AssetLoader.LoadContent<Texture2D>(Content, "Images/Effects/Projectiles");
            Particles.Generate_Effects();

            // --------- Build Tilesheets -------------
            Shared.IMG_Tiles = AssetLoader.LoadContent<Texture2D>(Content, "Images/Objects/Tiles");
            Shared.Tileset = new Dictionary<string, Tileset>();
            Terrain.Generate_Tiles(Shared.IMG_Tiles);

            // --------- Build Parallaxs --------
            Shared.IMG_Parallax = AssetLoader.LoadContent<Texture2D>(Content, "Images/Parallax");
            Shared.World_Parallax = new Dictionary<string, WorldParallax>();
            WorldParallax.Create_WorldParallax();

            // --------- Debug Data -------------
            Shared.Pixel = new Texture2D(Shared.Main.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Shared.Pixel.SetData(new[] { Color.White });

            SpriteFont font = Content.Load<SpriteFont>("Fonts/Pigsfont");
            Shared.DebugWindow = new BasicText(font, new Vector2(0, 100), "Test", Color.Lime);

            // --------- Set Up Menu Animation -------------
            MediaPlayer.Play(Shared.AUDIO_Music["Menu"]);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 1f;

            Menu_Animation.Add(Shared.IMG_Interface["MenuSceneF1"]);
            Menu_Animation.Add(Shared.IMG_Interface["MenuSceneF2"]);
            Menu_Animation.Add(Shared.IMG_Interface["MenuSceneF3"]);
            Menu_Animation.Add(Shared.IMG_Interface["MenuSceneF4"]);

            // --------- Scene Creation -------------

            // Menu Display -------------------------
            Title = Shared.IMG_Interface["TitleBar"];
            Menu_Parallax = Shared.World_Parallax["Menu"];
            // --------------------------------------

            mainMenu = new MainMenu();
            Shared.MainMenu = mainMenu;
            Components.Add(mainMenu);

            playScene = new PlayScene();
            Components.Add(playScene);

            deadScene = new DeadScene();
            Shared.DeadScene = deadScene;
            Components.Add(deadScene);

            controlScene = new ControlsScene();
            Components.Add(controlScene);

            aboutScene = new AboutScene();
            Components.Add(aboutScene);

            creditScene = new CreditScene();
            Components.Add(creditScene);

            splashscreen = new Splashscreen();
            Components.Add(splashscreen);

            HideAllScenes();
            splashscreen.Show();
            // -----------------------------------------

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // -----------------------------------------
            int selectedIndex = 0;
            KeyboardState ks = Keyboard.GetState();

            if (mainMenu.Enabled)
            {
                selectedIndex = mainMenu.Menu.SelectedIndex;

                //Selecting Start Game
                if (selectedIndex == 0 && ks.IsKeyDown(Keys.Enter) && ks.IsKeyDown(Keys.Enter) != key_cache.IsKeyDown(Keys.Enter))
                {
                    HideAllScenes();

                    MediaPlayer.Play(Shared.AUDIO_Music["Overworld"]);
                    MediaPlayer.Volume = 0.1f;
                    playScene.Show();

                    Shared.Player.Enabled = true;
                }

                //Selecting About
                if (selectedIndex == 1 && ks.IsKeyDown(Keys.Enter))
                {
                    HideAllScenes();
                    aboutScene.Show();
                }

                //Selecting Help
                if (selectedIndex == 2 && ks.IsKeyDown(Keys.Enter))
                {
                    HideAllScenes();
                    controlScene.Show();
                }

                //Selecting Credits
                if (selectedIndex == 3 && ks.IsKeyDown(Keys.Enter))
                {
                    HideAllScenes();
                    creditScene.Show();
                }

                //Selecting Exit
                if (selectedIndex == 4 && ks.IsKeyDown(Keys.Enter))
                {
                    Exit();
                }
            }

            if(!mainMenu.Enabled)
            {
                if (ks.IsKeyDown(Keys.Escape))
                {
                    if (playScene.Enabled)
                    {
                        MediaPlayer.Play(Shared.AUDIO_Music["Menu"]);
                        MediaPlayer.Volume = 1f;
                    }

                    HideAllScenes();
                    mainMenu.Show();

                    Shared.Player.Enabled = false;
                }
            }

            // Death Scene
            // ------------------------
            if (playScene.Enabled)
            {
                if (Shared.Player.Is_dead())
                    deadScene.Show();
                else
                    deadScene.Hide();
            }
            else
                deadScene.Hide();
            // ------------------------

            // -----------------------------------------
            // Additional Control Options
            if (ks.IsKeyDown(Shared.KEY_FULLSCREEN) && (ks != key_cache))
            {
                Shared.Fullscreen = !Shared.Fullscreen;
                graphics.IsFullScreen = Shared.Fullscreen;
                graphics.ApplyChanges();
            }
            key_cache = ks;
            // -----------------------------------------
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            if (!playScene.Enabled)
            {
                Shared.Screen.Set_Zoom(2f);
                Shared.Screen.Position = new Vector2(0, 0);

                Menu_Parallax.Update(gameTime);

                // -----------------------------------------
                // Blurb controls Title screen animation & parallax
                animation_counter++;
                if (animation_counter >= animation_delay)
                {
                    animation_counter = 0;
                    animation_index++;

                    if (animation_index >= Menu_Animation.Count())
                        animation_index = 0;
                }

                Vector2 Animation_Pos = new Vector2
                (
                100,
                Shared.RESOLUTION.Y - Menu_Animation[animation_index].Height
                );

                // -----------------------------------------
                // Animation
                spriteBatch.Begin();
                spriteBatch.Draw(Menu_Animation[animation_index], Animation_Pos, Color.White);
                spriteBatch.End();
                // -----------------------------------------
                // Parallax
                spriteBatch.Begin();
                spriteBatch.Draw(Title, Vector2.Zero, Color.White);
                spriteBatch.End();
                // -----------------------------------------
            }
            base.Draw(gameTime);
        }
    }
}
