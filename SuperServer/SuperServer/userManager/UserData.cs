using System.Collections.Generic;
using System.Reflection;
using System;
using SuperProto;

namespace SuperServer.userManager
{
    [System.Serializable]
    public class UserData
    {
        public string userName;
        internal string passward;

        private List<IValueData> valueList = new List<IValueData>();
        private List<IDicValueData> dicList = new List<IDicValueData>();

        public UserData()
        {
            Type type = GetType();

            FieldInfo[] fieldInfos = type.GetFields();

            foreach (FieldInfo info in fieldInfos)
            {
                object unit = info.GetValue(this);

                if(unit is IValueData)
                {
                    IValueData valueData = unit as IValueData;

                    valueData.SetName(info.Name);

                    valueList.Add(valueData);
                }
                else if(unit is IDicValueData)
                {
                    IDicValueData dicData = unit as IDicValueData;

                    dicData.SetName(info.Name);

                    dicList.Add(dicData);
                }
            }
        }

        internal SyncProto GetChangeData()
        {
            SyncProto proto = new SyncProto();

            foreach (IValueData unit in valueList)
            {
                if (unit.IsChange())
                {
                    proto.valueChangeList.Add(unit.GetChangeData());
                }
            }

            foreach (IDicValueData unit in dicList)
            {
                if (unit.IsChange())
                {
                    proto.dicChangeList.Add(unit.GetChangeData());
                }
            }

            return proto;
        }

        internal UserDataResultProto GetAllData()
        {
            UserDataResultProto proto = new UserDataResultProto();

            foreach (IValueData unit in valueList)
            {
                proto.valueList.Add(unit.GetData());
            }

            foreach (IDicValueData unit in dicList)
            {
                proto.dicList.Add(unit.GetData());
            }

            return proto;
        }

        internal void Init(string _userName,string _password)
        {
            userName = _userName;
            passward = _password;
        }
    }
}
