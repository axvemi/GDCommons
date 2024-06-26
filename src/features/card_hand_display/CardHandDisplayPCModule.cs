using Axvemi.Commons;
using Godot;

namespace Axvemi.GDCommons;

public partial class CardHandDisplayPCModule<TCard> : Node, IModule<CardHandDisplayController<TCard>> where TCard : Node2D, ITCard
{
    [Export] private float _focusScaleSize = 1.5f;
    [Export] private int _focusMoveYAmount = -250;

    public ModuleController<CardHandDisplayController<TCard>> ModuleController { get; set; }
    protected CardHandDisplayController<TCard> ModuleOwner => ModuleController.Owner;
    protected bool IsDraggingCard;

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
        //Move card to mouse
        if (IsDraggingCard && @event is InputEventScreenDrag dragEvent && ModuleOwner.FocusedData != null)
        {
            ModuleOwner.FocusedData.Card.GlobalPosition = dragEvent.Position;
        }
    }

    protected void ZoomCard(TCard cardController)
    {
        cardController.ZIndex = ModuleOwner.CardHandList.Count;
        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(cardController, Node2D.PropertyName.GlobalPosition.ToString(), ModuleOwner.GetTransformForIndex(cardController.GetIndex()).GlobalPosition + new Vector2(0, _focusMoveYAmount), 0.1f);
        tween.Parallel().TweenProperty(cardController, Node2D.PropertyName.Scale.ToString(), new Vector2(_focusScaleSize, _focusScaleSize), 0.1f);
        tween.Parallel().TweenProperty(cardController, Node2D.PropertyName.RotationDegrees.ToString(), 0, 0.1f);
        ModuleOwner.CardContainer.MoveChild(cardController, ModuleOwner.CardHandList.Count);
    }

    /// <summary>
    /// Move the cards aside from the index point. The card in that point will not be moved
    /// </summary>
    /// <param name="indexPoint"></param>
    protected void MoveCardsAside(int indexPoint)
    {
        bool firstHalf = true;
        for (int i = 0; i < ModuleOwner.CardHandList.Count; i++)
        {
            if (i == indexPoint)
            {
                firstHalf = false;
                continue;
            }
            float indexToUse = firstHalf ? Mathf.Max(0, i - 0.5f) : Mathf.Min(ModuleOwner.CardHandList.Count - 1, i + 0.5f);
            TCard card = ModuleOwner.CardHandList[i];
            CardTargetTransform cardTargetTransform = ModuleOwner.GetTransformForIndex(indexToUse);

            card.ZIndex = i;
            Tween tween = GetTree().CreateTween();
            tween.TweenProperty(card, Node2D.PropertyName.GlobalPosition.ToString(), cardTargetTransform.GlobalPosition, 0.1f);
            tween.Parallel().TweenProperty(card, Node2D.PropertyName.RotationDegrees.ToString(), cardTargetTransform.Rotation, 0.05f);
        }
    }

    protected virtual void OnCardAdded(CardHandDisplayController<TCard>.CardControllerAddedEventArgs args)
    {
        TCard card = args.Card;
        card.MouseEntered += () => OnCardControllerMouseEntered(card);
        card.MouseExited += () => OnCardControllerMouseExited(card);
        card.GuiInput += (@event) => OnCardControllerGui(@event, card);
    }

    protected virtual void OnCardControllerMouseEntered(TCard card)
    {
        if (ModuleOwner.FocusedData != null) return;
        ModuleOwner.SetFocusedCardDataFromCard(card);
        MoveCardsAside(card.GetIndex());
        ZoomCard(card);
    }

    protected virtual void OnCardControllerMouseExited(TCard card)
    {
        if (ModuleOwner.FocusedData == null || ModuleOwner.FocusedData.Card != card) return;
        ModuleOwner.RestoreFocusedCardVisually();
        ModuleOwner.SetDefaultCardTransforms();
        ModuleOwner.FocusedData = null;
    }

    protected virtual void OnCardControllerGui(InputEvent @event, TCard card)
    {
        if (@event is InputEventScreenTouch touchEvent)
        {
            //Focus card
            if (touchEvent.Pressed)
            {
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

            //Drop focused card
            if (ModuleOwner.FocusedData != null && !touchEvent.Pressed)
            {
                OnDropFocusedCard();
                ModuleOwner.SetDefaultCardTransforms();
                ModuleOwner.RestoreFocusedCardVisually();
                ModuleOwner.FocusedData = null;
                IsDraggingCard = false;
            }
        }
    }

    protected void OnDropFocusedCard()
    {
        //If dropped above half of the screen, select the card
        TCard cardController = ModuleOwner.FocusedData.Card;
        if (cardController.GlobalPosition.Y < GetViewport().GetVisibleRect().Size.Y / 2)
        {
            ModuleOwner.SelectCard(cardController);
        }
    }
}
