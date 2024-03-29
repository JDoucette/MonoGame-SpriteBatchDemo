﻿// Jason Allen Doucette
// July 18, 2022

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpriteBatchDemo
{
	public class Render
	{
		// ---- constants

		// resolution
		public const int scaleGame = 4;

		// hud
		private readonly StringBuilder strTitle = new StringBuilder("SpriteBatch Demo");
		private readonly StringBuilder str = new StringBuilder(256);


		// ---- properties

		public Point GetSizeResGame { get { return sizeResGame; } }


		// ---- data members

		// game
		private readonly Game1 game;

		// graphics
		private GraphicsDevice graphicsDevice;
		private readonly SpriteBatch spriteBatch;

		// game screen
		private readonly RenderTarget2D textLowResGame;

		// resolution
		private Point sizeResGame;
		private Point sizeResScreen;

		// hud
		private Texture2D textWhite;
		private Font fontMain;
		private Font fontSmall;


		// ---- methods

		public Render(Game1 game, GraphicsDevice graphicsDevice)
		{
			this.game = game;
			this.graphicsDevice = graphicsDevice;
			this.spriteBatch = new SpriteBatch(graphicsDevice);

			this.sizeResScreen = new Point(
				graphicsDevice.Viewport.Width,  // window size, if not full screen
				graphicsDevice.Viewport.Height);
			//this.sizeResScreen = new Point(
			//	GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,  // OS screen size
			//	GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);

			this.sizeResGame = new Point(
				sizeResScreen.X / scaleGame,
				sizeResScreen.Y / scaleGame);
			textLowResGame = new RenderTarget2D(graphicsDevice, 
				sizeResGame.X, 
				sizeResGame.Y, 
				mipMap: false, SurfaceFormat.Color, DepthFormat.None);
		}

		internal void LoadContent(ContentManager content)
		{
			textWhite = game.GetArt.CreateWhiteTexture(8);

			// lowercase
			//fontSmall = new Font(content.Load<Texture2D>(@"Fonts\font-arcade-classic-7x7-jason-edit"));
			//fontSmall = new Font(content.Load<Texture2D>(@"Fonts\font-arcade-classic-7x7-bold-smb2-jason-design"));

			// uppercase only
			fontSmall = new Font(content.Load<Texture2D>(@"Fonts\font-jason-5x6-fixed"));
			fontMain = new Font(content.Load<Texture2D>(@"Fonts\font-jason-7x8-fixed-double-bold"));
		}

		public void Render_LowResGameScreen(GameTime gameTime)
		{
			// get controller info
			SamplerState samplerState = game.GetController.GetSamplerState;
			bool bSpriteSheet = game.GetController.GetUseSpriteSheet;

			Tiles.Tile[] tilesToRender = bSpriteSheet ? 
				game.GetTiles.GetTilesSpriteSheet : 
				game.GetTiles.GetIndividualSprites;

			graphicsDevice.SetRenderTarget((RenderTarget2D)textLowResGame);
			graphicsDevice.Clear(new Color(0, 0, 0));

			Render_LowResGameScreen_Foreground(gameTime, tilesToRender);
			Render_LowResGameScreen_Hud();
		}

		private void Render_LowResGameScreen_Foreground(GameTime gameTime, Tiles.Tile [] tilesToRender)
		{
			// get controller info
			SamplerState samplerState = game.GetController.GetSamplerState;
			Matrix transformMatrix = game.GetController.GetTransformMatrix;

			spriteBatch.Begin(
				SpriteSortMode.Deferred,
				BlendState.AlphaBlend,
				samplerState,
				DepthStencilState.None,
				RasterizerState.CullNone,
				effect: null,
				transformMatrix);
			{
				// render all tiles
				foreach (Tiles.Tile tile in tilesToRender)
					spriteBatch.Draw(tile.texture, tile.position, tile.rectSource, Color.White);
			}
			spriteBatch.End();
		}

		private void Render_LowResGameScreen_Hud()
		{
			// get controller info
			SamplerState samplerState = game.GetController.GetSamplerState;
			bool bSpriteSheet = game.GetController.GetUseSpriteSheet;
			float zoom = game.GetController.GetZoom;
			float rotate = game.GetController.GetRotate;

			spriteBatch.Begin();
			{
				Vector2 pos = new Vector2(1, 1);
				fontMain.Draw(spriteBatch, strTitle, pos, Color.CornflowerBlue);
				pos.Y += fontMain.FontHeight;

				str.Clear().AppendFormat("[A] {0}", samplerState);
				fontSmall.Draw(spriteBatch, str, pos, Color.LightSkyBlue);
				pos.Y += fontSmall.FontHeight;

				str.Clear().AppendFormat("[B] SpriteSheet: {0}", bSpriteSheet);
				fontSmall.Draw(spriteBatch, str, pos, Color.LightSkyBlue);
				pos.Y += fontSmall.FontHeight;

				str.Clear().AppendFormat("[X/Z] Zoom:{0,6:F3}", zoom);
				fontSmall.Draw(spriteBatch, str, pos, Color.LightSkyBlue);
				pos.Y += fontSmall.FontHeight;

				str.Clear().AppendFormat("[Y/R] Rotate:{0,6:F3}", rotate);
				fontSmall.Draw(spriteBatch, str, pos, Color.LightSkyBlue);
				pos.Y += fontSmall.FontHeight;

				str.Clear().Append("[Space] Reset");
				fontSmall.Draw(spriteBatch, str, pos, Color.LightSkyBlue);
				pos.Y += fontSmall.FontHeight;
			}
			spriteBatch.End();
		}

		public void Render_LowResGameScreen_to_BackBuffer()
		{
			graphicsDevice.SetRenderTarget(null);
			graphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin(
				SpriteSortMode.Deferred,
				BlendState.AlphaBlend,
				SamplerState.PointClamp,  // do not use game.GetController.GetSamplerState -- that's for the game screen render
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

		public void Render_Hud_SpriteSheet(Texture2D textSpriteSheet)
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

		public void Render_Hud_IndividualSprites(Point sizeTile_pixels)
		{
			spriteBatch.Begin();
			{
				// lower-right

				Vector2 pos = new Vector2(sizeResScreen.X - sizeTile_pixels.X, sizeResScreen.Y - sizeTile_pixels.Y);
				foreach (Texture2D texture in game.GetTiles.GetTextSpritesIndividual)
				{
					spriteBatch.Draw(texture, pos, Color.White);
					pos.X -= sizeTile_pixels.X;
				}
			}
			spriteBatch.End();
		}

	}
}
