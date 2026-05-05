namespace Vymesy.Gems
{
    [System.Serializable]
    public class GemSlot
    {
        public GemData Gem;
        public int Level = 1;

        public bool IsEmpty => Gem == null;
    }
}
