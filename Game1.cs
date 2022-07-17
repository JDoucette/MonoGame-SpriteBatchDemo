// Jason Allen Doucette
// July 15, 2022
// SpriteBatch Demo
//
// Testing Draw() with scaling with PointClamp / PointWrap
// also with transforms, and to see if it works or ruins neares neighbour.
// And also using sprite sheets, to see if it changes things.

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpriteBatchDemo
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		// ---- constants

		// resolution
#if DEBUG
		private static Point sizeResScreen = new Point(1280, 720);
#else
		private static Point sizeResScreen = new Point(1920, 1080);
#endif
		private const int scaleGame = 4;
		private Point sizeResGame = new Point(
			sizeResScreen.X / scaleGame, 
			sizeResScreen.Y / scaleGame);

		// sprites
		private Point sizeTile_pixels = new Point(16, 16);
		private Point sizeSpriteSheet_tiles = new Point(8, 8);


		// ---- data members

		private Art art;
		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
		private RenderTarget2D textLowResGame;
		private Texture2D textSprite;
		private Texture2D textWhite;


		// ---- methods

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = sizeResScreen.X,
				PreferredBackBufferHeight = sizeResScreen.Y
			};
#if DEBUG
			graphics.IsFullScreen = false;
#else
			graphics.IsFullScreen = true;
#endif
			graphics.SynchronizeWithVerticalRetrace = true;

			this.IsFixedTimeStep = false;
			this.TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 240.0);  // not used unless IsFixedTimeStep = true
			Content.RootDirectory = "Content";
		}

		protected override void Initialize()
		{
			art = new Art(GraphicsDevice);
			textLowResGame = new RenderTarget2D(GraphicsDevice, sizeResGame.X, sizeResGame.Y, mipMap: false, SurfaceFormat.Color, DepthFormat.None);
			base.Initialize();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);

			textSprite = art.CreateSpriteSheetTexture(sizeTile_pixels, sizeSpriteSheet_tiles);
			textWhite = art.CreateWhiteTexture(8);
		}

		protected override void UnloadContent()
		{
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || 
				Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			// 1. render target

			Render_LowResGameScreen(gameTime);

			// 2. back buffer

			Render_LowResGameScreen_to_BackBuffer();

			// 3. base class

			base.Draw(gameTime);
		}

		private void Render_LowResGameScreen(GameTime gameTime)
		{
			GraphicsDevice.SetRenderTarget((RenderTarget2D)textLowResGame);
			GraphicsDevice.Clear(Color.CornflowerBlue);

			Render_LowResGameScreen_Backdrop();
			Render_LowResGameScreen_Foreground(gameTime);
		}

		private void Render_LowResGameScreen_Foreground(GameTime gameTime)
		{
			double timeTotal = gameTime.TotalGameTime.TotalSeconds;

			// exact copy of Kris Steele's transform matrix for PK, with changes to screen size & draw position:
			Vector2 DrawPosition = new Vector2(
				sizeResGame.X * 0.125f,
				sizeResGame.Y * 0.125f);
			float Rotation = 0.0f; // (float)(timeTotal * 0.5);
			float Zoom = (float)(2.0 + Math.Sin(timeTotal * 0.5));
			Vector2 origin = new Vector2(sizeResGame.X * 0.5f, sizeResGame.Y * 0.5f);  // for rotation and zoom
			Matrix transformMatrix =
				Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *
				Matrix.CreateRotationZ(Rotation) *
				Matrix.CreateScale(Zoom) *
				Matrix.CreateTranslation(new Vector3(origin, 0.0f));

			spriteBatch.Begin(
				SpriteSortMode.Deferred,
				BlendState.AlphaBlend,
				SamplerState.PointWrap,
				DepthStencilState.None,
				RasterizerState.CullNone,
				effect: null,
				transformMatrix);

			// render a set of tiles
			Point sizeSpriteArray = new Point(
				sizeResGame.X / sizeTile_pixels.X,
				sizeResGame.Y / sizeTile_pixels.Y);
			Random rng = new Random(0);
			for (int y = 0; y < sizeSpriteArray.Y; y++)
				for (int x = 0; x < sizeSpriteArray.X; x++)
				{
					// pick random tile
					Point tile = new Point(
						rng.Next(sizeSpriteSheet_tiles.X),
						rng.Next(sizeSpriteSheet_tiles.Y));

					// sprite within spritesheet
					Rectangle rectSource = new Rectangle(
						tile.X * sizeTile_pixels.X,
						tile.Y * sizeTile_pixels.Y, 
						sizeTile_pixels.X, 
						sizeTile_pixels.Y);

					Vector2 pos = new Vector2(
						x * sizeTile_pixels.X,
						y * sizeTile_pixels.Y);
					spriteBatch.Draw(textSprite, pos, rectSource, Color.White);
				}
			spriteBatch.End();
		}

		private void Render_LowResGameScreen_Backdrop()
		{
			spriteBatch.Begin();
			// show off the primary colors
			spriteBatch.Draw(textWhite, new Rectangle(0, 0, 16, 16), Color.White);
			spriteBatch.Draw(textWhite, new Rectangle(0, 16, 16, 16), new Color(255, 0, 0));
			spriteBatch.Draw(textWhite, new Rectangle(0, 32, 16, 16), new Color(0, 255, 0));
			spriteBatch.Draw(textWhite, new Rectangle(0, 48, 16, 16), new Color(0, 0, 255));
			// random 1x1 dots, to show the pixel size of the low resolution screen
			Random rng = new Random(0);
			for (int i = 0; i < 4096; i++)
				spriteBatch.Draw(
					textWhite,
					new Rectangle(rng.Next(sizeResGame.X), rng.Next(sizeResScreen.Y), 1, 1),
					new Color(rng.Next(256), rng.Next(256), rng.Next(256)));
			spriteBatch.End();
		}

		private void Render_LowResGameScreen_to_BackBuffer()
		{
			GraphicsDevice.SetRenderTarget(null);
			GraphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin(
				SpriteSortMode.Deferred,
				BlendState.AlphaBlend,
				SamplerState.PointWrap,
				DepthStencilState.None,
				RasterizerState.CullNone,
				effect: null,
				transformMatrix: null);
			spriteBatch.Draw(
				textLowResGame,
				position: Vector2.Zero,
				sourceRectangle: null,
				Color.White,
				rotation: 0.0f,
				origin: new Vector2(0, 0),
				scale: scaleGame,
				SpriteEffects.None,
				layerDepth: 0.0f);
			spriteBatch.End();
		}

	}  // 	public class Game1 : Game
}  // namespace SpriteBatchDemo
