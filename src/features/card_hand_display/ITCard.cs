using System;
using Godot;

namespace Axvemi.GDCommons;

public interface ITCard
{
    Action MouseEntered { get; set; }
    Action MouseExited { get; set; }
    Action Pressed { get; set; }
    Action<InputEvent> GuiInput { get; set; }
}