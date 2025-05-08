using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class ObjectParse 
{
    // Start is called before the first frame update
   public static string getString(JObject data,string propertyName) 
    {
        return (string)data[propertyName];
    }
    public static int getInt(JObject data, string propertyName)
    {
        return (int)data[propertyName];
    }
    public static long getLong(JObject data, string propertyName)
    {
        return (long)data[propertyName];
    }
    public static float getFloat(JObject data, string propertyName)
    {
        return (float)data[propertyName];
    }
    public static bool getBool(JObject data, string propertyName)
    {
        return (bool)data[propertyName];
    }
    public static List<int> getListInt(JObject data, string propertyName)
    {
        JArray arr = (JArray)data[propertyName];
        return arr.ToObject<List<int>>();
    }
    public static JArray getJArray(JObject data, string propertyName)
    {
        JArray arr = (JArray)data[propertyName];
        return arr;
    }
    public static List<JObject> getListJObject(JObject data, string propertyName)
    {

        List<JObject> arr = (getJArray(data,propertyName)).ToObject<List<JObject>>();
        return arr;
    }
}
