namespace SuperProto
{
    public enum PROTO_TYPE
    {
        C2S,
        S2C,
        SPC
    }

    [System.Serializable]
    public class BaseProto
    {
        public PROTO_TYPE type;
    }
}
