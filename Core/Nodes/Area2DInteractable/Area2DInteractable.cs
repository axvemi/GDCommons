using Godot;

namespace Axvemi.Commons;

[GlobalClass]
public partial class Area2DInteractable : Area2D, IInteractable
{
	[Signal] public delegate void InteractedWithEventHandler();

	public void Interact()
	{
		EmitSignal(SignalName.InteractedWith);
	}
}