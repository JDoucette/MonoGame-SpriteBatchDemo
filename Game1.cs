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

		private Point sizeScreen = new Point(1024, 768);
		private Point size = new Point(32, 32);


		// ---- data members

		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
		private Texture2D texture;


		// ---- methods

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = sizeScreen.X;
			graphics.PreferredBackBufferHeight = sizeScreen.Y;
			Content.RootDirectory = "Content";
		}

		protected override void Initialize()
		{
			base.Initialize();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);

			texture = CreateTexture();
		}

		private Texture2D CreateTexture()
		{
			int numPixels = size.X * size.Y;
			Color[] colors = new Color[numPixels];
			for (int y = 0; y < size.Y; y++)
				for (int x = 0; x < size.X; x++)
				{
					bool parity = ((x + y) & 1) == 1;
					int v = (parity ? 255 : 0);
					colors[y * size.X + x] = new Color(v, v, v);
				}
			texture = new Texture2D(GraphicsDevice, size.X, size.Y);
			texture.SetData<Color>(colors);
			return texture;
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

			spriteBatch.Begin();
			spriteBatch.Draw(texture, new Vector2(0, 0), Color.White);
			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
