using System;

namespace SuperProto
{
    [Serializable]
    public class LoginProto : BaseProto
    {
        public string userName;
        public string password;
    }

    [Serializable]
    public class LoginResultProto : BaseProto
    {
        public bool result;
    }
}
