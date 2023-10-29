namespace JRRagonGames.Utilities {
    public static class BitUtilities {
        public static int GetBits(
            int intValue,
            int registerAddress,
            int mask
        ) => ShiftBits(intValue & mask, -registerAddress);

        public static int PadBits(
            int value,
            int shift
        ) => ShiftBits(value, shift);

        public static int SetBits(
            int initialValue,
            int setBits,
            int mask
        ) => initialValue & ~mask | (setBits & mask);

        public static int ToggleBits(
            int initialValue,
            int mask,
            bool value
        ) => value ? SetBits(initialValue, mask, mask) : SetBits(initialValue, 0, mask);

        public static int ShiftBits(
            int value,
            int by
        ) => by > 0 ? value << by : value >> -by;

        public static int FindLSB(int bits, int start = 0) {
            int counter = start;
            while (ShiftBits(bits, -counter) > 0 && (ShiftBits(bits, -counter) & 1) == 0) counter++;
            return counter;
        }
    }
}