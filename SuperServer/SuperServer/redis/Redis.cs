using ServiceStack.Redis;
using System;
using SuperServer.superService;
using SuperServer.userManager;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SuperServer.server;

namespace SuperServer.redis
{
    public class Redis : SuperService
    {
        private static Redis _Instance;

        public static Redis Instance
        {
            get
            {
                if(_Instance == null)
                {
                    _Instance = new Redis();
                }

                return _Instance;
            }
        }

        private RedisClient redisClient;

        private MemoryStream stream = new MemoryStream();

        private BinaryFormatter formatter = new BinaryFormatter();

        public void Start(string _host, int _port)
        {
            redisClient = new RedisClient(_host, _port);
        }

        internal void Login<T,U>(string _userName,string _password,ServerUnit<T,U> _serverUnit) where T : SuperUserService<U>, new() where U : UserData, new()
        {
            string key = string.Format("userName_{0}", _userName);
            
            if (redisClient.ContainsKey(key))
            {
                string password = redisClient.Get<string>(string.Format("password_{0}", _userName));

                if(password.Equals(_password))
                {
                    byte[] bytes = redisClient.Get<byte[]>(key);

                    stream.Position = 0;

                    stream.Write(bytes, 0, bytes.Length);

                    stream.Position = 0;

                    U userData = (U)formatter.Deserialize(stream);

                    Action<UserManager<T,U>> callBack = delegate (UserManager<T, U> _service)
                    {
                        _service.GetLoginResult(userData, _serverUnit);
                    };

                    UserManager<T, U>.Instance.Process(callBack);
                }
                else
                {
                    Action<UserManager<T, U>> callBack = delegate (UserManager<T, U> _service)
                    {
                        _service.GetLoginResult(null, _serverUnit);
                    };

                    UserManager<T, U>.Instance.Process(callBack);
                }
            }
            else
            {
                U userData = new U();

                userData.Init(_userName, _password);

                formatter.Serialize(stream, userData);

                byte[] bytes = stream.GetBuffer();
                
                redisClient.Set(key, bytes);

                key = string.Format("password_{0}", _userName);

                redisClient.Set(key, _password);

                Action<UserManager<T, U>> callBack = delegate (UserManager<T, U> _service)
                {
                    _service.GetLoginResult(userData, _serverUnit);
                };

                UserManager<T, U>.Instance.Process(callBack);
            }
        }
    }
}
