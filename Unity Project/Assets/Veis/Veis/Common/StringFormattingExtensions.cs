using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Bots;

namespace Veis.Common
{
    /// <summary>
    /// The purpose of this class is to seperate string enconding, formatting, and decoding
    /// functionality from classes that communicate via formatted strings. 
    /// </summary>
    public class StringFormattingExtensions
    {
        public static string EncodeParameterString(IDictionary<string, string> parameters)
        {
            if (parameters.Count == 0) return String.Empty;
            // Format parameters like url parameters, split by &
            StringBuilder sb = new StringBuilder();
            foreach (var param in parameters)
            {
                sb.AppendFormat("{0}={1}&", param.Key, param.Value);
            }
            if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        public static IDictionary<string, string> DecodeParameterString(string parameterList)
        {
            var parameters = new Dictionary<string, string>();
            var splitList = parameterList.Split(new char[] {'&'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var param in splitList)
            {
                var paramValPair = param.Split('=');
                if (paramValPair.Length == 2)
                {
                    parameters.Add(paramValPair[0], paramValPair[1]);
                }
            }
            return parameters;
        }

        public static string EncodeExecutableActionString(ExecutableAction action, string executor)
        {
            // user_key|<user_key>|method_name|<method_name>|<param name>|<param value|...   
            StringBuilder sb = new StringBuilder();
            sb.Append("user_key|" + executor);
            sb.Append("|method_name|" + action.MethodName);
            foreach (var param in action.Parameters)
            {
                sb.AppendFormat("|{0}|{1}", param.Key, param.Value);
            }
            return sb.ToString();
        }
    }
}
