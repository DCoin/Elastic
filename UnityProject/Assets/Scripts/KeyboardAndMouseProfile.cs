using System;
using System.Collections;
using UnityEngine;
using InControl;


namespace NameSpace //TODO what name???
{
	// This custom profile is enabled by adding it to the Custom Profiles list
	// on the InControlManager component, or you can attach it yourself like so:
	// InputManager.AttachDevice( new UnityInputDevice( "KeyboardAndMouseProfile" ) );
	// 
	public class KeyboardAndMouseProfile : UnityInputDeviceProfile
	{
		public KeyboardAndMouseProfile()
		{
			Name = "Keyboard/Mouse";
			Meta = "A keyboard and mouse combination profile appropriate for simulating a controller.";

			// This profile only works on desktops.
			SupportedPlatforms = new[]
			{
				"Windows",
				"Mac",
				"Linux"
			};

			Sensitivity = 1.0f;
			LowerDeadZone = 0.0f;
			UpperDeadZone = 1.0f;

			ButtonMappings = new[]
			{
				new InputControlMapping
				{
					Handle = "Fire - Mouse",
					Target = InputControlType.Action1,
					Source = MouseButton0
				},
				new InputControlMapping
				{
					Handle = "Fire - Keyboard",
					Target = InputControlType.Action1,
					// KeyCodeButton fires when any of the provided KeyCode params are down.
					Source = KeyCodeButton( KeyCode.F, KeyCode.Return )
				},
				new InputControlMapping
				{
					Handle = "AltFire",
					Target = InputControlType.Action2,
					Source = MouseButton2
				},
				new InputControlMapping
				{
					Handle = "Middle",
					Target = InputControlType.Action3,
					Source = MouseButton1
				},
				new InputControlMapping
				{
					Handle = "Jump",
					Target = InputControlType.Action4,
					Source = KeyCodeButton( KeyCode.Space )
				},
				new InputControlMapping
				{
					Handle = "Combo",
					Target = InputControlType.LeftBumper,
					// KeyCodeComboButton requires that all KeyCode params are down simultaneously.
					Source = KeyCodeComboButton( KeyCode.LeftAlt, KeyCode.Alpha1 )
				},
			};

			AnalogMappings = new[]
			{
				new InputControlMapping
				{
					Handle = "Move X Left",
					Target = InputControlType.LeftStickX,
					// KeyCodeAxis splits the two KeyCodes over an axis. The first is negative, the second positive.
					Source = KeyCodeAxis( KeyCode.A, KeyCode.D )
				},
				new InputControlMapping
				{
					Handle = "Move Y Left",
					Target = InputControlType.LeftStickY,
					// Notes that up is positive in Unity, therefore the order of KeyCodes is down, up.
					Source = KeyCodeAxis( KeyCode.S, KeyCode.W )
				},
				new InputControlMapping {
					Handle = "Move X Right",
					Target = InputControlType.RightStickX,
					Source = KeyCodeAxis( KeyCode.LeftArrow, KeyCode.RightArrow )
				},
				new InputControlMapping {
					Handle = "Move Y Right",
					Target = InputControlType.RightStickY,
					Source = KeyCodeAxis( KeyCode.DownArrow, KeyCode.UpArrow )
				}
			};
		}
	}
}

