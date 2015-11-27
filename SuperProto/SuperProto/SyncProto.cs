using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperProto
{
    
    [Serializable]
    public class ValueChange<T> where T : struct
    {
        public string name;
        public T data; 
    }

    [Serializable]
    public class DicChange<T> where T : struct
    {
        public string name;
        public int[] type;
        public int[] index;
        public T[] data;
    }

    [Serializable]
    public class SyncProto : BaseProto
    {
        public List<object> valueChangeList = new List<object>();
        public List<object> dicChangeList = new List<object>();

        public SyncProto()
        {
            type = PROTO_TYPE.SPC;
        }
    }
}
