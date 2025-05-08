// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("hCK+mhZtgR3+vDZKJEYKH/5k9RalQu6ZKXB3LIyavk2gOza/R0Ki3UDK3BltJlcWKLJRkGTCImFC0eDbI6CuoZEjoKujI6CgoTgl4Uv1lycs7R+f3PoDlpnUareKke6P4ZOj1nAdjdsZMlRXIWV+Gv5fShUk/lMz+tC5WS+heIU4Ms3V6pLWXteH+MEmMRnDHLY/soiK9rivWFqQ6wOvhhAcyYVZaiu8t6/2f5oJhAxiJmh45mqCRfOFzv8D+Ly15ycxuPoLB0SRI6CDkaynqIsn6SdWrKCgoKShopB1y8+10fU8k1L6v1TDzRK/EYpHE1nTINoOq6gHhTzPdCtC/SssazK8IrkJdsPZbqk9qmREmtyR7B2tfDY6b8GDG6a26KOioKGg");
        private static int[] order = new int[] { 8,12,3,12,6,5,13,13,9,11,11,12,13,13,14 };
        private static int key = 161;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
