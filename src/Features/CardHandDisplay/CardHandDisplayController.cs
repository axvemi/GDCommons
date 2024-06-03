using System;
using System.Collections.Generic;
using System.Linq;
using Axvemi.Commons.Modules;
using Godot;

namespace Axvemi.GDCommons.Features.CardHandDisplay;

public partial class CardHandDisplayController<TCard> : Node2D where TCard : Node2D
{
	public class CardSelectedEventArgs : EventArgs
	{
		public TCard Card { get; }

		public CardSelectedEventArgs(TCard card)
		{
			Card = card;
		}
	}

	public class CardControllerAddedEventArgs : EventArgs
	{
		public TCard Card { get; }

		public CardControllerAddedEventArgs(TCard card)
		{
			Card = card;
		}
	}

	private const float CenterAngle = 90;

	public event Action<CardSelectedEventArgs> CardSelected;
	public event Action<CardControllerAddedEventArgs> CardAdded;

	[ExportGroup("Hand")]
	/// <summary>
	/// Max space angle allowed to cover by all the steps
	/// </summary>
	[Export] private float _maxAngle = 100;
	/// <summary>
	/// Max card rotation angle allowed
	/// </summary>
	[Export] private float _maxRotationAngle = 35;
	/// <summary>
	/// Angle space between cards
	/// </summary>
	[Export] private float _angleStep = 25;
	/// <summary>
	/// Rotation angle step per card
	/// </summary>
	[Export] private float _rotationStep = 5;

	[ExportGroup("Card")]
	[Export] private float _lerpSpeed = 5;

	[ExportGroup("Oval")]
	[Export] private Vector2 _ovalCenterOffset = new Vector2(0, 100);
	[Export] private Vector2 _ovalRadiusSize = new(480, 190);

	public GDModuleController<CardHandDisplayController<TCard>> ModuleController { get; private set; }
	public List<TCard> CardHandList = new();
	public FocusedCardData<TCard> FocusedData;
	public Node2D CardContainer { get; private set; }
	private Vector2 _ovalCenter;

	public override void _Ready()
	{
		base._Ready();

		CardContainer = GetNode<Node2D>("CardContainer");
		_ovalCenter = GlobalPosition + _ovalCenterOffset;

		ModuleController = new GDModuleController<CardHandDisplayController<TCard>>(this);
		ModuleController.Initialize();

		GetTree().Root.SizeChanged += OnRootSizeChanged;
	}

	public void AddCard(TCard card)
	{

		CardContainer.AddChild(card);
		CardHandList.Add(card);

		CardTargetTransform cardTargetTransform = GetTransformForIndex(CardHandList.Count - 1);
		card.GlobalPosition = cardTargetTransform.GlobalPosition;
		card.RotationDegrees = cardTargetTransform.Rotation;

		SetDefaultCardTransforms();
		CardAdded?.Invoke(new CardControllerAddedEventArgs(card));
	}
	public void RemoveCard(TCard card)
	{
		CardHandList.Remove(card);
		SetDefaultCardTransforms();
		card.QueueFree();
	}

	public void RestoreFocusedCardVisually()
	{
		TCard card = FocusedData.Card;

		CardContainer.MoveChild(card, FocusedData.OriginalChildIndex);
		card.ZIndex = FocusedData.OriginalZIndex;

		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(card, Node2D.PropertyName.GlobalPosition.ToString(), FocusedData.OriginalGlobalPosition, 0.1f);
		tween.Parallel().TweenProperty(card, Node2D.PropertyName.Scale.ToString(), Vector2.One, 0.1f);
	}

	/// <summary>
	/// Move each CardController to it's default position
	/// </summary>
	public void SetDefaultCardTransforms()
	{
		for (int i = 0; i < CardHandList.Count; i++)
		{
			TCard card = CardHandList[i];
			CardTargetTransform cardTargetTransform = GetTransformForIndex(i);

			card.ZIndex = i;
			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(card, Node2D.PropertyName.GlobalPosition.ToString(), cardTargetTransform.GlobalPosition, 0.1f);
			tween.Parallel().TweenProperty(card, Node2D.PropertyName.RotationDegrees.ToString(), cardTargetTransform.Rotation, 0.05f);
		}
	}

	public virtual void SelectCard(TCard card)
	{
		if (!CanSelectCard(card))
		{
			return;
		}
		CardSelected?.Invoke(new CardSelectedEventArgs(card));
	}

	public virtual bool CanSelectCard(TCard card) => true;

	public void SetFocusedCardDataFromCard(TCard card)
	{
		CardTargetTransform cardTargetTransform = GetTransformForIndex(card.GetIndex());
		FocusedData = new FocusedCardData<TCard>(card, card.GetIndex(), card.ZIndex, cardTargetTransform.GlobalPosition);
	}

	/// <summary>
	/// Get what should be the default transform for a card with index index.
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	public CardTargetTransform GetTransformForIndex(float index)
	{
		float currentStepMultiplier = ((float)(CardHandList.Count - 1) / 2) - index;
		float totalTheoreticalAngle = CardHandList.Count * _angleStep;
		float angleStepToUse = totalTheoreticalAngle > _maxAngle ? (_maxAngle / (CardHandList.Count - 1)) : _angleStep;
		float totalTheoreticalRotationAngle = (CardHandList.Count - 1) * _rotationStep;
		float rotationStepToUse = totalTheoreticalRotationAngle > _maxRotationAngle ? (_maxRotationAngle / (CardHandList.Count - 1)) : _rotationStep;

		//Position
		float currentAngle = CenterAngle + angleStepToUse * currentStepMultiplier;
		Vector2 ovalAnglePosition = new(_ovalRadiusSize.X * Mathf.Cos(Mathf.DegToRad(currentAngle)), -_ovalRadiusSize.Y * Mathf.Sin(Mathf.DegToRad(currentAngle)));
		Vector2 targetGlobalPosition = _ovalCenter + ovalAnglePosition;

		//Rotation
		float targetRotation = currentStepMultiplier * -rotationStepToUse;

		return new CardTargetTransform(targetGlobalPosition, targetRotation);
	}

	private void OnRootSizeChanged()
	{
		_ovalCenter = GlobalPosition + _ovalCenterOffset;
		SetDefaultCardTransforms();
	}
}

public class CardTargetTransform
{
	public Vector2 GlobalPosition { get; private set; }
	public float Rotation { get; private set; }

	public CardTargetTransform(Vector2 position, float rotation)
	{
		GlobalPosition = position;
		Rotation = rotation;
	}
}

public class FocusedCardData<TCard>
{
	public TCard Card { get; }
	public int OriginalChildIndex { get; }
	public int OriginalZIndex { get; }
	public Vector2 OriginalGlobalPosition { get; }

	public FocusedCardData(TCard card, int originalChildIndex, int originalZIndex, Vector2 originalGlobalPosition)
	{
		Card = card;
		OriginalChildIndex = originalChildIndex;
		OriginalZIndex = originalZIndex;
		OriginalGlobalPosition = originalGlobalPosition;
	}
}
