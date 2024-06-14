using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPEncryptedChat
{
    internal class Crypter
    {
        //Initialisiert das Alphabet Grid
        char[] AlphabetNew()
        {
            //Datengrid wird definiert und in ein Char Array geladen um damit Arbeiten zu können
            String abc = "ABCDEFGHIJKLMNOPQRSTUVWXYZzyxwvutsrqponmlkjihgfedcba %?!+ç&/=-*:;<>$£.,1234567890äöüÄÖÜéèà[]{}()'^`~";
            char[] alpha = abc.ToCharArray();
            return alpha;
        }

        //Schlüssel Encoding
        public string encodeKey(String input, String key)
        {
            //Werte werden geladen
            char[] alpha = AlphabetNew();

            //Input wird als Array Convertiert um die verschlüsselung zu berechnen
            char[] toEncode = input.ToCharArray();
            char[] secretKey = key.ToCharArray();
            char[] encrypted = null;

            //secretKey in Nummern abspeichern
            int[] keyCode = null;
            for (int i = 0; i < secretKey.Length; i++)
            {
                for (int d = 0; d < alpha.Length; d++)
                {
                    if (secretKey[i] == alpha[d])
                    {
                        Array.Resize(ref keyCode, i + 1);
                        keyCode[i] = d;
                    }
                }
            }

            //Key Verschlüsselung
            int count = 0;
            for (int i = 0; i < toEncode.Length; i++)
            {
                for (int d = 0; d < alpha.Length; d++)
                {
                    if (toEncode[i] == alpha[d])
                    {
                        int code = d;
                        for (int k = 0; k < keyCode[count]; k++)
                        {
                            code++;
                            if (code > alpha.Length - 1)
                            {
                                code = 0;
                            }
                        }
                        Array.Resize(ref encrypted, i + 1);
                        encrypted[i] = alpha[code];
                        count++;
                        if (count > keyCode.Length - 1)
                        {
                            count = 0;
                        }
                    }
                }
            }
            String encrypt = new string(encrypted);
            return encrypt;
        }

        //Key Decode Methode
        public string decodeKey(String input, String key)
        {
            //Alphabettabelle wird geladen
            char[] alpha = AlphabetNew();

            char[] encrypt = input.ToCharArray();
            char[] secretKey = key.ToCharArray();

            //secretKey in Nummern abspeichern
            int[] keyCode = null;
            for (int i = 0; i < secretKey.Length; i++)
            {
                for (int d = 0; d < alpha.Length; d++)
                {
                    if (secretKey[i] == alpha[d])
                    {
                        Array.Resize(ref keyCode, i + 1);
                        keyCode[i] = d;
                    }
                }
            }

            char[] decrypted = null;

            //Key entschlüsselung
            int count = 0;
            for (int i = 0; i < encrypt.Length; i++)
            {
                for (int d = 0; d < alpha.Length; d++)
                {
                    if (encrypt[i] == alpha[d])
                    {
                        Array.Resize(ref decrypted, i + 1);
                        int code = d;
                        //key abzählen
                        for (int k = 0; k < keyCode[count]; k++)
                        {
                            code--;
                            if (code < 0)
                            {
                                code = alpha.Length - 1;
                            }
                        }
                        decrypted[i] = alpha[code];
                        count++;
                        if (count > keyCode.Length - 1)
                        {
                            count = 0;
                        }
                    }
                }
            }
            String decode = new String(decrypted);
            return decode;
        }
    }
}
