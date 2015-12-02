using System;
using System.Runtime.Serialization;
using SuperProto;

namespace SuperServer.userManager
{
    [Serializable]
    public class ValueData<T> : ISerializable,IValueData where T : struct
    {
        private string name;

        private T m_data;

        private bool isChange;

        public ValueData()
        {

        }

        protected ValueData(SerializationInfo info, StreamingContext context)
        {
            m_data = (T)info.GetValue("m_data", typeof(T));

            name = (string)info.GetValue("name", typeof(string));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("m_data", m_data, typeof(T));

            info.AddValue("name", name, typeof(string));
        }

        public T data
        {
            get
            {
                return m_data;
            }

            set
            {
                isChange = true;

                m_data = value;
            }
        }

        public bool IsChange()
        {
            return isChange;
        }

        public void SetName(string _name)
        {
            name = _name;
        }

        public string GetName()
        {
            return name;
        }

        public ValueBase GetData()
        {
            Value<T> tmpData = new Value<T>();

            tmpData.name = name;

            tmpData.data = data;

            if (isChange)
            {
                isChange = false;
            }

            return tmpData;
        }

        public ValueChangeBase GetChangeData()
        {
            ValueChange<T> changeData = new ValueChange<T>();

            changeData.name = name;

            changeData.data = data;

            isChange = false;

            return changeData;
        }
    }
}
