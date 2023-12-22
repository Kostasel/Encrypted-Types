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
        //Other Example Variables
        EncryptedInt other_player_health = 5;
        int mob_health = 5;

        //If you had a variable say:
        int player_health = 10;
        //then doing
        EncryptedInt encrypted_player_health = 10;
        //will encrypt the value inside the encrypted_player_health variable.
        //Now player_health is encrypted in memory.
        //All EncryptedTypes follow the same principle.

        //All EncryptedTypes support the bellow operations:
        /* Increment ++
         * Decrement --
         * Add(+ or +=)
         * Substract(- or -=)
         * Greater than( > ) and greater or equal than( >= )
         * Lower that ( < ) and Lower than or equal than( <= )
         * Equal == and not Equal !=
         * Can use an EncryptedType variable with another with all operations as described above.
         */

        public Example()
        {
            //Increment encrypted variable
            encrypted_player_health++;
            //Decrement encrypted variable
            encrypted_player_health--;
            //Add to encrypted variable value
            encrypted_player_health = encrypted_player_health + 12;
            encrypted_player_health += player_health;
            //Substract from encrypted variable
            encrypted_player_health = encrypted_player_health - mob_health;
            encrypted_player_health -= mob_health;
            //Greater than or greater or equal than from a standard type(or an encrypted variable)
            if (encrypted_player_health > mob_health) { };
            if (encrypted_player_health >= mob_health) { };
            //Lower that or Lower than or equal to an encrypted variable
            if (encrypted_player_health < other_player_health) { };
            if (encrypted_player_health <= other_player_health) { };
            //Equal to an encrypted variable and not Equal to an encrypted variable
            if (encrypted_player_health == 5) { };
            if (encrypted_player_health != other_player_health) { };
        }
    }
}