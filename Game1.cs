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

		private Point sizeResScreen = new Point(1024, 768);
		private Point sizeResGame = new Point(320, 240);
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
			GraphicsDevice.Clear(Color.CornflowerBlue);

			GraphicsDevice.SetRenderTarget((RenderTarget2D)textGame);

			spriteBatch.Begin();
			spriteBatch.Draw(textSprite, new Vector2(0, 0), Color.White);
			spriteBatch.End();

			GraphicsDevice.SetRenderTarget(null);

			spriteBatch.Begin();
			Vector2 pos = new Vector2(0, 0);
			float rotation = 0.0f;
			float scale = 8.0f;
			float depth = 0.0f;
			spriteBatch.Draw(textGame, pos, null, Color.White, rotation, new Vector2(0,0), scale, SpriteEffects.None, depth);
			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
