using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGameManager : MonoBehaviour
{
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

    void Start() {
        state = GameState.SETUP;
    }

    void Update() 
    {
        Debug.Log(state);

        switch (state) {
            case GameState.SETUP:
                ShuffleDeck();

                state = GameState.OPPONENTDEAL;

                break;
            case GameState.OPPONENTDEAL:
                if (opponentHand.Count < playerHandCount)
                {
                    DealOpponentCard();
                }
                else
                {
                    state = GameState.DEAL;
                }

                break;
            case GameState.DEAL:
                if (playerHand.Count < playerHandCount)
                {
                    DealCard();
                }
                else{
                    state = GameState.PLAYERCARDFLIP;
                }
                break;
            case GameState.PLAYERCARDFLIP:
                for (int i = 0; i < playerHandCount; i++)
                {
                    playerHand[i].GetComponent<Card>().flipped = true; 
                }

                state = GameState.OPPONENTCHOOSE;

                break;

            case GameState.OPPONENTCHOOSE:

                int randomIndex = Random.Range(0, 3);

                GameObject randomOpposingCard = opponentHand[randomIndex];

                randomOpposingCard.transform.position -= new Vector3(0, 1, 0);

                opponentChosenCard = randomOpposingCard;

                state = GameState.CHOOSE;

                break;

            case GameState.CHOOSE:
                break;
            case GameState.RESOLVE:

                Sprite playerCardSprite = playerChosenCard.GetComponent<SpriteRenderer>().sprite;

                Sprite opponentCardSprite = opponentChosenCard.GetComponent<Card>().faceSprite;

                opponentChosenCard.GetComponent<Card>().flipped = true;

                RoundOutcome outcome = RoundOutcome.DRAW;
                

                //Now we have to evaluate each different scenario! We'll start by seeing what happens when the player picks rock.
                if (playerCardSprite == cardFaces[0])
                {
                    Debug.Log("THE PLAYER CHOSE ROCK!");

                    if (opponentCardSprite == cardFaces[1])
                    {
                        Debug.Log("OPPONENT CHOSE PAPER");
                        outcome = RoundOutcome.LOSS;


                    } else if (opponentCardSprite == cardFaces[2]) {

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

                }
                else if (outcome == RoundOutcome.LOSS)
                {
                    //Play sad noise
                    //Add opponent points

                }
                else
                {

                }

                state = GameState.CLEANUP;



                break;
            case GameState.CLEANUP:

                //Clean up opponent and player hands

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
                else {
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











                



                break;

        }    
    }

    void DealCard() {        
        GameObject nextCard = DeckManager.deck[DeckManager.deck.Count - 1];
        Vector3 newPos = playerPos.transform.position;
        newPos.x = newPos.x + (2f * playerHand.Count);
        nextCard.transform.position = newPos;
        playerHand.Add(nextCard);
        DeckManager.deck.Remove(nextCard);

    }

    void DealOpponentCard() {
        GameObject nextCard = DeckManager.deck[DeckManager.deck.Count - 1];
        Vector3 newPos = opponentPos.transform.position;
        newPos.x = newPos.x + (2f * opponentHand.Count);
        nextCard.transform.position = newPos;
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
        }
    
    }

    void DiscardCard(List<GameObject> listWeRemoveFrom, GameObject cardWeRemove) {
        listWeRemoveFrom.Remove(cardWeRemove);
        cardWeRemove.transform.position = discardTransform.position;
        cardWeRemove.transform.position -= new Vector3(0, -.02f * discardPile.Count, .1f * discardPile.Count);


        discardPile.Add(cardWeRemove);



    }

}
