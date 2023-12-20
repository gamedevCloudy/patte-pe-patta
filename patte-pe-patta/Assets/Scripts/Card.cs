using UnityEngine; 


public class Card : MonoBehaviour
{
    public enum Type
    {
        Hearts,
        Diamonds,
        Clubs,
        Spades
    }
    [SerializeField] private bool isBlack = false; 
    [SerializeField] private Type color; 
    [SerializeField] private int value; 


    public static bool operator ==(Card card1, Card card2)
    {
        return (card1.value == card2.value); 
    }

    public static bool operator !=(Card card1, Card card2)
    {
        return !(card1==card2); 
    }

}

