using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardGameManager : MonoBehaviour
{
    public TMP_Text playerScoreText;
    public TMP_Text opponentScoreText;

    int playerScore = 0;
    int opponentScore = 0;



    public enum GameState {
        SETUP,
        OPPONENTDEAL,
        DEAL,
        PLAYERCARDFLIP,
        OPPONENTCHOOSE,
        CHOOSE,
        RESOLVE,
        CLEANUP,
        BIGCLEANUP
    }

    enum RoundOutcome
    {
        DRAW,
        WIN,
        LOSS
    }

    public List<Sprite> cardFaces = new List<Sprite>();

    
    public List<AudioClip> soundEffects = new List<AudioClip>();

    public static AudioSource myAudioSource;


    public static GameState state;

    public static List<GameObject> playerHand = new List<GameObject>();

    public List<GameObject> opponentHand = new List<GameObject>();

    public List<GameObject> discardPile = new List<GameObject>();

    public Transform discardTransform;


    public int playerHandCount;
    public Transform playerPos;

    public Transform opponentPos;

    static public GameObject playerChosenCard;
    public GameObject opponentChosenCard;


    float cardDealTime = .3f;
    float cardDealTimer = 0;


    float flipCardTime = .1f;
    float flipCardTimer = 0;

    float opponentSelectTime = 1f;
    float opponentSelectTimer = 0;

    float resolveSuspenseTime = 2f;
    float resolveSuspenseTimer = 0f;

    float postOutcomeRevealTime = 1f;
    float postOutcomeRevealTimer = 0f;


    float reshuffleIncrementTime = .05f; 
    float reshuffleIncrementTimer = 0f;

    float newRoundTime = .1f;
    float newRoundTimer = 0;

    int cardsUndiscarded = 0;


    void Start() {
        state = GameState.SETUP;

        myAudioSource = GetComponent<AudioSource>();
    }

    void Update() 
    {
        Debug.Log(state);

        playerScoreText.text = playerScore.ToString();
        opponentScoreText.text = opponentScore.ToString();


        switch (state) {
            case GameState.SETUP:
                ShuffleDeck();

                state = GameState.OPPONENTDEAL;

                break;
            case GameState.OPPONENTDEAL:

                if (cardDealTimer < cardDealTime)
                {
                    cardDealTimer += Time.deltaTime;
                }
                else
                {
                    cardDealTimer = 0;

                    DealOpponentCard();

                    if (opponentHand.Count == playerHandCount)
                    {
                        state = GameState.DEAL;
                    }
                    
                        

                }

                break;
            case GameState.DEAL:
                if (cardDealTimer < cardDealTime)
                {
                    cardDealTimer += Time.deltaTime;
                }
                else
                {
                    cardDealTimer = 0;
                    DealCard();

                    if (playerHand.Count == playerHandCount)
                    {
                        state = GameState.PLAYERCARDFLIP;
                    }

                }
                break;
            case GameState.PLAYERCARDFLIP:

                if (flipCardTimer < flipCardTime)
                {
                    flipCardTimer += Time.deltaTime;
                }
                else
                {
                    flipCardTimer = 0;
                    for (int i = 0; i < playerHandCount; i++)
                    {
                        playerHand[i].GetComponent<Card>().flipped = true;
                    }

                    state = GameState.OPPONENTCHOOSE;
                }

                break;

            case GameState.OPPONENTCHOOSE:

                if (opponentSelectTimer < opponentSelectTime)
                {
                    opponentSelectTimer += Time.deltaTime;
                }
                else
                {
                    opponentSelectTimer = 0;

                    myAudioSource.PlayOneShot(soundEffects[0]);

                    int randomIndex = Random.Range(0, 3);

                    GameObject randomOpposingCard = opponentHand[randomIndex];

                    //randomOpposingCard.transform.position -= new Vector3(0, 1, 0);

                    randomOpposingCard.GetComponent<Card>().positionWeLerpTo = opponentPos.position + new Vector3(1.3f, -2.1f, 0);//randomOpposingCard.transform.position - new Vector3(0, 1, 0);

                    opponentChosenCard = randomOpposingCard;

                    state = GameState.CHOOSE;

                }

                break;

            case GameState.CHOOSE:
                break;
            case GameState.RESOLVE:


                if (resolveSuspenseTimer < resolveSuspenseTime)
                {
                    resolveSuspenseTimer += Time.deltaTime;
                }
                else if (opponentChosenCard.GetComponent<Card>().flipped != true){

                    opponentChosenCard.GetComponent<Card>().flipped = true;

                    Sprite playerCardSprite = playerChosenCard.GetComponent<SpriteRenderer>().sprite;

                    Sprite opponentCardSprite = opponentChosenCard.GetComponent<Card>().faceSprite;

                    //opponentChosenCard.GetComponent<Card>().flipped = true;

                    RoundOutcome outcome = RoundOutcome.DRAW;


                    //Now we have to evaluate each different scenario! We'll start by seeing what happens when the player picks rock.
                    if (playerCardSprite == cardFaces[0])
                    {
                        Debug.Log("THE PLAYER CHOSE ROCK!");

                        if (opponentCardSprite == cardFaces[1])
                        {
                            Debug.Log("OPPONENT CHOSE PAPER");
                            outcome = RoundOutcome.LOSS;


                        }
                        else if (opponentCardSprite == cardFaces[2])
                        {

                            Debug.Log("OPPONENT CHOSE SCISSORS");
                            outcome = RoundOutcome.WIN;
                        }

                    }

                    //This time the player picked paper
                    if (playerCardSprite == cardFaces[1])
                    {
                        Debug.Log("THE PLAYER CHOSE PAPER!");

                        if (opponentCardSprite == cardFaces[2])
                        {
                            Debug.Log("OPPONENT CHOSE SCISSORS");
                            outcome = RoundOutcome.LOSS;


                        }
                        else if (opponentCardSprite == cardFaces[0])
                        {

                            Debug.Log("OPPONENT CHOSE ROCK");
                            outcome = RoundOutcome.WIN;
                        }

                    }

                    //This time the player picked scissors!
                    if (playerCardSprite == cardFaces[2])
                    {
                        Debug.Log("THE PLAYER CHOSE SCISSORS!");

                        if (opponentCardSprite == cardFaces[0])
                        {
                            Debug.Log("OPPONENT CHOSE ROCK");
                            outcome = RoundOutcome.LOSS;


                        }
                        else if (opponentCardSprite == cardFaces[1])
                        {

                            Debug.Log("OPPONENT CHOSE PAPER");
                            outcome = RoundOutcome.WIN;
                        }

                    }

                    if (outcome == RoundOutcome.WIN)
                    {
                        //Play happy noise
                        //Add player points

                        myAudioSource.PlayOneShot(soundEffects[1]);
                        playerScore++;

                    }
                    else if (outcome == RoundOutcome.LOSS)
                    {
                        //Play sad noise
                        //Add opponent points

                        myAudioSource.PlayOneShot(soundEffects[2]);
                        opponentScore++;

                    }
                    else
                    {

                    }


                }
                else if (postOutcomeRevealTimer < postOutcomeRevealTime)
                {
                    postOutcomeRevealTimer += Time.deltaTime;
                }else
                {
                    postOutcomeRevealTimer = 0;
                    resolveSuspenseTimer = 0;

                   
                    state = GameState.CLEANUP;

                }

                break;
            case GameState.CLEANUP:

                //Clean up opponent and player hands

                if (cardDealTimer < cardDealTime)
                {
                    cardDealTimer += Time.deltaTime;

                }
                else {
                    cardDealTimer = 0;

                    //Clean up the 2 played cards
                    if (opponentChosenCard != null)
                    {
                        DiscardCard(opponentHand, opponentChosenCard);
                        opponentChosenCard = null;
                    }
                    else if (playerChosenCard != null)
                    {
                        DiscardCard(playerHand, playerChosenCard);
                        playerChosenCard = null;
                    }
                    else if (opponentHand.Count > 0)
                    {
                        //Clean opponent hand first
                        DiscardCard(opponentHand, opponentHand[opponentHand.Count - 1]);

                    }
                    else if (playerHand.Count > 0)
                    {
                        //Then we clean player hand;
                        DiscardCard(playerHand, playerHand[playerHand.Count - 1]);
                    }
                    else
                    {
                        //By now the playing field should be all ready for another round!
                        //...Unless there are no more cards to draw from the deck!
                        //We'll have to check for that too.

                        if (DeckManager.deck.Count > 0)
                        {
                            state = GameState.OPPONENTDEAL;
                        }
                        else
                        {
                            state = GameState.BIGCLEANUP;
                        }


                    }




                }

                break;

            case GameState.BIGCLEANUP:

                if (reshuffleIncrementTimer < reshuffleIncrementTime)
                {
                    reshuffleIncrementTimer += Time.deltaTime;
                }
                else
                {
                    reshuffleIncrementTimer = 0;
                    if (discardPile.Count > 0)
                    {
                        myAudioSource.PlayOneShot(soundEffects[0]);

                        GameObject cardWeReturn = discardPile[discardPile.Count - 1];

                        discardPile.Remove(cardWeReturn);

                        Vector3 newPos = new Vector3(-discardTransform.position.x, discardTransform.position.y + .02f * DeckManager.deck.Count, 0 - .1f * DeckManager.deck.Count);

                        Card cardComponent = cardWeReturn.GetComponent<Card>();

                        cardComponent.positionWeLerpTo = newPos;

                        cardComponent.flipped = false;

                        DeckManager.deck.Add(cardWeReturn);





                    }
                    else if (DeckManager.deck.Count > cardsUndiscarded)
                    {
                        myAudioSource.PlayOneShot(soundEffects[0]);

                        GameObject cardWePlaceInFront = DeckManager.deck[DeckManager.deck.Count - 1];

                        DeckManager.deck.Remove(cardWePlaceInFront);

                        DeckManager.deck.Insert(cardsUndiscarded, cardWePlaceInFront);

                        Vector3 newPos = new Vector3(-discardTransform.position.x, discardTransform.position.y + .02f * cardsUndiscarded, -3 - .1f * cardsUndiscarded);

                        Card cardComponent = cardWePlaceInFront.GetComponent<Card>();

                        cardComponent.positionWeLerpTo = newPos;

                        cardsUndiscarded += 1;
                    }
                    else if( Vector3.Distance(DeckManager.deck[DeckManager.deck.Count - 1].transform.position, DeckManager.deck[DeckManager.deck.Count-1].GetComponent<Card>().positionWeLerpTo) < .1 )
                    {
                        state = GameState.SETUP;
                        cardsUndiscarded = 0;                    
                    }

                    
                }

                break;

        }    
    }

    void DealCard() {

        myAudioSource.PlayOneShot(soundEffects[0]);

        GameObject nextCard = DeckManager.deck[DeckManager.deck.Count - 1];
        Vector3 newPos = playerPos.transform.position;
        
        newPos.x = newPos.x + (1.3f * playerHand.Count);

        nextCard.GetComponent<Card>().positionWeLerpTo = newPos;

        //nextCard.transform.position = newPos;
        playerHand.Add(nextCard);
        DeckManager.deck.Remove(nextCard);

    }

    void DealOpponentCard() {
        //Debug.Log(DeckManager.deck.Count);

        myAudioSource.PlayOneShot(soundEffects[0]);

        GameObject nextCard = DeckManager.deck[DeckManager.deck.Count - 1];
        Vector3 newPos = opponentPos.transform.position;
        newPos.x = newPos.x + (1.3f * opponentHand.Count);

        nextCard.GetComponent<Card>().positionWeLerpTo = newPos;

        //nextCard.transform.position = newPos;
        opponentHand.Add(nextCard);
        DeckManager.deck.Remove(nextCard);
    }

    void ShuffleDeck() {
        for (int i = 0; i < DeckManager.deck.Count; i++)
        {
            GameObject elementOne = DeckManager.deck[i];
            int randomElementTwo = Random.Range(i, DeckManager.deck.Count);
            DeckManager.deck[i] = DeckManager.deck[randomElementTwo];
            DeckManager.deck[randomElementTwo] = elementOne;

            DeckManager.deck[i].transform.position = new Vector3(-discardTransform.position.x, discardTransform.position.y + .02f * i, 0-.1f * i);

            DeckManager.deck[i].GetComponent<Card>().positionWeLerpTo = DeckManager.deck[i].transform.position;
        }
    
    }

    void DiscardCard(List<GameObject> listWeRemoveFrom, GameObject cardWeRemove) {

        myAudioSource.PlayOneShot(soundEffects[0]);

        listWeRemoveFrom.Remove(cardWeRemove);
        Vector3 newPos = discardTransform.position - new Vector3(0, -.02f * discardPile.Count, .1f * discardPile.Count);

        //cardWeRemove.transform.position = discardTransform.position;
        //cardWeRemove.transform.position -= new Vector3(0, -.02f * discardPile.Count, .1f * discardPile.Count);


        Card cardComponent = cardWeRemove.GetComponent<Card>();
        cardComponent.positionWeLerpTo = newPos;
        cardComponent.flipped = true;


        discardPile.Add(cardWeRemove);



    }

}
