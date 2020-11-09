namespace Unicraft.Core
{
    [System.Serializable]
    public struct RawBlock
    {
        public int type;
        public float value;

        public override string ToString()
        {
            return $"Type: {type}, Value: {value}";
        }
    }
}