using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Card
{

    /**
     * Class Card :
     * 
     * Reproduction of a card
     */
    class Card
    {
        /* - Val : int value for comparaison */
        public int Val { get; set; }
        /* - Name : Ace, King, Queen, Jack... */
        public string Name { get; set; }
        /* - Color : Hearth, Spades, Tiles, Clovers */
        public string Color { get; set; }


        /**
         * Constructor with int : val, string : name, string : color
         */
        public Card(int val, string name, string color)
        {
            Val = val;
            Name = name;
            Color = color;
        }

        /**
         * Default Constructor
         */
        public Card()
        {

        }

        /**
         * Copy Constructor
         */
        public Card(Card newCard)
        {
            Val = newCard.Val;
            Name = newCard.Name;
            Color = newCard.Color;
        }

    }
}