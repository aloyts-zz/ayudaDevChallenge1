using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ayudaDevChallenge1
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             * A buddy sent me this: http://www.ayudasystems.com/Jobs.
             * Now I haven't looked for jobs in a dozen years and don't know if this kind of thing is routine
             * when quizzing one's developers. I always just told them to write me a function with some purpose.
             * This, however, was awesome. 
             * 
             * I have no background in cryptography, nor do I spend 17 hours of my day staring at code any more
             * and coding is like any sport: if you want to do it well, do it often. Best workout in months though...
             * A little sore.
             * 
             * I'm sure there is an elegant solution to all this and had I even know what this whole "ass-key" thing
             * was before jumping in, I may have realized that the cypher flips the first byte between lower-and upper-case letters
             * as defined in the standard. Below is the original mapping from the handy encryption tester they provide.
             *            
             * 6B 68 69 6E 6F 6C 6D 62 63 60 61 66 67 64 65 7A 7B 78 79 7E 7F 7C 7D 72 73 70
             * A  B  C  D  E  F  G  H  I  J  K  L  M  N  O  P  Q  R  S  T  U  V  W  X  Y  Z 
             * a  b  c  d  e  f  g  h  i  j  k  l  m  n  o  p  q  r  s  t  u  v  w  x  y  z
             * 4B 48 49 4E 4F 4C 4D 42 43 40 41 46 47 44 45 5A 5B 58 59 5E 5F 5C 5D 52 53 50
             * 
             * I'm sure the second byte similarly follows a pattern and would fall quite quickly if it were fed into an algorithm.
             * However, I decided that instead of being elegant, I'd brute force the thing. I could stare at the patterns later. 
             * If I ever figure out the pattern, all that follows is a waste. I think it was Drucker who wrote:
             * "There is nothing so useless as doing well that which should not be done at all."
             * 
             * First, I needed to get a mapping of characters and relate them to their encrypted version.  I fed in the alphabet
             * and got my cypher string. The delimiter is useful to quickly create arrays with the results.
             * 
             * Being Québécois, I should have known that Ayuda would not have left it there but used accents and 
             * other tricks to screw me. Quebec turned out to be correlation and not causation as I later discovered. I now blame for this trickery
             * the philologist who came up with the phrases to begin with. Like we have nothing better to do than learn fake languages. 
             * 
             * Since Ayuda is a .NET shop, I decided to use C# which affords luxuries like collections but I wanted to make this as portable
             * as possible to the most primitive of languages. This wish had limits since there was no way in hades I was writing my own
             * split or replace functions. Gave me nightmares of C days. Even if I had the C muscles to flex, I wouldn't show them off. 
             * No one cares about those muscles. 
             * 
             */


            string strKeyEng = "a,b,c,d,e,ë,f,g,h,i,í,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,1,2,3,4,5,6,7,8,9,0, ,!,@,#,$,%,^,&,*,(,),-,_,+,=,.,/,?,<,>,[,],{,},|,'";
            string strKeyEnc = "4B06480649064E064F06C1064C064D0642064306C7064006410646064706440645065A065B06580659065E065F065C065D065206530650066B06680669066E066F066C066D06620663066006610666066706640665067A067B06780679067E067F067C067D067206730670061B06180619061E061F061C061D06120613061A060A060B066A0609060E060F0674060C06000602060306070675060106170604060506150616061406710677065106570656060D";
            string strMsg = "6B0A6F46484F584F5E420A6D43465E42454443C1462720594346435C584F440A5A4F44444B0A47C75843C1462720450A474F444F460A4B4D464B580A4F464F444B5E420B2720644B0749424B4F584F4E0A5A4B464B44074EC75843C1462720450A4D4B464B4E42584F474743440A4F444445584B5E420627206C4B445F43464559060A464F0A464344444B5E4245442720444F4C0A4B4F4B58060A59C70A444F4C0A4B4F4B5845440B";

            strMsg = strMsg.Replace("2720", "XX");
            /* 
             * A little cheating. Clean up the only 4-byte "entity" in the string. Something about the line feeds 
             * was screwing me and my desire to figure it out was exceeded by my lack of patience. 
             * It'll be hard enought to remember to replace this with \r\n 
             * 
             * I went through the cypher and X was never used so I thought it safe. 
             * Typically, when looking for good delimiters for array creation,
             * I'll use something like ALT-0169 which is: ©. 
             *      
             */

            int intStrMsgChars = strMsg.Length / 2; //gives the number of unique char entities in the encrypted string since each [1 decrypted char]=[2 encrypted char]   

            string[] arStrEncMsg = new string[intStrMsgChars]; //init array to fit encrypted char entity
            string[] arStrDecMsg = new string[intStrMsgChars]; //init array to fit decrypted char entity.

            string[] arStrKeyEng = strKeyEng.Split(','); // create array of decrypted results
            string[] arStrKeyEnc = strKeyEnc.Replace("06", ",").Split(','); //create comma-delimited version, then create array out of string.

            string strDKey = "";
            string strSQL = "";

            int encArrLoop = 0; //use to look through encrypted array
            int decArrLoop = 0; //use to loop through decrypted array

            // create a decryption key.
            for (decArrLoop = 0; decArrLoop < arStrKeyEnc.Length - 1; decArrLoop++)
            {
                for (encArrLoop = 0; encArrLoop < arStrKeyEng.Length - 1; encArrLoop++)
                {
                    if (arStrKeyEng[decArrLoop] == arStrKeyEng[encArrLoop])
                    {
                        arStrDecMsg[decArrLoop] = arStrKeyEng[encArrLoop]; // create equality array
                        arStrEncMsg[decArrLoop] = arStrKeyEnc[decArrLoop];

                        strDKey += arStrKeyEng[encArrLoop] + "=" + arStrKeyEnc[decArrLoop] + ","; // create readable string with the contents of the array
                        strSQL += "insert into tblKey (key_Enc, key_Eng) select '" + arStrKeyEnc[decArrLoop] + "','" + arStrKeyEng[encArrLoop] + "'\r\n";
                        /* 
                         * I'm already looping through so what's the harm? 
                         * Perhaps some SQL insert Statements to try to solve this in a database?
                         * SQL has substring and replace functions too and but to do this right
                         * I'd have to use a cursor and I have a profound hatred for cursors. 
                         */

                    }
                }
                /*
                 * strMsg = strMsg.Replace(arStrKeyEnc[i], arStrKeyEng[i]); 
                 * 
                 * Originally, I thought that the above replace function would be sufficient. Done in minutes! 
                 * 
                 * Idiot.
                 *  
                 * This creates a "race condition" where entity position can cause an incorrect 
                 * replacement based on seed order. Take the case of "...6460..." 
                 * The entities might be 64 and 60 but searching for 
                 * 46 meets the condition and causes an incorrect decryption at best, gibberish at worst.
                 * Sadly, running the cypher through replace  
                 * yields enough content to do a search and get the answer for the laziest among us. 
                 * It's how I confirmed I'd need to use accented characters in my key.
                 * But it's not a solution and is inelegant to say nothing of wholesale cheating.
                 * Besides, Turing didn't have Google when he was cracking codes. 
                 * 
                 */
            }
            // end key creation loop

            /*
             * now, since I can't get away with "replace" from within the original loop, I need to go through the message 2-characters (1 entity) at a time,
             * look up the encrypted entity in the arStrEncMsg array and put in its decrypted equivalent from the arStrDecMsg array.
             */

            int intMsgLen = strMsg.Length;
            int cyLoop = 0; //cypher looper
            int cyLookup = 0; //cypher lookup looper
            //lookup and replace encrypted entities with their decrypted equivalents.

            string strDecrypted = ""; // results containter for the loop below.

            for (cyLoop = 0; cyLoop < intMsgLen; cyLoop += 2)
            {
                //if the entity is not my fake newline... 
                if (strMsg.Substring(cyLoop, 2) == "XX")
                {
                    strDecrypted += "XX"; //keep the newline in for now...
                }
                else if (strMsg.Substring(cyLoop, 2) == "06")
                {
                    strDecrypted += ","; //how about our delimiter? easier to do this instead of re-initializing the lookup array with +1 values.
                }
                else
                {
                    //lookup encrypted entity
                    for (cyLookup = 0; cyLookup < arStrKeyEnc.Length - 1; cyLookup++)
                    {
                        /*                      
                         * this method of comparison works too here.
                         * if (strMsg.Substring(cyLoop, 2) == arStrKeyEnc[cyLookup]) 
                         *  
                         */

                        if (strMsg.Substring(cyLoop, 2).Equals(arStrKeyEnc[cyLookup], StringComparison.Ordinal))
                        {
                            strDecrypted += arStrKeyEng[cyLookup];
                            break;
                        }
                    }
                    //end lookup
                }
            }

            //end lookup/replace loop

            // clean up the newline XXs and we're done.
            strDecrypted = strDecrypted.Replace("XX", "\r\n");

            //Console.WriteLine(strDKey + "\r\n"); \\write out the key if I want to. 
            //System.IO.File.WriteAllText(@"f:\decryptedString.txt", strDecrypted);
            Console.WriteLine(strDecrypted);

        }

        }
    }

