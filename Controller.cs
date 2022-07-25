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
		private readonly double autoZoomSpeed = 0.6532014;
		private readonly double autoRotateSpeed = 0.3274659;

		private List<SamplerState> samplerStates = new List<SamplerState>() {
				SamplerState.AnisotropicClamp,
				SamplerState.AnisotropicWrap,
				SamplerState.LinearClamp,
				SamplerState.LinearWrap,
				SamplerState.PointClamp,
				SamplerState.PointWrap
			};


		// ---- properties

		public Vector2 GetPos { get { return pos; } }
		public float GetZoom { get { return zoom; } }
		public float GetRotate { get { return rotate; } }
		public bool GetUseSpriteSheet { get { return bUseSpriteSheet; } }
		public SamplerState GetSamplerState { get { return samplerState; } }
		public Matrix GetTransformMatrix { 
			get {
				Point sizeResGame = game.GetRender.GetSizeResGame;
				// exact copy of Kris Steele's transform matrix for PK, with changes to screen size & draw position:
				Vector2 origin = new Vector2(sizeResGame.X * 0.5f, sizeResGame.Y * 0.5f) + pos;  // for rotation and zoom
				Matrix transformMatrix =
					Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *
					Matrix.CreateRotationZ(rotate) *
					Matrix.CreateScale(zoom) *
					Matrix.CreateTranslation(new Vector3(origin, 0.0f)) *
					Matrix.CreateTranslation(new Vector3(-pos, 0.0f));
				return transformMatrix;
			}
		}


		// ---- data members

		private readonly Game1 game;

		// input
		private GamePadState gamePadStateCurr;
		private GamePadState gamePadStatePrev;
		private KeyboardState keyboardStateCurr;
		private KeyboardState keyboardStatePrev;

		// location
		private Vector2 pos = new Vector2(0, 0);
		private float zoom = 1.0f;
		private float rotate = 0.0f;

		// controls
		private SamplerState samplerState = SamplerState.PointClamp;
		private bool bUseSpriteSheet = true;
		private bool bAllowZoomControl = false;
		private bool bAllowRotateControl = true;



		// ---- methods

		public Controller(Game1 game)
		{
			this.game = game;
		}

		public void Update(GameTime gameTime)
		{
			// frame rate adjust
			double timeFrame = gameTime.ElapsedGameTime.TotalSeconds;
			double timeTotal = gameTime.TotalGameTime.TotalSeconds;

			// get input
			gamePadStatePrev = gamePadStateCurr;
			gamePadStateCurr = GamePad.GetState(PlayerIndex.One);
			keyboardStatePrev = keyboardStateCurr;
			keyboardStateCurr = Keyboard.GetState();

			// get data from gamepad and keyboard
			bool exit = 
				gamePadStateCurr.Buttons.Back == ButtonState.Pressed || 
				keyboardStateCurr.IsKeyDown(Keys.Escape);
			bool buttonA_samplerState = gamePadStatePrev.Buttons.A == ButtonState.Released && gamePadStateCurr.Buttons.A == ButtonState.Pressed;
			bool buttonB_spriteSheet = gamePadStatePrev.Buttons.B == ButtonState.Released && gamePadStateCurr.Buttons.B == ButtonState.Pressed;
			bool buttonX_autoZoom = gamePadStatePrev.Buttons.X == ButtonState.Released && gamePadStateCurr.Buttons.X == ButtonState.Pressed;
			bool buttonY_autoRotate = gamePadStatePrev.Buttons.Y == ButtonState.Released && gamePadStateCurr.Buttons.Y == ButtonState.Pressed;
			buttonA_samplerState |= keyboardStatePrev.IsKeyUp(Keys.A) && keyboardStateCurr.IsKeyDown(Keys.A);
			buttonB_spriteSheet |= keyboardStatePrev.IsKeyUp(Keys.B) && keyboardStateCurr.IsKeyDown(Keys.B);
			buttonX_autoZoom |= keyboardStatePrev.IsKeyUp(Keys.X) && keyboardStateCurr.IsKeyDown(Keys.X);
			buttonX_autoZoom |= keyboardStatePrev.IsKeyUp(Keys.Z) && keyboardStateCurr.IsKeyDown(Keys.Z);
			buttonY_autoRotate |= keyboardStatePrev.IsKeyUp(Keys.Y) && keyboardStateCurr.IsKeyDown(Keys.Y);
			buttonY_autoRotate |= keyboardStatePrev.IsKeyUp(Keys.R) && keyboardStateCurr.IsKeyDown(Keys.R);
			bool zoomIn = keyboardStateCurr.IsKeyDown(Keys.OemPlus) || keyboardStateCurr.IsKeyDown(Keys.Add);
			bool zoomOut = keyboardStateCurr.IsKeyDown(Keys.OemMinus) || keyboardStateCurr.IsKeyDown(Keys.Subtract);
			bool rotateLeft = keyboardStateCurr.IsKeyDown(Keys.OemOpenBrackets) || keyboardStateCurr.IsKeyDown(Keys.OemComma);
			bool rotateRight = keyboardStateCurr.IsKeyDown(Keys.OemCloseBrackets) || keyboardStateCurr.IsKeyDown(Keys.OemPeriod);

			// move
			Vector2 move = new Vector2();
			// inverse Y for gamepads
			move += gamePadStateCurr.ThumbSticks.Left;
			move += gamePadStateCurr.ThumbSticks.Right;
			move.Y = -move.Y;
			move.X += gamePadStateCurr.DPad.Left == ButtonState.Pressed ? -1 : 0;
			move.X += gamePadStateCurr.DPad.Right == ButtonState.Pressed ? +1 : 0;
			move.Y += gamePadStateCurr.DPad.Up == ButtonState.Pressed ? -1 : 0;
			move.Y += gamePadStateCurr.DPad.Down == ButtonState.Pressed ? +1 : 0;
			move.X += keyboardStateCurr.IsKeyDown(Keys.Left) ? -1 : 0;
			move.X += keyboardStateCurr.IsKeyDown(Keys.Right) ? +1 : 0;
			move.Y += keyboardStateCurr.IsKeyDown(Keys.Up) ? -1 : 0;
			move.Y += keyboardStateCurr.IsKeyDown(Keys.Down) ? +1 : 0;
			pos += move / zoom * (float)timeFrame * 200.0f;

			// zoom
			float zoomMultiplyFactor = 1.0f;
			float zoomInput = gamePadStateCurr.Triggers.Right - gamePadStateCurr.Triggers.Left
				+ (zoomIn ? +1 : 0) + (zoomOut ? -1 : 0);
			zoomMultiplyFactor += zoomInput * 2.0f * (float)timeFrame;

			// rotate
			float rotateAdditionFactor = 0.0f;
			rotateAdditionFactor += ((rotateLeft ? -1 : 0) + (rotateRight ? +1 : 0)) * (float)timeFrame;

			// exit?
			if (exit) game.Exit();

			if (buttonA_samplerState) ChangeSamplerState();
			if (buttonB_spriteSheet) ChangeSpriteSheet();
			if (buttonX_autoZoom) ChangeZoom();
			if (buttonY_autoRotate) ChangeRotate();

			// auto rotate & zoom
			if (bAllowZoomControl)
				zoom *= zoomMultiplyFactor;
			else
				zoom = (float)(2.0 + Math.Sin(timeTotal * autoZoomSpeed));

			if (bAllowRotateControl)
				rotate += rotateAdditionFactor;
			else
				rotate = (float)(timeTotal * autoRotateSpeed);
		}

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
			bAllowZoomControl = !bAllowZoomControl;
		}

		private void ChangeRotate()
		{
			bAllowRotateControl = !bAllowRotateControl;
		}

	}  // public class Controller
}  // namespace SpriteBatchDemo

