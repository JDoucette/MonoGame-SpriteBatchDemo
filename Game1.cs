// Jason Allen Doucette
// July 15, 2022
// SpriteBatch Demo
//
// Testing Draw() with scaling with PointClamp / PointWrap
// also with transforms, and to see if it works or ruins neares neighbour.
// And also using sprite sheets, to see if it changes things.

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

		private Point sizeResScreen = new Point(1920 - 256, 1080 - 256);
		private Point sizeResGame = new Point(256, 192);
		private Point sizeTexture = new Point(32, 32);


		// ---- data members

		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
		private RenderTarget2D textGame;
		private Texture2D textSprite;


		// ---- methods

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = sizeResScreen.X,
				PreferredBackBufferHeight = sizeResScreen.Y
			};
			Content.RootDirectory = "Content";
		}

		protected override void Initialize()
		{
			textGame = new RenderTarget2D(GraphicsDevice, sizeResGame.X, sizeResGame.Y, mipMap: false, SurfaceFormat.Color, DepthFormat.None);
			base.Initialize();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);

			textSprite = CreateTexture();
		}

		private Texture2D CreateTexture()
		{
			int numPixels = sizeTexture.X * sizeTexture.Y;
			Color[] colors = new Color[numPixels];
			for (int y = 0; y < sizeTexture.Y; y++)
				for (int x = 0; x < sizeTexture.X; x++)
				{
					bool parity = ((x + y) & 1) == 1;
					int v = (parity ? 255 : 0);
					colors[y * sizeTexture.X + x] = new Color(v, v, v);
				}
			textSprite = new Texture2D(GraphicsDevice, sizeTexture.X, sizeTexture.Y);
			textSprite.SetData<Color>(colors);
			return textSprite;
		}

		protected override void UnloadContent()
		{
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			// 1. render target

			RenderLowResGameScreen(gameTime);

			// 2. back buffer

			RenderGameScreen_to_BackBuffer();

			// 3. base class

			base.Draw(gameTime);
		}

		private void RenderLowResGameScreen(GameTime gameTime)
		{
			double timeTotal = gameTime.TotalGameTime.TotalSeconds;

			GraphicsDevice.SetRenderTarget((RenderTarget2D)textGame);
			GraphicsDevice.Clear(Color.Black);

			float scale = 1.0f;
			//float rotation = 0.0f;
			float rotation = (float)(timeTotal * 0.1);

			Matrix transformMatrix =
				//Matrix.CreateTranslation(new Vector3(0, 0, 0)) *
				//Matrix.CreateRotationX(rotation) *
				//Matrix.CreateRotationY(rotation) *
				Matrix.CreateRotationZ(rotation) *
				Matrix.CreateScale(scale)
				//Matrix.CreateTranslation(new Vector3(0, 0, 0))
				;

			spriteBatch.Begin(
				SpriteSortMode.Deferred,
				BlendState.AlphaBlend,
				SamplerState.PointWrap,
				DepthStencilState.None,
				RasterizerState.CullNone,
				effect: null,
				transformMatrix: transformMatrix
				//transformMatrix: null
				);
			Point sizeSpriteArray = new Point(4, 4);
			for (int y = 0; y < sizeSpriteArray.Y; y++)
				for (int x = 0; x < sizeSpriteArray.X; x++)
				{
					Vector2 pos = new Vector2(x * textSprite.Width, y * textSprite.Height);
					spriteBatch.Draw(textSprite, pos, Color.White);
				}
			spriteBatch.End();
		}

		private void RenderGameScreen_to_BackBuffer()
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
			float scaleGame = 8.0f;
			spriteBatch.Draw(
				textGame,
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


	}
}
