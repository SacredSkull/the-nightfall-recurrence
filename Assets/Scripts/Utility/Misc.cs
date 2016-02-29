using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

using UnityEngine;

using Object = System.Object;

namespace Utility
{
    public static class Logger
    {
        public enum Level
        {
            DEBUG,
            INFO,
            WARNING,
            ERROR
        }

        public static void UnityLog(string s, Level level = Level.DEBUG)
        {
            switch (level)
            {
                case Level.DEBUG:
                    s = "[DEBUG] " + s;
                    Debug.Log(s);
                    break;
                case Level.INFO:
                    s = "[INFO] " + s;
                    Debug.Log(s);
                    break;
                case Level.WARNING:
                    s = "[WARNING] " + s;
                    Debug.LogWarning(s);
                    break;
                case Level.ERROR:
                    Debug.LogError(s);
                    s = "[ERROR] " + s;
                    break;
                default:
                    s = "[DEFAULT] " + s;
                    UnityLog("I was given a log level that doesn't exist.", Level.WARNING);
                    break;
            }
        }

        public static void UnityLog(int i, Level level = Level.DEBUG)
        {
            string s = i.ToString();
            switch (level)
            {
                case Level.DEBUG:
                    s = "[DEBUG]" + s;
                    Debug.Log(s);
                    break;
                case Level.INFO:
                    s = "[INFO]" + s;
                    Debug.Log(s);
                    break;
                case Level.WARNING:
                    s = "[WARNING]" + s;
                    Debug.LogWarning(s);
                    break;
                case Level.ERROR:
                    Debug.LogError(s);
                    s = "[ERROR]" + s;
                    break;
                default:
                    s = "[DEFAULT]" + s;
                    UnityLog("I was given a log level that doesn't exist.", Level.WARNING);
                    break;
            }
        }

        public static string var_dump(Object o)
        {
            StringBuilder sb = new StringBuilder();

            // Include the type of the object
            Type type = o.GetType();
            sb.Append("Type: " + type.Name);

            // Include information for each Field
            sb.Append("\r\n\r\nFields:");
            FieldInfo[] fi = type.GetFields();
            if (fi.Length > 0)
            {
                foreach (FieldInfo f in fi)
                {
                    sb.Append("\r\n " + f + " = " +
                              f.GetValue(o));
                }
            }
            else
                sb.Append("\r\n None");

            // Include information for each Property
            sb.Append("\r\n\r\nProperties:");
            PropertyInfo[] pi = type.GetProperties();
            if (pi.Length > 0)
            {
                foreach (PropertyInfo p in pi)
                {
                    sb.Append("\r\n " + p + " = " +
                              p.GetValue(o, null));
                }
            }
            else
                sb.Append("\r\n None");

            return sb.ToString();
        }


    }
}