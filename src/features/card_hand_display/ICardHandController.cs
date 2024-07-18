using Godot;

namespace Axvemi.GDCommons.CardHandDisplay;
public interface ICardHandController<TData>
{
    public void Initialize(TData data);
    public Control GetControl();
}