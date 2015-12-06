using System.Collections;
using System.IO;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace SuperServer.csv
{
    public class StaticData
    {
        private static StaticData _Instance;

        public static StaticData Instance
        {
            get
            {
                if(_Instance == null)
                {
                    _Instance = new StaticData();
                }

                return _Instance;
            }
        }

        private string path;

        private Dictionary<Type, IDictionary> dic = new Dictionary<Type, IDictionary>();

        public void Init(string _path)
        {
            path = _path;
        }

        public T GetData<T>(int _id) where T : CsvBase
        {
            Dictionary<int, T> tmpDic = dic[typeof(T)] as Dictionary<int, T>;

            return tmpDic[_id];
        }

        public Dictionary<int, T> GetDic<T>() where T : CsvBase
        {
            Type type = typeof(T);

            return dic[type] as Dictionary<int, T>;
        }

        public void Load<T>(string _name) where T : CsvBase, new()
        {
            Type type = typeof(T);

            if (dic.ContainsKey(type))
            {
                return;
            }

            Dictionary<int, T> result = new Dictionary<int, T>();

            using (FileStream fs = new FileStream(path + "/" + _name + ".csv", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    int i = 0;

                    string lineStr = reader.ReadLine();

                    FieldInfo[] infoArr = null;

                    while (lineStr != null)
                    {
                        if (i == 2)
                        {
                            string[] dataArr = lineStr.Split(",".ToCharArray());

                            infoArr = new FieldInfo[dataArr.Length];

                            for (int m = 1; m < dataArr.Length; m++)
                            {
                                infoArr[m] = type.GetField(dataArr[m]);
                            }
                        }
                        else if (i > 2)
                        {
                            string[] dataArr = lineStr.Split(",".ToCharArray());

                            T csv = new T();

                            csv.ID = Int32.Parse(dataArr[0]);

                            for (int m = 1; m < infoArr.Length; m++)
                            {
                                FieldInfo info = infoArr[m];

                                if (info != null)
                                {
                                    SetData(info, csv, dataArr[m]);
                                }
                            }

                            csv.Fix();

                            result.Add(csv.ID, csv);
                        }

                        i++;

                        lineStr = reader.ReadLine();
                    }
                }
            }

            dic.Add(type, result);
        }

        private static void SetData(FieldInfo _info, CsvBase _csv, string _data)
        {
            try
            {
                switch (_info.FieldType.Name)
                {
                    case "Int32":

                        if (string.IsNullOrEmpty(_data))
                        {
                            _info.SetValue(_csv, 0);
                        }
                        else
                        {
                            _info.SetValue(_csv, Int32.Parse(_data));
                        }

                        break;

                    case "String":

                        _info.SetValue(_csv, _data);

                        break;

                    case "Boolean":

                        _info.SetValue(_csv, _data == "1" ? true : false);

                        break;

                    case "Single":

                        if (string.IsNullOrEmpty(_data))
                        {
                            _info.SetValue(_csv, 0);
                        }
                        else
                        {
                            _info.SetValue(_csv, float.Parse(_data));
                        }

                        break;

                    case "Int32[]":

                        int[] intResult;

                        if (!string.IsNullOrEmpty(_data))
                        {
                            string[] strArr = _data.Split("$".ToCharArray());

                            intResult = new int[strArr.Length];

                            for (int i = 0; i < strArr.Length; i++)
                            {
                                intResult[i] = Int32.Parse(strArr[i]);
                            }
                        }
                        else
                        {
                            intResult = new int[0];
                        }

                        _info.SetValue(_csv, intResult);

                        break;

                    case "String[]":

                        string[] stringResult;

                        if (!string.IsNullOrEmpty(_data))
                        {
                            stringResult = _data.Split("$".ToCharArray());
                        }
                        else
                        {
                            stringResult = new string[0];
                        }

                        _info.SetValue(_csv, stringResult);

                        break;

                    case "Boolean[]":

                        bool[] boolResult;

                        if (!string.IsNullOrEmpty(_data))
                        {
                            string[] strArr = _data.Split("$".ToCharArray());

                            boolResult = new bool[strArr.Length];

                            for (int i = 0; i < strArr.Length; i++)
                            {
                                boolResult[i] = strArr[i] == "1" ? true : false;
                            }
                        }
                        else
                        {
                            boolResult = new bool[0];
                        }

                        _info.SetValue(_csv, boolResult);

                        break;

                    default:

                        float[] floatResult;

                        if (!string.IsNullOrEmpty(_data))
                        {
                            string[] strArr = _data.Split("$".ToCharArray());

                            floatResult = new float[strArr.Length];

                            for (int i = 0; i < strArr.Length; i++)
                            {
                                floatResult[i] = float.Parse(strArr[i]);
                            }
                        }
                        else
                        {
                            floatResult = new float[0];
                        }

                        _info.SetValue(_csv, floatResult);

                        break;
                }
            }
            catch (Exception e)
            {
                string str = "SetData:" + _info.Name + "   " + _info.FieldType.Name + "   " + _data + "   " + _data.Length;

                Console.WriteLine(str);

                Console.WriteLine(e.ToString());
            }
        }
    }
}