using System;
using Godot;

namespace Axvemi.Commons;

//TODO: Convert to scene when the commit gets merged
//https://github.com/godotengine/godot/pull/72619
//https://github.com/godotengine/godot/issues/27470
public partial class InteractableNotifier : Area2D, IInteractable
{
	[Signal] public delegate void InteractedWithEventHandler();

	public Action OnInteractedWith;
	public void Interact()
	{
		OnInteractedWith?.Invoke();
		EmitSignal(SignalName.InteractedWith);
	}
}