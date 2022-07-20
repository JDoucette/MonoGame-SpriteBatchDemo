// Jason Allen Doucette
// July 15, 2022
// SpriteBatch Demo
//
// PURPOSE:
// Testing Draw() with scaling with PointClamp / PointWrap
// also with transforms, and to see if it works or ruins "nearest neighbour" style rendering.
// And also using sprite sheets, to see if it changes things.
//
// CONCLUSION:
// "Nearest neighbour" is maintained with PointClamp / PointWrap, even with matrix transforms.
// However, the texel access out-of-bounds range is for the texture, not the sub-texture within the sprite sheet.
// This means floating point inaccuracies that occur with the zoom factor is non-integer dips into out-of-range pixels.
// In our case, our tile sprite sheet has no borders, so out-of-bounds texel access is accessing am adjacent tile.
//
// TODO:
//	1.	Implement camera scrolling, through a level, to check for floating point precision issues as the magnitudes rise.
//	2.	Allow keyboard control:  zoom, rotate, and recenter

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace SpriteBatchDemo
{
	public class Game1 : Game
	{
		// ---- constants

		// resolution
		private static Point sizeResScreen =
#if DEBUG
			new Point(1280, 720);
#else
			new Point(1920, 1080);
#endif


		// ---- properties

		public GraphicsDeviceManager GetGraphics { get { return graphics; } }
		public Art GetArt { get { return art; } }
		public Tiles GetTiles { get { return tiles; } }
		public Controller GetController { get { return controller; } }
		public Render GetRender { get { return render; } }


		// ---- data members

		// systems
		private Art art;
		private Tiles tiles;
		private Render render;
		private Controller controller;

		// graphics
		private readonly GraphicsDeviceManager graphics;


		// ---- methods

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = sizeResScreen.X,
				PreferredBackBufferHeight = sizeResScreen.Y,
#if DEBUG
				IsFullScreen = false,
#else
				IsFullScreen = true,
#endif
				SynchronizeWithVerticalRetrace = true
			};

			this.IsFixedTimeStep = false;
			this.TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 240.0);  // not used unless IsFixedTimeStep = true
			Content.RootDirectory = "Content";
		}

		protected override void Initialize()
		{
			Initialize_before_LoadedContent();
			base.Initialize();  // calls LoadContent()
			Initialize_after_LoadedContent();
		}

		private void Initialize_before_LoadedContent()
		{
			art = new Art(GraphicsDevice);
			tiles = new Tiles(this);
			render = new Render(this, GraphicsDevice);
			controller = new Controller(this);
		}

		private void Initialize_after_LoadedContent()
		{
			tiles.GenerateTiles();
		}

		protected override void LoadContent()
		{
			tiles.CreateSprites();
			render.LoadContent(Content);
		}

		protected override void UnloadContent()
		{
		}

		protected override void Update(GameTime gameTime)
		{
			controller.Update(gameTime);
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			render.Render_LowResGameScreen(gameTime);
			render.Render_LowResGameScreen_to_BackBuffer();
			render.Render_Hud_SpriteSheet(tiles.GetTextSpriteSheet);
			render.Render_Hud_IndividualSprites(tiles.GetSizeTilePixels);

			base.Draw(gameTime);
		}

	}  // 	public class Game1 : Game
}  // namespace SpriteBatchDemo
