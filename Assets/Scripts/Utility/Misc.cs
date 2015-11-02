using System;
using System.Reflection;
using System.Text;

using UnityEngine;

using Object = System.Object;

public static class Utility {
    public static int level = LogLevels.DEBUG;

    public static void UnityLog(string s, int msgLevel = LogLevels.INFO) {
        if (level <= msgLevel) {
            switch (msgLevel) {
                case LogLevels.DEBUG:
                    s = "[DEBUG] " + s;
                    Debug.Log(s);
                    break;
                case LogLevels.INFO:
                    s = "[INFO] " + s;
                    Debug.Log(s);
                    break;
                case LogLevels.WARNING:
                    s = "[WARNING] " + s;
                    Debug.LogWarning(s);
                    break;
                case LogLevels.ERROR:
                    Debug.LogError(s);
                    s = "[ERROR] " + s;
                    break;
                default:
                    msgLevel = 1;
                    s = "[DEFAULT] " + s;
                    UnityLog("I was given a log level that doesn't exist.", LogLevels.WARNING);
                    break;
            }
        }
    }

    public static void UnityLog(int i, int msgLevel = LogLevels.DEBUG) {
        string s = i.ToString();
        if(level <= msgLevel){
            switch (msgLevel) {
                case LogLevels.DEBUG:
                    s = "[DEBUG]" + s;
                    Debug.Log(s);
                    break;
                case LogLevels.INFO:
                    s = "[INFO]" + s;
                    Debug.Log(s);
                    break;
                case LogLevels.WARNING:
                    s = "[WARNING]" + s;
                    Debug.LogWarning(s);
                    break;
                case LogLevels.ERROR:
                    Debug.LogError(s);
                    s = "[ERROR]" + s;
                    break;
                default:
                    msgLevel = 1;
                    s = "[DEFAULT]" + s;
                    UnityLog("I was given a log level that doesn't exist.", LogLevels.WARNING);
                    break;
            }
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

public static class LogLevels{
    public const int DEBUG = 1;
	public const int INFO = 2;
	public const int WARNING = 3;
	public const int ERROR = 4;
}