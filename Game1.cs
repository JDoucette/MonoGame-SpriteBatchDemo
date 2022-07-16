﻿// Jason Allen Doucette
// July 15, 2022
// SpriteBatch Demo
//
// Testing Draw() with scaling with PointClamp / PointWrap
// also with transforms, and to see if it works or ruins neares neighbour.
// And also using sprite sheets, to see if it changes things.

// TODO NEXT
// - try it using a sprite sheet

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

		private Point sizeResGame = new Point(256, 192);
		private Point sizeResScreen = new Point(1920, 1080);
		private Point sizeSpriteTexture = new Point(32, 32);


		// ---- data members

		private Art art;
		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
		private RenderTarget2D textGame;
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
			graphics.IsFullScreen = true;
			graphics.SynchronizeWithVerticalRetrace = true;

			this.IsFixedTimeStep = false;
			this.TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 240.0);  // not used unless IsFixedTimeStep = true
			Content.RootDirectory = "Content";
		}

		protected override void Initialize()
		{
			art = new Art(GraphicsDevice);
			textGame = new RenderTarget2D(GraphicsDevice, sizeResGame.X, sizeResGame.Y, mipMap: false, SurfaceFormat.Color, DepthFormat.None);
			base.Initialize();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);

			textSprite = art.CreateSpriteTexture(sizeSpriteTexture);
			textWhite = art.CreateWhiteTexture(8);
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

			////float scale = 1.0f;
			//float rotation = 0.0f;
			//float scale = (float)(2.0 + Math.Sin(timeTotal * 0.5));
			////float rotation = (float)(timeTotal * 0.1);

			//Matrix transformMatrix =
			//	//Matrix.CreateTranslation(new Vector3(0, 0, 0)) *
			//	//Matrix.CreateRotationX(rotation) *
			//	//Matrix.CreateRotationY(rotation) *
			//	Matrix.CreateRotationZ(rotation) *
			//	Matrix.CreateScale(scale)
			//	//Matrix.CreateTranslation(new Vector3(0, 0, 0))
			//	;

			// Kris Steele:
			Vector2 DrawPosition = new Vector2(sizeResGame.X / 3, sizeResGame.Y / 3);
			float Rotation = 0.0f;
			float Zoom = (float)(2.0 + Math.Sin(timeTotal * 1.0));
			//float ViewportWidth = GraphicsDevice.Viewport.Width;
			//float ViewportHeight = GraphicsDevice.Viewport.Height;
			float ViewportWidth = sizeResGame.X;
			float ViewportHeight = sizeResGame.Y;
			Matrix transformMatrix =
				Matrix.CreateTranslation(new Vector3((int)-DrawPosition.X, (int)-DrawPosition.Y, 0)) *
				Matrix.CreateRotationZ(Rotation) *
				Matrix.CreateScale(Zoom) *
				Matrix.CreateTranslation(new Vector3(ViewportWidth * 0.5f, ViewportHeight * 0.5f, 0));

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
					Color color = new Color(
						(byte)(128 + x * 82 + y * 104),
						(byte)(192 + x * 135 + y * 72),
						(byte)(64 + x * 93 + y * 59));
					spriteBatch.Draw(textSprite, pos, color);
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
