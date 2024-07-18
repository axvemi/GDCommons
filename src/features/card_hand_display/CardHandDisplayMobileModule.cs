using System;
using Axvemi.Commons;
using Godot;

namespace Axvemi.GDCommons.CardHandDisplay;

[Obsolete]
public partial class CardHandDisplayMobileModule<TData, TCardController> : Node, IModule<CardHandDisplayController<TData, TCardController>> where TCardController : Node2D, ICardHandController<TData>
{
    /*protected const float TapTime = 0.2f;
    public ModuleController<CardHandDisplayController<TCard>> ModuleController { get; set; }
    protected CardHandDisplayController<TCard> ModuleOwner => ModuleController.Owner;

    protected bool IsDraggingCard;
    private float _remainingCardTapTime;

    public void OnModulesReady()
    {
    }

    public void Initialize()
    {
        ModuleOwner.CardAdded += OnCardAdded;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        //Move card to the finger position
        if (IsDraggingCard && @event is InputEventScreenDrag dragEvent && ModuleOwner.FocusedData != null)
        {
            ModuleOwner.FocusedData.Card.GlobalPosition = dragEvent.Position;
        }

        //Hide detail / drop card
        if (@event is InputEventScreenTouch touchEvent && !touchEvent.Pressed)
        {
            if (ModuleOwner.FocusedData != null)
            {
                OnDropFocusedCard();
                ModuleOwner.SetDefaultCardTransforms();
                ModuleOwner.RestoreFocusedCardVisually();
                ModuleOwner.FocusedData = null;
                IsDraggingCard = false;
            }
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        _remainingCardTapTime -= (float)delta;
    }

    protected virtual void OnGuiCardControllerCoverButton(InputEvent @event, TCard card)
    {
        if (@event is InputEventScreenTouch touchEvent)
        {
            if (touchEvent.Pressed)
            {
                _remainingCardTapTime = TapTime;
                //Focus and drag card
                if (ModuleOwner.FocusedData == null)
                {
                    ModuleOwner.SetFocusedCardDataFromCard(card);
                    IsDraggingCard = true;
                }
                else if (card == ModuleOwner.FocusedData.Card)
                {
                    IsDraggingCard = true;
                }
            }
        }
    }

    private void OnCardAdded(CardHandDisplayController<TCard>.CardControllerAddedEventArgs args)
    {
        args.Card.GuiInput += (@event) => OnGuiCardControllerCoverButton(@event, args.Card);
    }

    private void OnDropFocusedCard()
    {
        //If dropped above half of the screen, select the card
        TCard card = ModuleOwner.FocusedData.Card;
        if (card.GlobalPosition.Y < GetViewport().GetVisibleRect().Size.Y / 2)
        {
            ModuleOwner.SelectCard(card);
        }
    }*/
    public ModuleController<CardHandDisplayController<TData, TCardController>> ModuleController { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void OnModulesReady()
    {
        throw new NotImplementedException();
    }
}
