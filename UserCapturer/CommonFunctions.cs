using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace UserCapturer
{
    public class CommonFunctions
    {

        // Custom logic to get the next auto-increment value
        public static int GetNextId(XDocument xmlDoc)
        {
            // Find the maximum "id" value in the existing records (if any)
            int maxId = xmlDoc.Descendants("Id")
                .Select(e => (int)e)
                .DefaultIfEmpty(0)
                .Max();

            // Increment the maximum value to get the next auto-incremented ID
            return maxId + 1;
        }

        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            // Define a regular expression pattern for a South African cellphone number
            string pattern = @"^(\+27|27|0)[0-9]{2}( |-)?[0-9]{3}( |-)?[0-9]{4}( |-)?(x[0-9]+)?(ext[0-9]+)?";

            // Create a regex object and match the phone number against the pattern
            Regex regex = new Regex(pattern);
            Match match = regex.Match(phoneNumber);

            // Check if the match was successful and covers the entire input string
            return match.Success && match.Length == phoneNumber.Length;
        }

    }
}
