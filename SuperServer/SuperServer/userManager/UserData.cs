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

        private List<IBaseData> valueList = new List<IBaseData>();
        private List<IBaseData> dicList = new List<IBaseData>();

        public UserData()
        {
            Type type = GetType();

            FieldInfo[] fieldInfos = type.GetFields();

            foreach (FieldInfo info in fieldInfos)
            {
                IBaseData unit = info.GetValue(this) as IBaseData;

                if (unit != null) {

                    unit.SetName(info.Name);

                    if (unit is System.Collections.IEnumerable)
                    {
                        dicList.Add(unit);
                    }
                    else
                    {
                        valueList.Add(unit);
                    }
                }
            }
        }

        internal SyncProto GetChangeData()
        {
            SyncProto proto = new SyncProto();

            foreach (IBaseData unit in valueList)
            {
                if (unit.IsChange())
                {
                    proto.valueChangeList.Add(unit.GetChangeData());
                }
            }

            foreach (IBaseData unit in dicList)
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

            foreach (IBaseData unit in valueList)
            {
                proto.valueList.Add(unit.GetData());
            }

            foreach (IBaseData unit in dicList)
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
