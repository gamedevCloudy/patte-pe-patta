using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions; 
using UnityEngine.UI; 
using TMPro; 
using DG.Tweening;


public class GameManager : MonoBehaviour
{
    //Simpler implemetation for prototype, can later be replaced with a scriptable object; 
    [SerializeField] private List<Card> _cardDeck; 

    //this will be the running/used deck for the game
    [SerializeField] private List<Card> _activeDeck;

    //used to store cards until next turn comes 
    [SerializeField] private List<Card> _p1Cards; 
    [SerializeField] private List<Card> _p2Cards; 

    [SerializeField] private Transform[] _spawns; 
    [SerializeField] private Transform[] _show;
    //this is the list for macthing pair of cards
    [SerializeField] private List<Card> _matchingList = new List<Card>(2); 

    private int turnsMade = 0; 
    private int _p1Score; 
    private int _p2Score; 

    [SerializeField] private TMP_Text _p1ScoreText; 
    [SerializeField] private TMP_Text _p2ScoreText; 
    private bool isP1Turn = true; 


    [Header("UI Management")]
    [SerializeField] private Button _p1Button; 
    [SerializeField] private Button _p2Button; 
    

    [SerializeField] private AudioSource[] _turnSounds; 
    
    [SerializeField] private bool canPlay = false; 
    void Start()
    {
        CreateDeck(); 
        ShuffleDeck(); 
        StartCoroutine("DistributeCards"); 
        // HandlePlayerTurn(); 

        _p1Button.interactable = true;  // Enable player 1's button
        _p2Button.interactable = false; 

    }

    void CreateDeck()
    {
        foreach(Card c in _cardDeck)
        {
            Card card =Instantiate(c, transform.position, Quaternion.identity); 
            _activeDeck.Add(card); 
        }
    }

    void ShuffleDeck()
    {
        Extensions.ListExtensions.Shuffle(_activeDeck); 
    }

   IEnumerator DistributeCards()
    {
        while (_activeDeck.Count > 0)
        {
            int index = _activeDeck.Count - 1; 
            Card nextCard = _activeDeck[index];  
            _activeDeck.RemoveAt(index);          

            if (_activeDeck.Count % 2 == 0)
            {
                _p1Cards.Add(nextCard);
               
                TweenTransform(nextCard.transform, _spawns[0], 0.3f);  
                yield return new WaitForSeconds(0.01f); 
    
            }
            else                            
            {
                _p2Cards.Add(nextCard);

                TweenTransform(nextCard.transform, _spawns[1], 0.3f);  
                yield return new WaitForSeconds(0.01f); 
            }
        }
        canPlay = true; 
    }

    private void HandlePlayerTurn()
    {
        if(!canPlay) return; 
        if (isP1Turn)
        {
            _turnSounds[0].Play(); 
            MoveCard(); 
        }
        else
        {
            _turnSounds[1].Play(); 
            MoveCard(); 
        }
        _p1Button.interactable = !_p1Button.interactable;  
        _p2Button.interactable = !_p1Button.interactable;  
        isP1Turn = !isP1Turn; 
        
        
        turnsMade+=1; 
    }

    void MoveCard()
    {
        if(_p1Cards.Count <=0 || _p2Cards.Count <= 0) 
        {
            canPlay = !canPlay; 
            return; 
        }
            

        float z =  (float)(  -1 * (turnsMade / 1000f) );

        Vector3 displacement = new Vector3(0f,0f,z); 
        
        if(isP1Turn)
        {
            Card c = _p1Cards[0]; 
            _p1Cards.RemoveAt(0); 
           
            PutInMatchingList(c); 
            _show[0].position+=displacement; 

            TweenTransform(c.transform, _show[0], 0.3f); 
        }
        else{
            Card c = _p2Cards[0]; 
            _p2Cards.RemoveAt(0); 

            PutInMatchingList(c); 
            _show[1].position+=displacement; 

            TweenTransform(c.transform, _show[1], 0.3f); 
        }
    }

    void PutInMatchingList(Card c)
    {
        if(_matchingList.Count>1)
        {
            Card t = _matchingList[0];
            _matchingList.RemoveAt(0); 
            _activeDeck.Add(t); 
        }
        _matchingList.Add(c); 
        CompareCards(); 
    }

    void CompareCards()
    {
        if(_matchingList.Count<2) return; 
        if(_matchingList[0] == _matchingList[1])
        {
            if(isP1Turn){
                _p1Score+=1; 
                //move all cards to player 1
            } 
            else 
            {
                _p2Score +=1; 
                //move all cards to player 2
            }
            UpdateText(); 
            Debug.Log($"{_p1Score} {_p2Score}");  
        }
    }

    IEnumerator MoveToPlayer(int player)
    {
        List<Card> _listToAddTo; 
        
        if(player >= 3 || player <= -1) return; 
        if(player == 1){
            
        }
        else if (player == 2){

        }
        canPlay = false; 

        int index = _activeDeck.Count - 1; 
        while (_activeDeck.Count > 0)
        {
            Card nextCard = _activeDeck[index];  
            _activeDeck.RemoveAt(index);          

            _p1Cards.Add(nextCard);
               
            TweenTransform(nextCard.transform, _spawns[0], 0.3f);  
            yield return new WaitForSeconds(0.01f); 
        }
        canPlay = true; 
    }
    void TweenTransform(Transform a, Transform b, float duration)
    {
        DOTween.To(()=> a.position, x=> a.position = x, b.position, duration);
    }

    private void UpdateText()
    {
        _p1ScoreText.text = _p1Score.ToString(); 
        _p2ScoreText.text = _p2Score.ToString(); 
    }

    public void P1()
    {
        HandlePlayerTurn(); 
    }
    public void P2()
    {
        HandlePlayerTurn(); 
    }
}

