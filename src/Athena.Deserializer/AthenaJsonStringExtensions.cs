namespace Athena.Deserializer;

using System;
using System.Text.RegularExpressions;

public static class AthenaJsonStringExtensions
{

    public static T ToJsonObject<T>(this string jsonInput)
    {
        var jsonString = SanitizeString(jsonInput);

        var serialized = System.Text.Json.JsonSerializer.Serialize(jsonString.ToJsonObject());
        return System.Text.Json.JsonSerializer.Deserialize<T>(serialized, new System.Text.Json.JsonSerializerOptions
        {
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
            PropertyNameCaseInsensitive = true
        });
    }
    public static string ToSerializedJson(this string jsonInput)
    {
        var jsonString = SanitizeString(jsonInput);
        return System.Text.Json.JsonSerializer.Serialize(jsonString.ToJsonObject());
    }
    public static object ToJsonObject(this string jsonInput)
    {
        var jsonString = Regex.Replace(jsonInput, @"\t|\n|\r", "").Trim();
        if (jsonString.Trim().StartsWith('{'))
        {
            return GetObject(jsonString.Trim());
        }
        else if (jsonString.Trim().StartsWith('['))
        {
            return GetArray(jsonString.Trim());
        }
        else
        {
            return String.Empty;
        }
    }

    private static String SanitizeString(string jsonInput)
    {
        return Regex.Replace(jsonInput, @"\t|\n|\r", "").Trim();
    }
    private static Dictionary<string, object> GetObject(string jsonString)
    {
        var jsonInput =SanitizeString(jsonString);
        var result = new Dictionary<string, object>();

        var objTree = new Stack<int>();
        var objValue = String.Empty;
        var objKey = String.Empty;

        var lvl = 0;
        var objLvl = 0;

        var jsonValue = String.Empty;

        //foreach (var chr in jsonInput)
        for(int i=0;i < jsonInput.Length; i++)
        {
            var chr = jsonInput[i];
            if (char.Equals('{', chr))
            {
                lvl++;
                objTree.Push(lvl);
                if (lvl == 1) continue;
            }

            if (lvl >= 1)
            {
                if (char.Equals('{', chr) || char.Equals('[', chr) || char.Equals('(', chr)) objLvl++;
                if (char.Equals('}', chr) || char.Equals(']', chr) || char.Equals(')', chr)) objLvl--;

                if (objLvl == 0 && char.Equals(',', chr) || char.Equals('}', chr))
                {
                    if (!String.IsNullOrEmpty(objKey))
                    {
                        var key = objKey.Trim('\'', '"', ' ');
                        if (objValue.Trim().StartsWith('['))
                        {                            

                            var value = GetArray(objValue);
                            result.Add(key, value);
                        }
                        else if (objValue.Trim().StartsWith('{'))
                        {
                            for (int j = i; j < jsonInput.Length; j++)
                            {
                                chr = jsonInput[j];
                                objValue += chr;
                                if (char.Equals('}', chr))
                                {
                                    i = j;
                                    break;
                                }
                            }
                            var value = GetObject(objValue);
                            result.Add(key, value);
                        }
                        else
                        {
                            result.Add(key, objValue.Trim('\'', '"', ' '));
                        }

                    }
                    objValue = String.Empty;
                    objKey = String.Empty;
                    objLvl = 0;
                }
                else if (objLvl == 0 && char.Equals('=', chr) && String.IsNullOrEmpty(objKey))
                {
                    objKey = objValue;
                    objValue = String.Empty;
                }
                else
                {
                    objValue += chr;
                }
            }

            if (char.Equals('}', chr)) 
            {
                var endLevel = objTree.Pop();
                if (!objTree.TryPeek(out lvl)) lvl = 0;
            }

            if (lvl == 0)
            {
                continue;
            }

            jsonValue += chr;
        }

        return result;
    }

    private static List<object> GetArray(string jsonString)
    {
        var jsonInput = SanitizeString(jsonString);
        var result = new List<object>();

        var objTree = new Stack<int>();
        var objValue = String.Empty;

        var lvl = 0;
        var objLvl = 0;


        for (int i=0;i< jsonInput.Length; i++)
        {
            var chr = jsonInput[i];
            if (char.Equals('[', chr)) // || char.Equals('{', chr))
            {
                lvl++;
                objTree.Push(lvl);
                if (lvl == 1)
                {
                    continue;
                }
            }

            if (lvl >= 1)
            {

                if (char.Equals('{', chr) || char.Equals('[', chr) || char.Equals('(', chr)) objLvl++;
                if (char.Equals('}', chr) || char.Equals(']', chr) || char.Equals(')', chr)) objLvl--;

                if (objLvl <= 0 && (char.Equals(',', chr) || char.Equals(']', chr)))
                {
                    if (objValue.Trim().StartsWith('{'))
                    {
                        var val = GetObject(objValue);
                        result.Add(val);
                    }
                    else
                    {
                        result.Add(objValue.Trim('\'', '"', ' '));
                    }
                    objValue = String.Empty;
                    objLvl = 0;
                }
                else
                {
                    objValue += chr;
                }
            }

            if (char.Equals(']', chr)) // || char.Equals('}', chr))
            {
                var endLevel = objTree.Pop();
                if (!objTree.TryPeek(out lvl)) lvl = 0;
            }

            if (lvl == 0)
            {
                continue;
            }

        }
        return result;
    }
}
