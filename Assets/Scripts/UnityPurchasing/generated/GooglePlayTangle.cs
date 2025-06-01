// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("mG9RbjMZzTdU1HEz3VTkFZnYrSSShCy52JpHHzcMlyaiQyroKIJV4pyOoL/PTu2hTNmt2R//r+A6OyxS1N2zBX8pKGctknEMEFUFRenX2B78jPqGBm+Od6OwhuDnpxvCemg2QrwOja68gYqFpgrECnuBjY2NiYyPbzfEcJXppc24II4Ce2s4BfLZlbwxPWFSLDiClJUNwt1IPQZJNvEn6RZhDnFIx2HO18CzYH+GcxHKnRX0Do2DjLwOjYaODo2NjDdILKK6URRaZHMuS24TAtF8bmTgOGNl+F04acZjGWUBQdU7BM9BS82MqaPWa9oArKduQQsyvlKs3A4rkHTxivagdVIpOp3L+nY2LMGOHFLRYwC96qUyUTuPUDSBdVexZY6PjYyN");
        private static int[] order = new int[] { 12,8,10,13,11,12,9,12,9,9,13,13,12,13,14 };
        private static int key = 140;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
