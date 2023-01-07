namespace Athena.Serializer;

using System;

    public static class AthenaJsonStringExtensions
    {
        private static Dictionary<string, object> GetObject(string jsonInput)
        {
            var result = new Dictionary<string, object>();

            var objTree = new Stack<int>();
            var objValue = String.Empty;
            var objKey = String.Empty;

            var lvl = 0;
            var objLvl = 0;

            var jsonValue = String.Empty;

            foreach (var chr in jsonInput)
            {
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

                if (char.Equals('}', chr)) // || char.Equals('}', chr))
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

        private static List<object> GetArray(string jsonInput)
        {
            var result = new List<object>();

            var objTree = new Stack<int>();
            var objValue = String.Empty;

            var lvl = 0;
            var objLvl = 0;


            foreach (var chr in jsonInput)
            {
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
                    if (char.Equals('{', chr)) objLvl++;
                    if (char.Equals('}', chr)) objLvl--;
                    if (objLvl == 0 && (char.Equals(',', chr) || char.Equals(']', chr)))
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
