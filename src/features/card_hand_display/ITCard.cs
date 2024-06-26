using System;
using Godot;

namespace Axvemi.GDCommons.Features.CardHandDisplay;

public interface ITCard
{
    //Could also just return a button but yeah
    Action MouseEntered { get; set; }
    Action MouseExited { get; set; }
    Action Pressed { get; set; }
    Action<InputEvent> GuiInput { get; set; }
}