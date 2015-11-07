namespace SuperServer.userManager
{
    [System.Serializable]
    public class UserData
    {
        internal string userName;
        internal string passward;

        internal void Init(string _userName,string _password)
        {
            userName = _userName;
            passward = _password;
        }
    }
}
