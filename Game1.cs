// Jason Allen Doucette
// July 15, 2022
// SpriteBatch Demo
//
// Testing Draw() with scaling with PointClamp / PointWrap
// also with transforms, and to see if it works or ruins neares neighbour.
// And also using sprite sheets, to see if it changes things.
//
// TODO:
//	1.	Try rendering without sprite sheet, where each tile is its own Texture2D.
//		When the transform coordinates go outside of texture coordinates, it should Clamp or Wrap.
//		Whereas with the sprite sheet, it appears neither of these prevent out-of-range, as it applies to the texture, not the sub-texture.

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Text;

namespace SpriteBatchDemo
{
	public class Game1 : Game
	{
		// ---- constants

		private readonly StringBuilder strTitle = new StringBuilder("SpriteBatchDemo");
		private readonly StringBuilder str = new StringBuilder(256);

		// sampler state
		//private static SamplerState samplerState = SamplerState.PointWrap;
		private static readonly SamplerState samplerState = SamplerState.PointClamp;

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

		// matrix transform
		private float rotate;
		private float zoom;


		// ---- data members

		private Art art;
		private readonly GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
		private RenderTarget2D textLowResGame;
		private Texture2D textSpriteSheet;
		private Texture2D textWhite;
		private Font font;


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

			textSpriteSheet = art.CreateSpriteSheetTexture(sizeTile_pixels, sizeSpriteSheet_tiles);
			textWhite = art.CreateWhiteTexture(8);

			//font = new Font(Content.Load<Texture2D>(@"Fonts\font-arcade-classic-7x7-jason-edit"));
			//font = new Font(Content.Load<Texture2D>(@"Fonts\font-jason-5x6-fixed"));
			font = new Font(Content.Load<Texture2D>(@"Fonts\font-jason-7x8-fixed-double-bold"));
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
			Render_LowResGameScreen(gameTime);
			Render_LowResGameScreen_to_BackBuffer();
			Render_SpriteSheet();

			base.Draw(gameTime);
		}

		private void Render_LowResGameScreen(GameTime gameTime)
		{
			GraphicsDevice.SetRenderTarget((RenderTarget2D)textLowResGame);
			GraphicsDevice.Clear(new Color(0, 0, 0));

			Render_LowResGameScreen_Foreground(gameTime);
			Render_LowResGameScreen_HUD();
		}

		private void Render_LowResGameScreen_Foreground(GameTime gameTime)
		{
			double timeTotal = gameTime.TotalGameTime.TotalSeconds;

			// exact copy of Kris Steele's transform matrix for PK, with changes to screen size & draw position:
			rotate = 0.0f;  // (float)(timeTotal * 0.2746593);
			zoom = (float)(3.0 + 2.0 * Math.Sin(timeTotal * 0.482658202));
			Vector2 origin = new Vector2(sizeResGame.X * 0.5f, sizeResGame.Y * 0.5f);  // for rotation and zoom
			Matrix transformMatrix =
				Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *
				Matrix.CreateRotationZ(rotate) *
				Matrix.CreateScale(zoom) *
				Matrix.CreateTranslation(new Vector3(origin, 0.0f));

			spriteBatch.Begin(
				SpriteSortMode.Deferred,
				BlendState.AlphaBlend,
				samplerState,
				DepthStencilState.None,
				RasterizerState.CullNone,
				effect: null,
				transformMatrix);
			{

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
						spriteBatch.Draw(textSpriteSheet, pos, rectSource, Color.White);
					}
			}
			spriteBatch.End();
		}

		private void Render_LowResGameScreen_HUD()
		{
			spriteBatch.Begin();
			{
				Vector2 pos = new Vector2(1, 1);
				font.Draw(spriteBatch, strTitle, pos, Color.White);
				pos.Y += font.FontHeight;

				str.Clear().AppendFormat("Zoom: {0}", zoom);
				font.Draw(spriteBatch, str, pos, Color.White);
				pos.Y += font.FontHeight;

				str.Clear().AppendFormat("Rotate: {0}", rotate);
				font.Draw(spriteBatch, str, pos, Color.White);
				pos.Y += font.FontHeight;
			}
			spriteBatch.End();
		}

		private void Render_LowResGameScreen_to_BackBuffer()
		{
			GraphicsDevice.SetRenderTarget(null);
			GraphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin(
				SpriteSortMode.Deferred,
				BlendState.AlphaBlend,
				samplerState,
				DepthStencilState.None,
				RasterizerState.CullNone,
				effect: null,
				transformMatrix: null);
			{

				// 1. zoomed in
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

				// 2. actual size
				// lower-left
				// border
				spriteBatch.Draw(textWhite,
					new Rectangle(
						0, sizeResScreen.Y - textLowResGame.Height - 2,
						sizeResGame.X + 2, sizeResGame.Y + 2),
					Color.Black);
				// game screen
				spriteBatch.Draw(textLowResGame, new Vector2(1, sizeResScreen.Y - textLowResGame.Height - 1), Color.White);

			}
			spriteBatch.End();
		}

		private void Render_SpriteSheet()
		{
			spriteBatch.Begin();
			{
				// upper-right

				// border
				spriteBatch.Draw(textWhite,
					new Rectangle(
						sizeResScreen.X - textSpriteSheet.Width - 2, 0,
						textSpriteSheet.Width + 2, textSpriteSheet.Height + 2),
					Color.Black);
				// sprite sheet
				spriteBatch.Draw(textSpriteSheet,
					new Vector2(sizeResScreen.X - textSpriteSheet.Width - 1, 0),
					Color.White);
			}
			spriteBatch.End();
		}

	}  // 	public class Game1 : Game
}  // namespace SpriteBatchDemo
