using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions; 
using UnityEngine.UI; 
using TMPro; 
using DG.Tweening;

using UnityEngine.SceneManagement; 


public class GameManager : MonoBehaviour
{
    [Header("Card Management")]
    //Simpler implemetation for prototype, can later be replaced with a scriptable object; ß
    [SerializeField] private List<Card> _cardDeck; 

    //this will be the running/used deck for the game
    [SerializeField] private List<Card> _activeDeck;


    //used to store cards until next turn comes 
    // for some reason, the cards won't distribute if I do not make the SerializeField
    [SerializeField] private List<Card> _p1Cards; 
    [SerializeField] private List<Card> _p2Cards; 
    [SerializeField] private List<Card> _matchingList = new List<Card>(2); 



    [Header("Placement Transforms")]
    [SerializeField] private Transform[] _spawns; 
    [SerializeField] private Transform[] _show;
    //this is the list for macthing pair of cards

    private int turnsMade = 0; 
    private int _p1Score; 
    private int _p2Score; 

    private bool isP1Turn = true; 
    private bool canPlay = false; 


    [Header("UI Management")]
    [SerializeField] private TMP_Text _p1ScoreText; 
    [SerializeField] private TMP_Text _p2ScoreText; 
    [SerializeField] private TMP_Text _p1CardsLeftText; 
    [SerializeField] private TMP_Text _p2CardsLeftText; 
    [SerializeField] private Button _p1Button; 
    [SerializeField] private Button _p2Button; 

    // [SerializeField] private AudioSource _cardFlip; 
    [SerializeField] private GameObject _winCanvas; 
    [SerializeField] private TMP_Text _winnerText; 

    [Header("Audio Source")]
    [SerializeField] private AudioSource[] _turnSounds; 
    [SerializeField] private AudioSource _matchFound; 
    [SerializeField] private AudioSource _shuffleSound; 
    
    void Start()
    {
        InstantiateAllCards(); 
        ShuffleDeck(); 
        StartCoroutine("DistributeCards"); 

        SetupPlayerButtons(); 
    }

    void SetupPlayerButtons()
    {
        _p1Button.interactable = true;  // Enable player 1's button
        _p2Button.interactable = false; 
    }
    void InstantiateAllCards()
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
            Card nextCard = DequeueCard(_activeDeck); 

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
        _shuffleSound.Play(); 

        
        UpdateCardsLeftUI(); 
        
        canPlay = true; 
    }

    private void HandlePlayerTurn()
    {
        if(!canPlay) return;
    
        PlayerTurnSound(); 
        MoveCard(); 
        SwitchPlayerTurn(); 
        UpdateCardsLeftUI();

        if(IsGameOver()) return;  
    }

    void PlayerTurnSound()
    {
        if(isP1Turn) _turnSounds[0].Play(); 
        else _turnSounds[1].Play(); 
    }

    void SwitchPlayerTurn()
    {
        _p1Button.interactable = !_p1Button.interactable;  
        _p2Button.interactable = !_p2Button.interactable;  
        isP1Turn = !isP1Turn; 

        turnsMade+=1; 
    }
    void MoveCard()
    {
        

        UpdateDispacementPosition(); 

        List<Card> list = GetCurrentPlayerDeck(); 
        
        Card c = DequeueCard(list); 
        TweenTransform(c.transform,GetCurrentPlayerDeckTransform(), 0.3f);
        PutInMatchingList(c); 

    }

    Card DequeueCard(List<Card> list)
    {
        Card c = list[0]; 
        list.RemoveAt(0); 
        return c; 
    }
    void UpdateDispacementPosition()
    {
        float z =  (float)(  -1 * (turnsMade / 1000f) );
        Vector3 displacement = new Vector3(0f,0f,z); 

        // if(isP1Turn) _show[0].position+=displacement; 
        // else  _show[1].position+=displacement; 
        GetCurrentPlayerDeckTransform().position += displacement; 
    }

    bool IsGameOver()
    {
        if(_p1Cards.Count <=0 || _p2Cards.Count <= 0) 
        {
            canPlay = !canPlay; 
            //enable game over canvas!!
            string player =  _p1Cards.Count ==0 && _p2Cards.Count== 1 ? "No one" : 
            (_p1Cards.Count > _p2Cards.Count ? "Red" : "Blue"); 

            _winnerText.text = $"{player} Wins!"; 
            _winCanvas.SetActive(true); 


            return true; 
        }
        else return false; 
    }

    void PutInMatchingList(Card c)
    {
        if(_matchingList.Count>1)
        {
            Card t = DequeueCard(_matchingList); 
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
            if(isP1Turn)  _p1Score+=1;    
            else _p2Score +=1; 
            UpdateText(); 

            _matchFound.Play(); 
            StartCoroutine("MoveAllToPlayer"); 
        }
    }

    Transform GetCurrentPlayerDeckTransform()
    {
        if(isP1Turn) return _show[0]; 
        else return _show[1]; 
    }
    Transform GetCurrentPlayerTransform()
    {
        if(isP1Turn) return _spawns[0]; 
        else return _spawns[1]; 
    }
    List<Card> GetCurrentPlayerDeck()
    {
        if(isP1Turn) return _p1Cards; 
        else return _p2Cards; 
    }
    IEnumerator MoveAllToPlayer()
    {
        canPlay = false; 
        List<Card> list = GetCurrentPlayerDeck(); 
        Transform playerDeck = GetCurrentPlayerTransform(); 

        yield return new WaitForSeconds(1.5f); 
        //move all cards to current player's deck
        while(_activeDeck.Count > 0 )
        {
            Card c = DequeueCard(_activeDeck); 
            list.Add(c); 
            TweenTransform(c.transform, playerDeck, 0.3f ); 
            yield return new WaitForSeconds(0.01f); 
        }
        while(_matchingList.Count > 0 )
        {
            Card c = DequeueCard(_matchingList); 
            list.Add(c); 
            TweenTransform(c.transform, playerDeck, 0.3f ); 
            yield return new WaitForSeconds(0.01f); 
        }

        _shuffleSound.Play(); 

        UpdateCardsLeftUI(); 
        canPlay = true; 
    }

 
    void TweenTransform(Transform a, Transform b, float duration)
    {
        // _cardFlip.Play(); 
        DOTween.To(()=> a.position, x=> a.position = x, b.position, duration);
    }

    private void UpdateText()
    {
        _p1ScoreText.text = _p1Score.ToString(); 
        _p2ScoreText.text = _p2Score.ToString(); 
    }

    private void UpdateCardsLeftUI()
    {
        _p1CardsLeftText.text = _p1Cards.Count.ToString(); 
        _p2CardsLeftText.text = _p2Cards.Count.ToString(); 
    }

    public void HandleTurn()
    {
        HandlePlayerTurn(); 
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu"); 
    }
}

