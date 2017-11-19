using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Card;


/**
 * Class Deck:
 * 
 * Reproduction of a list of Card
 */
class Deck
{

    /* - Packet : List of Card */
    public List<Card.Card> Packet { get; set; }


    /**
     * Default Constructor
     */
    public Deck()
    {
        Packet = new List<Card.Card>();
    }

    /**
     * Constructor with file : string
     * 
     * Read all line from file
     * Create a card per line
     */
    public Deck(string file)
    {
        Packet = new List<Card.Card>();
        string[] lines = System.IO.File.ReadAllLines(file);

        foreach (string line in lines)
        {
            string[] parts = line.Split(' ');
            Packet.Add(new Card.Card(int.Parse(parts[0]), parts[1], parts[2]));
        }

    }

    /**
     * Return a card picked randomly
     */
    public Card.Card         PickACard()
    {
        if (Packet.Any())
        {
            Random rand = new Random();
            int randNb = rand.Next(Packet.Count());
            Card.Card tmp = new Card.Card(Packet[randNb]);
            Packet.RemoveAt(randNb);
            return (tmp);
        }
        return null;
    }
}