using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperProto
{
    public interface ValueChangeBase
    {
        void SetData(object _obj);
    }

    [Serializable]
    public class ValueChange<T> : ValueChangeBase where T : struct
    {
        public string name;
        public T data; 

        public void SetData(object _obj)
        {
            _obj.GetType().GetField(name).SetValue(_obj, data);
        }
    }

    public enum CHANGE_TYPE
    {
        ADD,
        REMOVE,
        CHANGE
    }

    public interface DicChangeBase
    {
        void SetData(object _obj);
    }

    [Serializable]
    public class DicChange<T> : DicChangeBase where T : struct
    {
        public string name;
        public CHANGE_TYPE[] type;
        public int[] index;
        public T[] data;

        public void SetData(object _obj)
        {
            Dictionary<int, T> dic = _obj.GetType().GetField(name).GetValue(_obj) as Dictionary<int, T>;

            for(int i = 0; i < index.Length; i++)
            {
                switch (type[i])
                {
                    case CHANGE_TYPE.ADD:

                        dic.Add(index[i], data[i]);

                        break;

                    case CHANGE_TYPE.CHANGE:

                        dic[index[i]] = data[i];

                        break;

                    default:

                        dic.Remove(index[i]);

                        break;
                }
            }
        }
    }

    [Serializable]
    public class SyncProto : BaseProto
    {
        public List<ValueChangeBase> valueChangeList = new List<ValueChangeBase>();
        public List<DicChangeBase> dicChangeList = new List<DicChangeBase>();

        public SyncProto()
        {
            type = PROTO_TYPE.SPC;
        }

        public void SetData(object _obj)
        {
            foreach (ValueChangeBase valueBase in valueChangeList)
            {
                valueBase.SetData(_obj);
            }

            foreach (DicChangeBase dicBase in dicChangeList)
            {
                dicBase.SetData(_obj);
            }
        }
    }
}
