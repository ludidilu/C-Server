using System;

namespace SuperProto
{
    [Serializable]
    public class LoginProto : BaseProto
    {
        public string userName;
        public string password;
        public LoginProto()
        {
            type = PROTO_TYPE.C2S;
        }
    }

    [Serializable]
    public class LoginResultProto : BaseProto
    {
        public bool result;

        public LoginResultProto()
        {
            type = PROTO_TYPE.S2C;
        }
    }
}
