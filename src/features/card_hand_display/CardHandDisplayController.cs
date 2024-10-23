using System;
using System.Collections.Generic;
using Godot;

namespace Axvemi.GDCommons.CardHandDisplay;

public partial class CardHandDisplayController<TData, TCardController> : Node2D where TCardController : Node2D, ICardHandController<TData>
{
    public class CardAddedEventArgs : EventArgs
    {
        public TCardController CardController { get; }

        public CardAddedEventArgs(TCardController cardController)
        {
            CardController = cardController;
        }
    }

    public class CardSelectedEventArgs : EventArgs
    {
        public TCardController CardController { get; }

        public CardSelectedEventArgs(TCardController cardController)
        {
            CardController = cardController;
        }
    }
    public event EventHandler<CardAddedEventArgs> CardAdded;
    public event EventHandler<CardSelectedEventArgs> CardSelected;

    protected const float CenterAngle = 90;

    // OnReady
    public Node2D CardContainer { get; protected set; }


    [ExportGroup("Hand")]
    /// <summary>
    /// Max space angle allowed to cover by all the steps
    /// </summary>
    [Export] public float MaxAngle { get; protected set; } = 100;
    /// <summary>
    /// Max card rotation angle allowed
    /// </summary>
    [Export] public float MaxRotationAngle { get; protected set; } = 35;
    /// <summary>
    /// Angle space between cards
    /// </summary>
    [Export] public float AngleStep { get; protected set; } = 25;
    /// <summary>
    /// Rotation angle step per card
    /// </summary>
    [Export] public float RotationStep = 5;

    [ExportGroup("Card")]
    [Export] public float LerpSpeed { get; protected set; } = 5;

    [ExportGroup("Oval")]
    [Export] public Vector2 OvalCenterOffset { get; protected set; } = new Vector2(0, 100);
    [Export] public Vector2 OvalRadiusSize { get; protected set; } = new(480, 190);

    public PackedScene CardControllerPrefab;
    public GDModuleController<CardHandDisplayController<TData, TCardController>> ModuleController { get; private set; }
    public List<TCardController> Cards = new();
    public FocusedData<TCardController> FocusedData;
    public bool CanInteract = true;


    public override void _Ready()
    {
        base._Ready();
        CardContainer = GetNode<Node2D>("%CardContainer");

        ModuleController = new GDModuleController<CardHandDisplayController<TData, TCardController>>(this);
        ModuleController.Initialize();
    }

    /// <summary>
    /// Adds a new card controller to the hand
    /// </summary>
    /// <param name="data">Data of the card</param>
    public void AddCard(TData data)
    {
        var instance = CardControllerPrefab.Instantiate() as TCardController;
        CardContainer.AddChild(instance);
        Cards.Add(instance);

        instance.Initialize(data);

        // Set starting transform for the card
        TargetTransform cardTargetTransform = GetTransformForIndex(Cards.Count - 1);
        instance.Position = cardTargetTransform.Position;
        instance.RotationDegrees = cardTargetTransform.RotationDegrees;

        SetDefaultCardTransforms();
        CardAdded?.Invoke(this, new CardAddedEventArgs(instance));
    }

    /// <summary>
    /// Removes a card from the hand
    /// </summary>
    /// <param name="cardController">Card controller to remove</param>
    public void RemoveCard(TCardController cardController)
    {
        Cards.Remove(cardController);
        SetDefaultCardTransforms();
        cardController.QueueFree();
    }

    /// <summary>
    /// 
    /// </summary>
    public void RestoreFocusedCardVisually()
    {
        CardContainer.MoveChild(FocusedData.CardController, FocusedData.OriginalChildIndex);
        FocusedData.CardController.ZIndex = FocusedData.OriginalZIndex;

        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(FocusedData.CardController, Node2D.PropertyName.Position.ToString(), FocusedData.OriginalPosition, 0.1f);
        tween.Parallel().TweenProperty(FocusedData.CardController, Node2D.PropertyName.Scale.ToString(), Vector2.One, 0.1f);
    }

    /// <summary>
    /// Move each card to it's default transform
    /// </summary>
    public void SetDefaultCardTransforms()
    {
        for (int i = 0; i < Cards.Count; i++)
        {
            TCardController cardController = Cards[i];
            TargetTransform targetTransform = GetTransformForIndex(i);

            cardController.ZIndex = i;
            Tween tween = GetTree().CreateTween();
            tween.TweenProperty(cardController, Node2D.PropertyName.Position.ToString(), targetTransform.Position, 0.1f);
            tween.Parallel().TweenProperty(cardController, Node2D.PropertyName.RotationDegrees.ToString(), targetTransform.RotationDegrees, 0.05f);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cardController"></param>
    public void SetFocusedCardDataFromCard(TCardController cardController)
    {
        FocusedData = new FocusedData<TCardController>(cardController, cardController.GetIndex(), cardController.ZIndex, cardController.Position);
    }

    /// <summary>
    /// Get what should be the transform for a card with a certain index. The index can be a decimal
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public TargetTransform GetTransformForIndex(float index)
    {
        float currentStepMultiplier = ((Cards.Count - 1f) / 2f) - index;
        float totalTheoreticalAngle = Cards.Count * AngleStep;
        float angleStepToUse = totalTheoreticalAngle > MaxAngle ? (MaxAngle / (Cards.Count - 1)) : AngleStep;
        float totalTheoreticalRotationAngle = (Cards.Count - 1) * RotationStep;
        float rotationStepToUse = totalTheoreticalRotationAngle > MaxRotationAngle ? (MaxRotationAngle / (Cards.Count - 1)) : RotationStep;

        // Position
        float currentAngle = CenterAngle + angleStepToUse * currentStepMultiplier;
        Vector2 ovalAnglePosition = new(OvalRadiusSize.X * Mathf.Cos(Mathf.DegToRad(currentAngle)), -OvalRadiusSize.Y * Mathf.Sin(Mathf.DegToRad(currentAngle)));
        Vector2 targetPosition = OvalCenterOffset + ovalAnglePosition;

        // Rotation
        float targetRotation = currentStepMultiplier * -rotationStepToUse;

        return new TargetTransform(targetPosition, targetRotation);
    }

    public void InvokeCardSelected(CardSelectedEventArgs e)
    {
        CardSelected?.Invoke(this, e);
    }
}



public class TargetTransform
{
    public Vector2 Position { get; private set; }
    public float RotationDegrees { get; private set; }

    public TargetTransform(Vector2 position, float rotationDegrees)
    {
        Position = position;
        RotationDegrees = rotationDegrees;
    }
}

public class FocusedData<TCardController>
{
    public TCardController CardController { get; }
    public int OriginalChildIndex { get; }
    public int OriginalZIndex { get; }
    public Vector2 OriginalPosition { get; }

    public FocusedData(TCardController cardController, int originalChildIndex, int originalZIndex, Vector2 originalPosition)
    {
        CardController = cardController;
        OriginalChildIndex = originalChildIndex;
        OriginalZIndex = originalZIndex;
        OriginalPosition = originalPosition;
    }
}
