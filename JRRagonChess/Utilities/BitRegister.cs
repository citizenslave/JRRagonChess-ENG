namespace JRRagonGames.Utilities {
    public class BitRegister {
        private int bitData;

        public BitRegister(int _bitData, uint size = 32) { bitData = _bitData; }

        public int this[int mask] {
            get => mask < 0 ? bitData : (bitData & mask) >> BitUtilities.FindLSB(mask);
            set => bitData = mask < 0 ? bitData : (bitData & ~mask) | ((value << BitUtilities.FindLSB(mask)) & mask);
        }

        public int BitData { get => bitData; set => bitData = value; }
    }
}
