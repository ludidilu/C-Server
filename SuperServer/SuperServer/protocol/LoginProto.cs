namespace SuperServer.protocol
{
    [System.Serializable]
    public class LoginProto : BaseProto
    {
        public string userName;
        public string password;
    }

    [System.Serializable]
    public class LoginResultProto : BaseProto
    {
        public bool result;
    }
}
