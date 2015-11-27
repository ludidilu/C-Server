using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperProto
{
    [Serializable]
    public class UserDataProto : BaseProto
    {
        public UserDataProto()
        {
            type = PROTO_TYPE.C2S;
        }
    }

    [Serializable]
    public class Value<T> where T : struct
    {
        public string name;
        public T data;
    }

    [Serializable]
    public class Dic<T> where T : struct
    {
        public string name;
        public int[] index;
        public T[] data;
    }

    [Serializable]
    public class UserDataResultProto : BaseProto
    {
        public List<object> valueList = new List<object>();
        public List<object> dicList = new List<object>();

        public UserDataResultProto()
        {
            type = PROTO_TYPE.S2C;
        }
    }
}
