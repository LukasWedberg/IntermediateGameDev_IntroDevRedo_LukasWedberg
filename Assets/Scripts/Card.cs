using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public Sprite faceSprite;


    public Sprite backSprite;

    public SpriteRenderer myRenderer;

    public bool flipped = false;

    bool mouseOver = false;

    void start() {
        //myRenderer = GetComponent<SpriteRenderer>();
        backSprite = myRenderer.sprite;
    }

    void Update() {
        if (flipped)
        {
            //Debug.Log(myRenderer);

            myRenderer.sprite = faceSprite;

            
        }
    
    }

    void OnMouseDown() {

        //When it's time to pick a card, and this card is picked, we have to search the player's hand for this card.
        //If we don't find it, we definitely shouldn't let the player click on a card from a hand they don't have!
        //...as funny as that is.

        if (CardGameManager.state == CardGameManager.GameState.CHOOSE)
        {
            bool foundCard = false;

            for (int i = 0; i < CardGameManager.playerHand.Count; i++)
            {
                GameObject cardWeCheck = CardGameManager.playerHand[i];

                if (transform.gameObject == cardWeCheck)
                {
                    foundCard = true;
                }



            }

            if (foundCard)
            {

                mouseOver = true;

                CardGameManager.playerChosenCard = transform.gameObject;

                CardGameManager.state = CardGameManager.GameState.RESOLVE;
            }


        }
    }


}
