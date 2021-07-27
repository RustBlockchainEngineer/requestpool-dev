using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Vodafone
{
    public class HMACSHA
    {
        public string ConvertStringToHash(string InputString, string secreatKey)
        {
       
            // hex decode value of the secret key.
            var keybyte = HexDecode(secreatKey);

            // Encode secreat key
            var keybytes = Encoding.UTF8.GetBytes(secreatKey);

            // Initialize the keyed hash object using the secret key as the key
            HMACSHA256 hashObject = new HMACSHA256(keybytes);

            // Encode input string to UTF-8
            var messagebytes = Encoding.UTF8.GetBytes(InputString);

            // Computes the signature by hashing the salt with the secret key as the key
            var signature = hashObject.ComputeHash(messagebytes);

            // Base 64 Encode
            var encodedSignature = Convert.ToBase64String(signature);
          
            // URLEncode
            return BitConverter.ToString(signature).Replace("-", "").ToUpper(); 

        }
        private byte[] HexDecode(string hex)
        {
            var bytes = new byte[hex.Length / 2];
            for (long i = 0; i < bytes.Length; i++)
            {
                // return result of converting decimal to hex
                bytes[i] = byte.Parse(hex.Substring(i * 2, 2), NumberStyles.HexNumber);
            }
            return bytes;
        }
    

    }
}