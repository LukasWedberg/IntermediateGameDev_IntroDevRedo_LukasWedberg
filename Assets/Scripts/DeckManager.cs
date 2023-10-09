using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public GameObject cardPrefab;

    public Sprite[] cardFaces;

    public int deckCount;

    public static List<GameObject> deck = new List<GameObject>();


    void Start()
    {
        for (int i = 0; i < deckCount; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, gameObject.transform);
            newCard.transform.position = transform.position;
            newCard.transform.position -= new Vector3(0, .02f * deck.Count, -.1f * deck.Count);
            Card newCardScript = newCard.GetComponent<Card>();
            newCardScript.faceSprite = cardFaces[i % 3];
            deck.Add(newCard);

        }

    }
}
