// Jason Allen Doucette
// July 18, 2022

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpriteBatchDemo
{
	public class Tiles
	{
		// ---- constants

		// sprites
		private Point sizeTile_pixels = new Point(16, 16);  // size of each tile, in pixels
		private Point sizeSpriteSheet_tiles = new Point(8, 8);  // size of the sprite sheet (a matrix of equal sized tiles), in tiles


		// ---- struct

		public struct Tile
		{
			public Texture2D texture;
			public Rectangle? rectSource;  // if null, then use the entire texture
			public Vector2 position;
		}


		// ---- properties

		public Tile[] GetTilesSpriteSheet { get { return tilesSpriteSheet; } }
		public Tile[] GetIndividualSprites { get { return tilesIndividualSprites; } }
		public Texture2D GetTextSpriteSheet { get { return textSpriteSheet; } }
		public List<Texture2D> GetTextSpritesIndividual { get { return textSpritesIndividual; } }
		public Point GetSizeTilePixels { get { return sizeTile_pixels; } }


		// ---- data members

		// game
		Game1 game;

		// texture
		private Texture2D textSpriteSheet;
		private List<Texture2D> textSpritesIndividual;

		// tiles
		private Tile[] tilesSpriteSheet;  // 1D array is fine, since we store the position within each tile
		private Tile[] tilesIndividualSprites;  // 1D array is fine, since we store the position within each tile


		// ---- methods

		public Tiles (Game1 game)
		{
			this.game = game;
		}

		internal void CreateSprites()
		{
			textSpriteSheet = game.GetArt.CreateSpriteSheetTexture(sizeTile_pixels, sizeSpriteSheet_tiles);
			textSpritesIndividual = game.GetArt.CreateIndividualSprites(numSprites: 16, sizeTile_pixels);
		}

		internal void GenerateTiles()
		{
			GenerateTiles(ref tilesSpriteSheet, ref tilesIndividualSprites);
		}

		private void GenerateTiles(ref Tile[] tilesSpriteSheet, ref Tile[] tilesIndividualSprites)
		{
			// determine number of tiles that can fit on the screen
			// (yes, we're rounding down, but no biggie)
			Point sizeResGame = game.GetRender.GetSizeResGame;
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

	}  // public class Tiles
}  // namespace SpriteBatchDemo

