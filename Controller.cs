// Jason Allen Doucette
// July 19, 2022

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpriteBatchDemo
{
	public class Controller
	{
		// ---- constants

		// speed
		private readonly double zoomSpeed = 0.4534828;
		private readonly double rotateSpeed = 0.0;  // 0.2746593;


		// ---- properties

		public Vector2 GetPos { get { return pos; } }
		public float GetZoom { get { return zoom; } }
		public float GetRotate { get { return rotate; } }
		public bool GetUseSpriteSheet { get { return bUseSpriteSheet; } }
		public SamplerState GetSamplerState { get { return samplerState; } }


		// ---- data members

		private Game1 game;

		// input
		private GamePadState gamePadStateCurr;
		private GamePadState gamePadStatePrev;
		private KeyboardState keyboardStateCurr;
		private KeyboardState keyboardStatePrev;

		// controls
		private Vector2 pos = new Vector2(0, 0);
		private float zoom = 1.0f;
		private float rotate = 0.0f;
		private bool bUseSpriteSheet = true;
		private SamplerState samplerState = SamplerState.PointClamp;


		// ---- methods

		public Controller(Game1 game)
		{
			this.game = game;
		}

		public void Update(GameTime gameTime)
		{
			// get input
			gamePadStatePrev = gamePadStateCurr;
			gamePadStateCurr = GamePad.GetState(PlayerIndex.One);
			keyboardStatePrev = keyboardStateCurr;
			keyboardStateCurr = Keyboard.GetState();

			// process input devices
			bool exit = 
				gamePadStateCurr.Buttons.Back == ButtonState.Pressed || 
				keyboardStateCurr.IsKeyDown(Keys.Escape);
			bool buttonA = gamePadStatePrev.Buttons.A == ButtonState.Released && gamePadStateCurr.Buttons.A == ButtonState.Pressed;
			bool buttonB = gamePadStatePrev.Buttons.B == ButtonState.Released && gamePadStateCurr.Buttons.B == ButtonState.Pressed;
			bool buttonX = gamePadStatePrev.Buttons.X == ButtonState.Released && gamePadStateCurr.Buttons.X == ButtonState.Pressed;
			bool buttonY = gamePadStatePrev.Buttons.Y == ButtonState.Released && gamePadStateCurr.Buttons.Y == ButtonState.Pressed;
			buttonA |= keyboardStatePrev.IsKeyUp(Keys.A) && keyboardStateCurr.IsKeyDown(Keys.A);
			buttonB |= keyboardStatePrev.IsKeyUp(Keys.B) && keyboardStateCurr.IsKeyDown(Keys.B);
			buttonX |= keyboardStatePrev.IsKeyUp(Keys.X) && keyboardStateCurr.IsKeyDown(Keys.X);
			buttonY |= keyboardStatePrev.IsKeyUp(Keys.Y) && keyboardStateCurr.IsKeyDown(Keys.Y);

			// exit?
			if (exit) game.Exit();

			if (buttonA) ChangeSamplerState();
			if (buttonB) ChangeSpriteSheet();
			if (buttonX) ChangeZoom();
			if (buttonY) ChangeRotate();

			// auto rotate & zoom
			double timeTotal = gameTime.TotalGameTime.TotalSeconds;
			rotate = (float)(timeTotal * rotateSpeed);
			zoom = (float)(2.5 + 1.5 * Math.Sin(timeTotal * zoomSpeed));
		}

		private List<SamplerState> samplerStates = new List<SamplerState>() {
				SamplerState.AnisotropicClamp,
				SamplerState.AnisotropicWrap,
				SamplerState.LinearClamp,
				SamplerState.LinearWrap,
				SamplerState.PointClamp,
				SamplerState.PointWrap
			};

		private void ChangeSamplerState()
		{
			int count = samplerStates.Count;
			int index = 0;
			while (samplerStates[index] != samplerState) index++;
			index++;  // next
			if (index >= count) index = 0;
			samplerState = samplerStates[index];
		}

		private void ChangeSpriteSheet()
		{
			bUseSpriteSheet = !bUseSpriteSheet;
		}

		private void ChangeZoom()
		{
			throw new NotImplementedException();
		}

		private void ChangeRotate()
		{
			throw new NotImplementedException();
		}

	}  // public class Controller
}  // namespace SpriteBatchDemo

