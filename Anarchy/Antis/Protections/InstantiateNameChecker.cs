namespace Antis.Protections
{
    public class InstantiateNameChecker : IProtection<string>
    {
        public bool Check(string rpcName)
        {
            return true;
        }

        bool IProtection.Check(object rpcName)
        {
            return true;
        }
    }
}