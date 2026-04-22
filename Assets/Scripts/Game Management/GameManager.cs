using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }
    
	[Header("Game State")]
	public GameState currentState = GameState.PlayPhase;
	public int currentQuarter = 1;
	public int currentScore = 0;
	public int targetScore = 25;
	public int remainingPlays = 15;
    
	[Header("Starting Deck")]
	public List<CardData> startingDeck = new List<CardData>();
    
	[Header("Deck Management")]
	private List<CardData> deck = new List<CardData>();
	private List<CardData> discardPile = new List<CardData>();
	public List<Card> hand = new List<Card>();
	public List<Card> playArea = new List<Card>();
    
	[Header("Player Cards (Bench)")]
	public List<PlayerCard> activePlayerCards = new List<PlayerCard>();
	public int maxPlayerCards = 5;
    
	[Header("Settings")]
	public int handSize = 8;
	public int basePlayCount = 15;
    
	[Header("Reward System")]
	public RewardUI rewardUI;
    
	[Header("Shop System")]
	public ShopUI shopUI;
	public bool enableShop = true; // Toggle shop on/off
	
	[Header("Opponent System")]
	public OpponentManager opponentManager;
    
	[Header("References")]
	public HandManager handManager;
	public UIManager uiManager;
	public Transform benchContainer;
	public GameObject playerCardPrefab;
    
	[Header("Possession System")]
	public int maxCardsPerPossession = 5;
	public int currentPossessionCards = 0;
	public int possessionsPerQuarter = 10; // Total possessions available
	public int remainingPossessions = 10;
	public bool autoEndTurn = false; // Auto-end when 5 cards played

	[Header("Possession Bonuses")]
	public bool enablePossessionBonuses = true;
	public float perfectPossessionBonus = 1.5f; // Bonus for using all 5 cards
	public int chainBonusPoints = 10; // Bonus points for 5-card possession
    
	[Header("Testing")]
	public PlayerCardData[] testPlayerCards;
	
	[Header("System Toggle")]
	public bool usePossessionSystem = true; // New system
	public bool useOldPlaySystem = false;   // Old unlimited system

    
	private int handSizeModifier = 0;
	private int playCountModifier = 0;
    
	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
		
		if (usePossessionSystem)
		{
			// Use new possession-based system
			maxCardsPerPossession = 5;
			possessionsPerQuarter = 10;
		}
		else
		{
			// Use old unlimited system
			maxCardsPerPossession = 999;
			remainingPlays = 15;
		}
	}
    
	void Start()
	{
		InitializeGame();
	}
    
	void Update()
	{
		// Keep keyboard shortcut for Unity Editor testing only
        #if UNITY_EDITOR
		//if (Input.GetKeyDown(KeyCode.P))
		//{
		//	TestAddPlayerCard();
		//}
        #endif
	}
    
	void InitializeGame()
	{
		deck = new List<CardData>(startingDeck);
		ShuffleDeck();
		StartQuarter();
	}
    
	public void StartQuarter()
	{
		Debug.Log($"Starting Quarter {currentQuarter}");
    
		currentState = GameState.DrawPhase;
    
		// Reset possession count
		remainingPossessions = possessionsPerQuarter;
		currentPossessionCards = 0;
    
		// Select opponent
		if (opponentManager != null)
		{
			opponentManager.SelectOpponent(currentQuarter);
			targetScore = opponentManager.GetQuarterTarget(currentQuarter, targetScore);
        
			int handPenalty = opponentManager.GetHandSizePenalty();
			handSize = Mathf.Max(4, handSize - handPenalty);
		}
    
		CalculateModifiers();
    
		currentScore = 0;
    
		DrawHand();
    
		foreach (PlayerCard playerCard in activePlayerCards)
		{
			playerCard.ApplyGlobalEffect(EffectTrigger.OnQuarterStart);
		}
    
		currentState = GameState.PlayPhase;
		uiManager.UpdateAllUI();
	}
    
	void CalculateModifiers()
	{
		handSizeModifier = 0;
		playCountModifier = 0;
        
		foreach (PlayerCard playerCard in activePlayerCards)
		{
			handSizeModifier += playerCard.GetHandSizeModifier();
			playCountModifier += playerCard.GetPlayCountModifier();
		}
        
		Debug.Log($"Hand size modifier: {handSizeModifier}, Play count modifier: {playCountModifier}");
	}
    
	void DrawHand()
	{
		int cardsToDraw = Mathf.Min(handSize + handSizeModifier, deck.Count + discardPile.Count);
        
		Debug.Log($"Drawing {cardsToDraw} cards");
        
		for (int i = 0; i < cardsToDraw; i++)
		{
			DrawCard();
		}
	}
    
	public void DrawCard()
	{
		if (deck.Count == 0)
		{
			if (discardPile.Count == 0)
			{
				Debug.LogWarning("No cards left to draw!");
				return;
			}
            
			deck = new List<CardData>(discardPile);
			discardPile.Clear();
			ShuffleDeck();
			Debug.Log("Reshuffled discard pile into deck");
		}
        
		CardData drawnCard = deck[0];
		deck.RemoveAt(0);
        
		Debug.Log($"Drew card: {drawnCard.cardName}");
        
		handManager.CreateCardInHand(drawnCard);
        
		uiManager.UpdateAllUI();
	}
    
	void ShuffleDeck()
	{
		for (int i = deck.Count - 1; i > 0; i--)
		{
			int j = Random.Range(0, i + 1);
			CardData temp = deck[i];
			deck[i] = deck[j];
			deck[j] = temp;
		}
        
		Debug.Log($"Deck shuffled. Cards in deck: {deck.Count}");
	}
    
	public bool CanPlayCard(Card card)
	{
		return currentState == GameState.PlayPhase && 
			currentPossessionCards < maxCardsPerPossession &&
			remainingPossessions > 0;
	}
	
	public void PlayCard(Card card)
	{
		if (!CanPlayCard(card)) return;
    
		hand.Remove(card);
		playArea.Add(card);
		currentPossessionCards++;
    
		Debug.Log($"Cards played this possession: {currentPossessionCards}/{maxCardsPerPossession}");
    
		if (opponentManager != null)
		{
			opponentManager.OnCardPlayed(card);
		}
    
		uiManager.UpdateAllUI();
    
		// Check if possession is full
		if (currentPossessionCards >= maxCardsPerPossession)
		{
			if (autoEndTurn)
			{
				// Auto-resolve possession
				Invoke("EndTurn", 0.5f); // Small delay for visual feedback
			}
			else
			{
				// Disable playing more cards, but let player manually end turn
				Debug.Log("Possession full! End turn to score.");
			}
		}
	}
    
	public void EndTurn()
	{
		if (playArea.Count == 0)
		{
			Debug.Log("No cards played this possession!");
        
			// Still counts as a used possession
			if (remainingPossessions <= 0)
			{
				CheckLossCondition();
			}
			return;
		}
    
		currentState = GameState.ResolvePhase;
    
		// Calculate score with possession bonuses
		int turnScore = CalculateTurnScore();
    
		// Apply possession bonus if all 5 cards used
		if (enablePossessionBonuses && currentPossessionCards == maxCardsPerPossession)
		{
			Debug.Log("PERFECT POSSESSION! Bonus applied!");
			turnScore = Mathf.RoundToInt(turnScore * perfectPossessionBonus);
			turnScore += chainBonusPoints;
        
			uiManager.ShowPossessionBonus("PERFECT POSSESSION!");
		}
    
		currentScore += turnScore;
    
		uiManager.ShowScorePopup(turnScore);
    
		foreach (Card card in playArea)
		{
			discardPile.Add(card.data);
			Destroy(card.gameObject);
		}
		playArea.Clear();
    
		// Reset possession
		currentPossessionCards = 0;
		remainingPossessions--;
    
		Debug.Log($"Possessions remaining: {remainingPossessions}");
    
		foreach (PlayerCard playerCard in activePlayerCards)
		{
			playerCard.ApplyGlobalEffect(EffectTrigger.OnTurnEnd);
		}
    
		uiManager.UpdateAllUI();
    
		if (currentScore >= targetScore)
		{
			QuarterWon();
		}
		else if (remainingPossessions <= 0)
		{
			CheckLossCondition();
		}
		else
		{
			// Draw new hand for next possession
			DrawHand();
			currentState = GameState.PlayPhase;
		}
	}
    
	int CalculateTurnScore()
	{
		int totalScore = 0;
		float totalMultiplier = 1f;
		bool isBelowHalf = currentScore < (targetScore / 2);
    
		int setupBonus = 0; // For setup cards
    
		for (int i = 0; i < playArea.Count; i++)
		{
			Card card = playArea[i];
			int cardScore = card.data.basePoints;
			float cardMultiplier = 1f;
        
			// Apply possession effects
			switch (card.data.possessionEffect)
			{
			case PossessionEffect.ChainBonus:
				// +value per card played before this
				cardScore += (i * card.data.possessionValue);
				Debug.Log($"Chain bonus: +{i * card.data.possessionValue}");
				break;
                
			case PossessionEffect.FinisherBonus:
				// Multiply if last card (5th position)
				if (i == playArea.Count - 1 && playArea.Count == maxCardsPerPossession)
				{
					cardMultiplier *= card.data.possessionValue;
					Debug.Log($"Finisher bonus: {card.data.possessionValue}x");
				}
				break;
                
			case PossessionEffect.SetupBonus:
				// Boost next N cards
				setupBonus = card.data.possessionValue;
				Debug.Log($"Setup: Next {setupBonus} cards get bonus");
				break;
			}
        
			// Apply setup bonus if active
			if (setupBonus > 0)
			{
				cardScore += 2; // Example: +2 per setup card
				setupBonus--;
			}
        
			// Apply player card effects
			foreach (PlayerCard playerCard in activePlayerCards)
			{
				EffectTrigger trigger = DetermineCardTrigger(card);
				playerCard.ApplyEffects(trigger, card, ref cardScore, ref cardMultiplier);
            
				if (isBelowHalf)
				{
					playerCard.ApplyEffects(EffectTrigger.BelowHalfTarget, card, ref cardScore, ref cardMultiplier);
				}
			}
        
			// Apply opponent defense
			if (opponentManager != null)
			{
				opponentManager.ApplyDefenseModifiers(ref cardScore, ref cardMultiplier, card);
			}
        
			totalScore += cardScore;
			totalMultiplier *= cardMultiplier;
		}
    
		// Detect combos
		if (ComboDetector.Instance != null)
		{
			ComboResult combo = ComboDetector.Instance.DetectCombos(playArea);
			totalMultiplier *= combo.multiplier;
			totalScore += combo.bonusPoints;
        
			if (combo.comboNames.Count > 0)
			{
				string comboText = string.Join(" + ", combo.comboNames);
				uiManager.ShowComboText(comboText, combo.multiplier);
            
				if (opponentManager != null)
				{
					opponentManager.OnCombo();
				}
			}
		}
		
		if (PositionSynergyDetector.Instance != null)
		{
			SynergyResult synergy = PositionSynergyDetector.Instance.EvaluatePlayArea(playArea);
			totalMultiplier *= synergy.multiplierBonus;
			totalScore += synergy.bonusPoints;

			if (synergy.activeSynergyNames.Count > 0)
			{
				string synText = string.Join(" + ", synergy.activeSynergyNames);
				uiManager.ShowComboText(synText, synergy.multiplierBonus); // Reuse combo popup
			}

			if (!string.IsNullOrEmpty(synergy.newlyDiscoveredName))
			{
				uiManager.ShowDiscoveryPopup(synergy.newlyDiscoveredName);
				StatsManager.Instance?.RecordSynergyDiscovery(synergy.newlyDiscoveredName);
			}
		}
    
		int finalScore = Mathf.RoundToInt(totalScore * totalMultiplier);
    
		if (opponentManager != null)
		{
			opponentManager.OnPlayerScore(finalScore);
		}
    
		return finalScore;
	}
	
	EffectTrigger DetermineCardTrigger(Card card)
	{
		if (card.data.cardType == CardType.Shot)
		{
			if (card.data.cardName.Contains("Three"))
				return EffectTrigger.OnThreePointer;
			else if (card.data.cardName.Contains("Layup"))
				return EffectTrigger.OnLayup;
			else if (card.data.cardName.Contains("Dunk"))
				return EffectTrigger.OnDunk;
        
			return EffectTrigger.OnShotPlayed;
		}
		else if (card.data.cardName.Contains("Assist"))
		{
			return EffectTrigger.OnAssist;
		}
    
		return EffectTrigger.OnCardPlayed;
	}
    
	void CheckGameContinuation()
	{
		if (remainingPlays <= 0)
		{
			CheckLossCondition();
			return;
		}
        
		if (hand.Count == 0)
		{
			Debug.Log("Hand empty - drawing new hand");
			DrawHand();
		}
        
		currentState = GameState.PlayPhase;
	}
    
	void CheckLossCondition()
	{
		if (remainingPossessions <= 0)
		{
			if (currentScore < targetScore)
			{
				QuarterLost();
			}
			else
			{
				QuarterWon();
			}
		}
		else
		{
			currentState = GameState.PlayPhase;
		}
	}
    
	void QuarterWon()
	{
		Debug.Log($"Quarter {currentQuarter} won!");
    
		currentState = GameState.QuarterEnd;
    
		// Reward money with opponent multiplier
		bool isPerfect = currentScore == targetScore;
		int baseMoney = CurrencyManager.Instance.baseMoneyPerQuarter + 
		(currentScore * CurrencyManager.Instance.moneyPerPoint);
    
		if (opponentManager != null)
		{
			baseMoney = Mathf.RoundToInt(baseMoney * opponentManager.GetMoneyMultiplier());
		}
    
		CurrencyManager.Instance.AddMoney(baseMoney);
    
		if (currentQuarter >= 4)
		{
			MatchWon();
		}
		else
		{
			if (enableShop && shopUI != null)
			{
				shopUI.ShowShop(currentQuarter, () => {
					ShowQuarterRewards();
				});
			}
			else
			{
				ShowQuarterRewards();
			}
		}
	}
    
	void QuarterLost()
	{
		Debug.Log("Quarter lost!");
		currentState = GameState.GameOver;
		uiManager.ShowGameOverScreen();
	}
    
	void MatchWon()
	{
		Debug.Log("Match won!");
		currentState = GameState.MatchWon;
    
		// Show final victory rewards
		rewardUI.ShowRewardScreen(4, () => {
			uiManager.ShowVictoryScreen();
		});
	}
    
	public void ContinueToNextQuarter()
	{
		foreach (Card card in hand)
		{
			deck.Add(card.data);
			Destroy(card.gameObject);
		}
		hand.Clear();
        
		deck.AddRange(discardPile);
		discardPile.Clear();
        
		StartQuarter();
	}
	
	void ShowQuarterRewards()
	{
		rewardUI.ShowRewardScreen(currentQuarter, () => {
			currentQuarter++;
			targetScore += 15;
			ContinueToNextQuarter();
		});
	}
    
	public void AddPlayerCard(PlayerCardData cardData)
	{
		if (activePlayerCards.Count >= maxPlayerCards)
		{
			Debug.LogWarning("Bench is full!");
			return;
		}
        
		GameObject playerCardObj = Instantiate(playerCardPrefab, benchContainer);
		PlayerCard playerCard = playerCardObj.GetComponent<PlayerCard>();
		playerCard.Initialize(cardData);
        
		activePlayerCards.Add(playerCard);
        
		Debug.Log($"Added player card: {cardData.playerName}");
        
		CalculateModifiers();
	}
	
	public void AddCardToDeck(CardData card)
	{
		startingDeck.Add(card);
		Debug.Log($"Added {card.cardName} to deck");
	}

    
	public void TestAddPlayerCard()
	{
		if (testPlayerCards != null && testPlayerCards.Length > 0)
		{
			PlayerCardData randomCard = testPlayerCards[Random.Range(0, testPlayerCards.Length)];
			AddPlayerCard(randomCard);
		}
	}
    
	public int GetDeckCount()
	{
		return deck.Count;
	}
    
	public int GetDiscardCount()
	{
		return discardPile.Count;
	}
    
	public void RestartGame()
	{
		Debug.Log("Restarting game...");
        
		foreach (Card card in hand)
		{
			Destroy(card.gameObject);
		}
		hand.Clear();
        
		foreach (Card card in playArea)
		{
			Destroy(card.gameObject);
		}
		playArea.Clear();
        
		foreach (PlayerCard playerCard in activePlayerCards)
		{
			Destroy(playerCard.gameObject);
		}
		activePlayerCards.Clear();
        
		deck.Clear();
		discardPile.Clear();
        
		currentQuarter = 1;
		currentScore = 0;
		targetScore = 25;
		remainingPlays = basePlayCount;
		currentState = GameState.PlayPhase;
        
		// Reset currency
		if (CurrencyManager.Instance != null)
		{
			CurrencyManager.Instance.ResetCurrency();
		}
        
		InitializeGame();
        
		uiManager.HideAllScreens();
		
		if (rewardUI != null)
		{
			rewardUI.HideRewardScreen();
		}
    
		if (shopUI != null)
		{
			shopUI.HideShop();
		}
	}
	
	public void RemoveCardFromDeck(CardData card)
	{
		if (startingDeck.Contains(card))
		{
			startingDeck.Remove(card);
			Debug.Log($"Removed {card.cardName} from deck. Deck size: {startingDeck.Count}");
		}
	}
}

public enum GameState
{
	DrawPhase,
	PlayPhase,
	ResolvePhase,
	QuarterEnd,
	MatchWon,
	GameOver
}