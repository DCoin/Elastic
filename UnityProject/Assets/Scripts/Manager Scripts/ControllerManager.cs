using System;
using UnityEngine;
using InControl;
using System.Collections.Generic;


public class ControllerManager {
	private const float ANALOG_THRESHOLD = 0.5f;
	private static HashSet<int> missingControllers = new HashSet<int>();

	/// <summary>
	/// Gets the jump input bool.
	/// </summary>
	/// <returns><c>true</c>, if analogue stick is moved beyond threshold, <c>false</c> otherwise.</returns>
	/// <param name="controller">Controller index.</param>
	/// <param name="leftSide">Set to <c>true</c> if left side of controller.</param>
	public static bool GetJumpInputBool(int controller, bool leftSide) {
		return (GetJumpInput(controller, leftSide) > ANALOG_THRESHOLD) ? true : false;
	}

	/// <summary>
	/// Gets the jump input as float.
	/// </summary>
	/// <returns>The jump input.</returns>
	/// <param name="controller">Controller index.</param>
	/// <param name="leftSide">Set to <c>true</c> if left side of controller.</param>
	private static float GetJumpInput(int controller, bool leftSide) {
		if (!checkController(controller)) return 0;
		if (leftSide)
			return InputManager.Devices[controller].LeftStickY.Value; //TODO Returns -1 when heavy
		else 
			return InputManager.Devices[controller].RightStickY.Value;  //TODO Returns -1 when heavy
	}

	/// <summary>
	/// Gets the heavy input bool.
	/// </summary>
	/// <returns><c>true</c>, if analogue stick is moved beyond threshold, <c>false</c> otherwise.</returns>
	/// <param name="controller">Controller index.</param>
	/// <param name="leftSide">Set to <c>true</c> if left side of controller.</param>
	public static bool GetHeavyInputBool(int controller, bool leftSide){
		return (GetHeavyInput(controller, leftSide) > ANALOG_THRESHOLD) ? true : false;
	}

	/// <summary>
	/// Gets the heavy input as float.
	/// </summary>
	/// <returns>The heavy input.</returns>
	/// <param name="controller">Controller index.</param>
	/// <param name="leftSide">Set to <c>true</c> if left side of controller.</param>
	private static float GetHeavyInput(int controller, bool leftSide) {
		if (!checkController(controller)) return 0;
		if (leftSide)
			return -InputManager.Devices[controller].LeftStickY.Value;  //TODO Returns -1 when jumping
		else 
			return -InputManager.Devices[controller].RightStickY.Value;  //TODO Returns -1 when jumping
	}

	/// <summary>
	/// Gets if stick is held to the left.
	/// </summary>
	/// <returns><c>true</c>, if analogue stick is moved beyond threshold, <c>false</c> otherwise.</returns>
	/// <param name="controller">Controller index.</param>
	/// <param name="leftSide">Set to <c>true</c> if left side of controller.</param>
	public static bool GetLeftInputBool(int controller, bool leftSide) {
		return (GetHorizontalInput(controller, leftSide) < -ANALOG_THRESHOLD) ? true : false;
	}

	/// <summary>
	/// Gets if stick is held to the right.
	/// </summary>
	/// <returns><c>true</c>, if analogue stick is moved beyond threshold, <c>false</c> otherwise.</returns>
	/// <param name="controller">Controller index.</param>
	/// <param name="leftSide">Set to <c>true</c> if left side of controller.</param>
	public static bool GetRightInputBool(int controller, bool leftSide) {
		return (GetHorizontalInput(controller, leftSide) > ANALOG_THRESHOLD) ? true : false;
	}

	/// <summary>
	/// Gets the horizontal value of the control stick
	/// </summary>
	/// <returns>The x axis value.</returns>
	/// <param name="controller">Controller index.</param>
	/// <param name="leftSide">Set to <c>true</c> if left side of controller.</param>
	public static float GetHorizontalInput(int controller, bool leftSide) {
		if (!checkController(controller)) return 0;
		if (leftSide)
			return InputManager.Devices[controller].LeftStickX.Value;
		else 
			return InputManager.Devices[controller].RightStickX.Value;
	}

	/// <summary>
	/// Gets the vertical value of the control stick
	/// </summary>
	/// <returns>The y axis value.</returns>
	/// <param name="controller">Controller index.</param>
	/// <param name="leftSide">Set to <c>true</c> if left side of controller.</param>
	public static float GetVerticalInput(int controller, bool leftSide) {
		if (!checkController(controller)) return 0;
		if (leftSide)
			return InputManager.Devices[controller].LeftStickY.Value;
		else 
			return InputManager.Devices[controller].RightStickY.Value;
	}
	/// <summary>
	/// Gets the stick click input as bool.
	/// </summary>
	/// <returns>The stick button input.</returns>
	/// <param name="controller">Controller index.</param>
	/// <param name="leftSide">Set to <c>true</c> if left side of controller.</param>
	public static bool GetStickButtonInput(int controller, bool leftSide) {
		if (!checkController(controller)) return false;
		if (leftSide)
			return (InputManager.Devices[controller].LeftStickButton.Value > 0.001f) ? true : false;
		else 
			return (InputManager.Devices[controller].RightStickButton.Value> 0.001f) ? true : false;
	}

	/// <summary>
	/// Gets the A button input as bool.
	/// </summary>
	/// <returns>The A button input.</returns>
	/// <param name="controller">Controller index.</param>
	public static bool GetAButtonInput(int controller) {
		if (!checkController(controller)) return false;
		return (InputManager.Devices[controller].Action1.Value > 0.001f) ? true : false;
	}
	
	/// <summary>
	/// Gets the B button input as bool.
	/// </summary>
	/// <returns>The B button input.</returns>
	/// <param name="controller">Controller index.</param>
	public static bool GetBButtonInput(int controller) {
		if (!checkController(controller)) return false;
		return (InputManager.Devices[controller].Action2.Value > 0.001f) ? true : false;
	}

	/// <summary>
	/// Gets if any controller paused.
	/// </summary>
	/// <returns>The pause input.</returns>
	public static bool GetPauseButtonInput() {
		return InputManager.MenuWasPressed;
	}

	/// <summary>
	/// Gets the pause input as bool.
	/// </summary>
	/// <returns>The pause input.</returns>
	/// <param name="controller">Controller index.</param>
	public static bool GetPauseButtonInput(int controller) {
		if (!checkController(controller)) return false;
		return InputManager.Devices[controller].MenuWasPressed;
	}

	// TODO add method comment
	private static bool checkController(int controller) {
		if (InputManager.Devices.Count > controller) return true;
		if (missingControllers.Add(controller)) Debug.Log ("Controller " + controller + " not attached");
		return false;
	}
}

