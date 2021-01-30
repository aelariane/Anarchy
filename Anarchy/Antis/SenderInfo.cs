namespace Antis
{
    public sealed class SenderInfo
    {
        public readonly string Name;
        public readonly int OwnerID;
        private int[] targetArray;

        public SenderInfo(int id) : this(id, "Unknown")
        {
        }

        public SenderInfo(int id, string name)
        {
            Name = name;
            OwnerID = id;
            targetArray = new int[] { id };
        }

        public override string ToString()
        {
            return $"SenderInfo(ID: {OwnerID.ToString()}, Name: {Name})";
        }

        public int[] ToTargetArray()
        {
            return targetArray;
        }
    }
}