using Godot;

namespace Axvemi.Commons;

[GlobalClass]
public partial class Area2DInteractableClick : Area2DInteractable
{
	public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
	{
		base._InputEvent(viewport, @event, shapeIdx);
		if (@event is InputEventMouseButton eventMouseButton)
		{
			if (eventMouseButton.ButtonIndex == MouseButton.Left && eventMouseButton.IsPressed())
			{
				Interact();
			}
		}
	}
}