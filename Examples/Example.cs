//Copyright 2023 Kostasel
//See license.txt for license details

//To start using the EncryptedTypes
//1. Add the using statement bellow to your code.
//Using EncryptedTypes;
using UnityEngine;

namespace EncryptedTypes.Examples
{
    //2.All you have to do is replace the variable type
    //holding the data you want to protect with an 
    //encrypted type and assign the data to
    //the encrypted type.
    public class Example : MonoBehaviour
    {
        //If you had a variable say
        int health = 10;
        //then doing
        EncryptedInt encrypted_health = 10;
        //and this is it.
        //Now health is encrypted in memory inside the encrypted_health variable.
        //All EncryptedTypes follow the same principle.  
        //All EncryptedTypes support the bellow operations:
        public Example(EncryptedInt encrypted_health)
        {
            //Increment ++
            encrypted_health++;
            //Decrement --
            encrypted_health--;
            //Add(+ or +=)
            encrypted_health = encrypted_health + health;
            encrypted_health += health;
            //Substract(- or -=)
            encrypted_health = encrypted_health - health;
            encrypted_health -= health;
            //Greater than > and greater or equal >=
            if (encrypted_health > health) { };
            if (encrypted_health >= health) { };
            //Can also compare with another NetworkInt variable
            if (encrypted_health >= healh_current) { };
            //Lower that < and Lower than or equal <=
            if (encrypted_health < health) { };
            if (encrypted_health <= health) { };
            //Can also compare with another NetworkInt variable
            if (encrypted_health <= healh_current) { };
            //Equals == and not Equals !=
            if (encrypted_health == health) { };
            if (encrypted_health != health) { };
        }
    }
}