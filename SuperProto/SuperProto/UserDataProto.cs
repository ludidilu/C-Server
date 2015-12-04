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

    public interface ValueBase
    {
        void SetData(object _obj);
    }

    [Serializable]
    public class Value<T> : ValueBase where T : struct
    {
        public string name;
        public T data;

        public void SetData(object _obj)
        {
            _obj.GetType().GetField(name).SetValue(_obj, data);
        }
    }

    public interface DicBase
    {
        void SetData(object _obj);
    }


    [Serializable]
    public class Dic<T> : DicBase where T : struct
    {
        public string name;
        public int[] index;
        public T[] data;

        public void SetData(object _obj)
        {
            Dictionary<int, T> dic = new Dictionary<int, T>();

            for(int i = 0; i < index.Length; i++)
            {
                dic.Add(index[i], data[i]);
            }

            _obj.GetType().GetField(name).SetValue(_obj, dic);
        }
    }

    [Serializable]
    public class UserDataResultProto : BaseProto
    {
        public List<ValueBase> valueList = new List<ValueBase>();
        public List<DicBase> dicList = new List<DicBase>();

        public UserDataResultProto()
        {
            type = PROTO_TYPE.S2C;
        }

        public void SetData(object _obj)
        {
            for(int i = 0; i < valueList.Count; i++)
            {
                ValueBase valueBase = valueList[i];
           
                valueBase.SetData(_obj);
            }

            for(int i = 0; i < dicList.Count; i++)
            {
                DicBase dicBase = dicList[i];
           
                dicBase.SetData(_obj);
            }
        }
    }
}
