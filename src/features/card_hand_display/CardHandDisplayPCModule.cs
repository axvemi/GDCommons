using Axvemi.Commons;
using Godot;

namespace Axvemi.GDCommons.CardHandDisplay;

public partial class CardHandDisplayPCModule<TData, TCardController> : Node, IModule<CardHandDisplayController<TData, TCardController>> where TCardController : Node2D, ICardHandController<TData>
{
    /// <summary>
    /// Scale being set for the card when it's focused
    /// </summary>
    [Export] private float FocusScale = 1.5f;
    /// <summary>
    /// Amount to move in the Y axis when it's focused
    /// </summary>
    [Export] private int FocusMoveYAmount;

    public ModuleController<CardHandDisplayController<TData, TCardController>> ModuleController { get; set; }
    public CardHandDisplayController<TData, TCardController> ModuleOwner => ModuleController.Owner;
    protected bool IsDraggingCard;

    public void OnModulesReady()
    {
        ModuleOwner.CardAdded += OnCardAdded;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (!ModuleOwner.CanInteract)
        {
            return;
        }
        if (IsDraggingCard && @event is InputEventMouseMotion eventMouseMotion && ModuleOwner.FocusedData != null)
        {
            ModuleOwner.FocusedData.CardController.GlobalPosition = eventMouseMotion.Position;
        }
    }

    /// <summary>
    /// Makes the cardController zoomed
    /// TODO: The card doesn't set the final correct position because there is a tween already running.
    /// ie: Hover over the card to focus it, exit the mouse, and hover it before that first animation is done
    /// </summary>
    /// <param name="cardController"></param>
    protected void ZoomCard(TCardController cardController)
    {
        cardController.ZIndex = ModuleOwner.Cards.Count;

        Tween tween = GetTree().CreateTween();
        //Vector2 tweenTargetPosition = ModuleOwner.GetTransformForIndex(cardController.GetIndex()).Position + new Vector2(0, FocusMoveYAmount);
        Vector2 tweenTargetPosition = ModuleOwner.GetTransformForIndex(cardController.GetIndex()).Position;
        tweenTargetPosition.Y = (-cardController.GetSize().Y * FocusScale / 2) + FocusMoveYAmount;
        tween.TweenProperty(cardController, Node2D.PropertyName.Position.ToString(), tweenTargetPosition, 0.1f);
        tween.Parallel().TweenProperty(cardController, Node2D.PropertyName.Scale.ToString(), new Vector2(FocusScale, FocusScale), 0.1f);
        tween.Parallel().TweenProperty(cardController, Node2D.PropertyName.RotationDegrees.ToString(), 0, 0.1f);
        ModuleOwner.CardContainer.MoveChild(cardController, ModuleOwner.Cards.Count);
    }

    /// <summary>
    /// Move the cards aside from the index point. The card in that point will not be moved
    /// </summary>
    /// <param name="indexPoint"></param>
    protected void MoveCardsAside(int indexPoint)
    {
        bool isFirstHalf = true;
        for (int i = 0; i < ModuleOwner.Cards.Count; i++)
        {
            if (i == indexPoint)
            {
                isFirstHalf = false;
                continue;
            }
            float indexToUse = isFirstHalf ? Mathf.Max(0, i - 0.5f) : Mathf.Min(ModuleOwner.Cards.Count - 1, i + 0.5f);
            TCardController cardController = ModuleOwner.Cards[i];
            TargetTransform targetTransform = ModuleOwner.GetTransformForIndex(indexToUse);

            cardController.ZIndex = i;
            Tween tween = GetTree().CreateTween();
            tween.TweenProperty(cardController, Node2D.PropertyName.Position.ToString(), targetTransform.Position, 0.1f);
            tween.Parallel().TweenProperty(cardController, Node2D.PropertyName.RotationDegrees.ToString(), targetTransform.RotationDegrees, 0.05f);
        }
    }

    private void OnCardAdded(object sender, CardHandDisplayController<TData, TCardController>.CardAddedEventArgs e)
    {
        TCardController cardController = e.CardController;
        cardController.GetControl().MouseEntered += () => OnCardControllerMouseEntered(cardController);
        cardController.GetControl().MouseExited += () => OnCardControllerMouseExited(cardController);
        cardController.GetControl().GuiInput += (@event) => OnCardControllerGui(@event, cardController);
    }

    protected void OnDropFocusedCard()
    {
        TCardController cardController = ModuleOwner.FocusedData.CardController;
        if (cardController.GlobalPosition.Y < GetViewport().GetVisibleRect().Size.Y / 2)
        {
            ModuleOwner.InvokeCardSelected(new CardHandDisplayController<TData, TCardController>.CardSelectedEventArgs(cardController));
        }
    }

    protected virtual void OnCardControllerMouseEntered(TCardController cardController)
    {
        if (!ModuleOwner.CanInteract)
        {
            return;
        }
        if (ModuleOwner.FocusedData != null)
        {
            return;
        }
        ModuleOwner.SetFocusedCardDataFromCard(cardController);
        MoveCardsAside(cardController.GetIndex());
        ZoomCard(cardController);
    }

    protected virtual void OnCardControllerMouseExited(TCardController cardController)
    {
        if (!ModuleOwner.CanInteract)
        {
            return;
        }
        if (ModuleOwner.FocusedData == null || ModuleOwner.FocusedData.CardController != cardController)
        {
            return;
        }
        ModuleOwner.RestoreFocusedCardVisually();
        ModuleOwner.SetDefaultCardTransforms();
        ModuleOwner.FocusedData = null;
    }

    protected virtual void OnCardControllerGui(InputEvent @event, TCardController cardController)
    {
        if (!ModuleOwner.CanInteract)
        {
            return;
        }
        if (@event is InputEventMouseButton eventMouseButton)
        {
            //Focus card
            if (eventMouseButton.Pressed)
            {
                if (ModuleOwner.FocusedData == null)
                {
                    ModuleOwner.SetFocusedCardDataFromCard(cardController);
                    IsDraggingCard = true;
                }
                else if (cardController == ModuleOwner.FocusedData.CardController)
                {
                    IsDraggingCard = true;
                }
            }

            //Drop card
            if (ModuleOwner.FocusedData != null && !eventMouseButton.Pressed)
            {
                OnDropFocusedCard();
                ModuleOwner.SetDefaultCardTransforms();
                ModuleOwner.RestoreFocusedCardVisually();
                ModuleOwner.FocusedData = null;
                IsDraggingCard = false;
            }
        }
    }


}
