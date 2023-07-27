using UnityEngine;
using Rewired;

public class ControllerDetectionHelper : MonoBehaviour
{


	void Start()
	{
		// Subscribe to events
		ReInput.ControllerConnectedEvent += OnControllerConnected;

		// Assign each Joystick to a Player initially
		foreach (Joystick j in ReInput.controllers.Joysticks)
		{
			if (ReInput.controllers.IsJoystickAssigned(j)) continue; // Joystick is already assigned

			// Assign Joystick to first Player that doesn't have any assigned
			AssignJoystickToNextOpenPlayer(j);
		}
	}

// This will be called when a controller is connected
	void OnControllerConnected(ControllerStatusChangedEventArgs args)
	{
		if (args.controllerType != ControllerType.Joystick) return; // skip if this isn't a Joystick

		Debug.Log("Attempting to assign joystick to Player.");
		// Assign Joystick to first Player that doesn't have any assigned
		AssignJoystickToNextOpenPlayer(ReInput.controllers.GetJoystick(args.controllerId));
	}


	void AssignJoystickToNextOpenPlayer(Joystick j)
	{
		Player p = ReInput.players.GetPlayer("Player0");
		p.controllers.ClearAllControllers();
		p.controllers.AddController(j, true); // assign joystick to player

	}

	void OnDestroy()
	{
		// Unsubscribe from events
		ReInput.ControllerConnectedEvent -= OnControllerConnected;
	}
}