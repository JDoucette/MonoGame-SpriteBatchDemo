// Jason Allen Doucette
// July 15, 2022
// SpriteBatch Demo
//
// Testing Draw() with scaling with PointClamp / PointWrap
// also with transforms, and to see if it works or ruins neares neighbour.
// And also using sprite sheets, to see if it changes things.
//
// TODO:
//	1.	Allow keyboard control:  sample state, and spritesheet vs individual.  Show results in HUD.
//	2.	Allow keyboard control:  zoom, rotate, and recenter

#define SETTING_POINTCLAMP  // for SamplerState: if set, then PointClamp, else PointWrap
#define USE_SPRITESHEET  // else, draw individual textures

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

		// speed
		private double zoomSpeed = 0.453482658;
		private double rotateSpeed = 0.0;  // 0.2746593

		// sampler state
#if SETTING_POINTCLAMP
		private static readonly SamplerState samplerState = SamplerState.PointClamp;
#else
		private static readonly SamplerState samplerState = SamplerState.PointWrap;
#endif

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

		// hud
		private readonly StringBuilder strTitle = new StringBuilder("SpriteBatch Demo");
		private readonly StringBuilder str = new StringBuilder(256);

		// sprites
		private Point sizeTile_pixels = new Point(16, 16);  // size of each tile, in pixels
		private Point sizeSpriteSheet_tiles = new Point(8, 8);  // size of the sprite sheet (a matrix of equal sized tiles), in tiles

		// matrix transform
		private float rotate;
		private float zoom;


		// ---- struct

		private struct Tile
		{
			public Texture2D texture;
			public Rectangle? rectSource;  // if null, then use the entire texture
			public Vector2 position;
		}


		// ---- data members

		// graphics
		private readonly GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;

		// texture
		private Art art;
		private RenderTarget2D textLowResGame;
		private Texture2D textSpriteSheet;
		private List<Texture2D> textSpritesIndividual;

		// hud
		private Texture2D textWhite;
		private Font font;

		// tiles
		private Tile[] tilesSpriteSheet;  // 1D array is fine, since we store the position within each tile
		private Tile[] tilesIndividualSprites;  // 1D array is fine, since we store the position within each tile


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

		private void Initialize_after_LoadedContent()
		{
			GenerateTiles(ref tilesSpriteSheet, ref tilesIndividualSprites);
		}

		private void Initialize_before_LoadedContent()
		{
			art = new Art(GraphicsDevice);
			textLowResGame = new RenderTarget2D(GraphicsDevice, sizeResGame.X, sizeResGame.Y, mipMap: false, SurfaceFormat.Color, DepthFormat.None);
		}

		private void GenerateTiles(ref Tile[] tilesSpriteSheet, ref Tile[] tilesIndividualSprites)
		{
			// determine number of tiles that can fit on the screen
			// (yes, we're rounding down, but no biggie)
			Point sizeSpriteArray = new Point(
				sizeResGame.X / sizeTile_pixels.X,
				sizeResGame.Y / sizeTile_pixels.Y);
			int numTiles = sizeSpriteArray.X * sizeSpriteArray.Y;
			tilesSpriteSheet = new Tile[numTiles];
			tilesIndividualSprites = new Tile[numTiles];

			// generate the tiles
			int index = 0;
			for (int y = 0; y < sizeSpriteArray.Y; y++)
				for (int x = 0; x < sizeSpriteArray.X; x++)
				{
					// pick random tile from the sprite sheet
					Point posSpriteSheet = new Point(
						MathUtil.rng.Next(sizeSpriteSheet_tiles.X),
						MathUtil.rng.Next(sizeSpriteSheet_tiles.Y));

					// sprite within spritesheet
					Rectangle rectSource = new Rectangle(
						posSpriteSheet.X * sizeTile_pixels.X,
						posSpriteSheet.Y * sizeTile_pixels.Y,
						sizeTile_pixels.X,
						sizeTile_pixels.Y);

					Vector2 posScreen = new Vector2(
						x * sizeTile_pixels.X,
						y * sizeTile_pixels.Y);

					// 1. sprite sheet
					Tile tile = new Tile
					{
						position = posScreen,
						rectSource = rectSource,  // use a portion of the texture
						texture = textSpriteSheet
					};
					Debug.Assert(tile.texture != null);  // must call this after LoadContent()
					tilesSpriteSheet[index] = tile;

					// 2. individual
					tile = new Tile
					{
						position = posScreen,
						rectSource = null,  // use the whole texture
						texture = textSpritesIndividual[MathUtil.rng.Next(textSpritesIndividual.Count)]  // pick one random texture
					};
					Debug.Assert(tile.texture != null);  // must call this after LoadContent()
					tilesIndividualSprites[index] = tile;

					index++;
				}
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);

			textSpriteSheet = art.CreateSpriteSheetTexture(sizeTile_pixels, sizeSpriteSheet_tiles);
			textSpritesIndividual = art.CreateIndividualSprites(numSprites: 16, sizeTile_pixels);
			textWhite = art.CreateWhiteTexture(8);

			// lowercase
			//font = new Font(Content.Load<Texture2D>(@"Fonts\font-arcade-classic-7x7-jason-edit"));
			//font = new Font(Content.Load<Texture2D>(@"Fonts\font-arcade-classic-7x7-bold-smb2-jason-design"));

			// uppercase only
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
			Render_Hud_SpriteSheet();
			Render_Hud_IndividualSprites();

			base.Draw(gameTime);
		}

		private void Render_LowResGameScreen(GameTime gameTime)
		{
			GraphicsDevice.SetRenderTarget((RenderTarget2D)textLowResGame);
			GraphicsDevice.Clear(new Color(0, 0, 0));

			Render_LowResGameScreen_Foreground(gameTime);
			Render_LowResGameScreen_Hud();
		}

		private void Render_LowResGameScreen_Foreground(GameTime gameTime)
		{
			double timeTotal = gameTime.TotalGameTime.TotalSeconds;

			// exact copy of Kris Steele's transform matrix for PK, with changes to screen size & draw position:
			rotate = (float)(timeTotal * rotateSpeed);
			zoom = (float)(2.5 + 1.5 * Math.Sin(timeTotal * zoomSpeed));
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
				// render all tiles
#if USE_SPRITESHEET
				foreach (Tile tile in tilesSpriteSheet)
					spriteBatch.Draw(tile.texture, tile.position, tile.rectSource, Color.White);
#else
				foreach (Tile tile in tilesIndividualSprites)
					spriteBatch.Draw(tile.texture, tile.position, tile.rectSource, Color.White);
#endif
			}
			spriteBatch.End();
		}

		private void Render_LowResGameScreen_Hud()
		{
			spriteBatch.Begin();
			{
				Vector2 pos = new Vector2(1, 1);
				font.Draw(spriteBatch, strTitle, pos, Color.PaleVioletRed);
				pos.Y += font.FontHeight;

				str.Clear().AppendFormat("  Zoom:{0,6:F3}", zoom);
				font.Draw(spriteBatch, str, pos, Color.White);
				pos.Y += font.FontHeight;

				str.Clear().AppendFormat("Rotate:{0,6:F3}", rotate);
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

		private void Render_Hud_SpriteSheet()
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

		private void Render_Hud_IndividualSprites()
		{
			spriteBatch.Begin();
			{
				// lower-right

				Vector2 pos = new Vector2(sizeResScreen.X - sizeTile_pixels.X, sizeResScreen.Y - sizeTile_pixels.Y);
				foreach (Texture2D texture in textSpritesIndividual)
				{
					spriteBatch.Draw(texture, pos, Color.White);
					pos.X -= sizeTile_pixels.X;
				}
			}
			spriteBatch.End();
		}

	}  // 	public class Game1 : Game
}  // namespace SpriteBatchDemo
