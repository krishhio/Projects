using System;
using System.Text;
using System.Collections.Specialized;

namespace ISS.Net
{

    public class Message
    {
        private StringDictionary _paramsMap;
        private string _type;
        private string _id;
        private string _action;

        public Message(string type, string id, string action)
        {
            _paramsMap = new StringDictionary();
            _type = type;
            _id = id;
            _action = action;
        }

        public Message(string s)
        {
            _paramsMap = new StringDictionary();
            FromString(s);
            messageString = s;
        }

        public void FromString(string s)
        {

            string[] tokens = s.Split("|".ToCharArray());
            if (tokens.Length < 4)
                return;

            _type = tokens[0];
            _id = tokens[1];
            _action = tokens[2];

            string paramList = tokens[3];
            int paramIndex = 0;
            while (paramIndex < paramList.Length)
            {
                int startIndex = paramList.IndexOf('<', paramIndex);
                int stopIndex = paramList.IndexOf('>', startIndex);
                string paramName = paramList.Substring(paramIndex, startIndex - paramIndex);
                try
                {
                    string paramValue = paramList.Substring(startIndex + 1, stopIndex - startIndex - 1);
                    _paramsMap.Add(paramName, paramValue);
                    paramIndex = stopIndex + 2;
                }
                catch
                {
                    //Console.WriteLine("Algo anda mal =(");
                    paramIndex = paramList.Length + 1;
                }

            }

            //string[] paramsTokens = tokens[3].Split(",".ToCharArray());
            //for (int i = 0; i < paramsTokens.Length; ++i)
            //{
            //    if (paramsTokens[i].Length > 1)
            //    {
            //        int startIndex = paramsTokens[i].IndexOf('<');
            //        int stopIndex = paramsTokens[i].IndexOf('>');

            //        string paramName = paramsTokens[i].Substring(0, startIndex);
            //        string paramValue =
            //            paramsTokens[i].Substring(startIndex + 1, stopIndex - startIndex - 1);
            //        _paramsMap.Add(paramName, paramValue);
            //    }
            //}

        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();

            b.Append(_type).Append('|').Append(_id).Append('|').Append(_action);

            if (_paramsMap.Values.Count > 0)
            {
                b.Append('|');

                foreach (string currKey in _paramsMap.Keys)
                {
                    b.Append(currKey).Append('<').Append(_paramsMap[currKey]).Append(">,");
                }

                b.Remove(b.Length - 1, 1);
            }

            return b.ToString();
        }
        private string messageString;

        public string GetMessageString()
        {
            return messageString;
        }

        public string GetMessageAction()
        {
            return _action;
        }

        public string GetMessageId()
        {
            return _id;
        }

        public string GetMessageType()
        {
            return _type;
        }

        public void SetParam(string key, string val)
        {
            _paramsMap[key] = val;
        }

        public string GetParam(string key)
        {
            return _paramsMap[key];
        }

        public StringDictionary GetParamsMap()
        {
            return _paramsMap;
        }
    }

}

