using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDownloader.Extensions
{
    class ContentExtensions
    {
        public static List<string> ReformatJson(string content, string token, string subToken = "")
        {
            List<string> List = new List<string>();
            try
            {
                JObject jobject = JObject.Parse(content);
                JToken[] tokens = jobject.SelectToken(token).ToArray();

                foreach (JToken item in tokens)
                {
                    string Output = "";
                    foreach (JProperty jprop in JObject.Parse(item.SelectToken("attributes").ToString()).Properties())
                    {
                        Output += "<i>" + jprop.Name + ": </i>" + jprop.Value + "<br/>" + Environment.NewLine;
                    }

                    List.Add(Output.ToString());
                }
            }
            catch (Exception)
            {
                List.Add(content);
            }


            return List;
        }

        public static string[] GetCharsWithCombinations(bool Uppercase = true, int length = 2)
        {
            if (Uppercase)
            {
                List<string> Combinations = new List<string>();
                for (int i = 65; i <= 90; i++)
                    for (int j = 65; j <= 90; j++)
                        Combinations.Add((char)i + "" + (char)j);

                return Combinations.ToArray();
            }
            else
            {
                List<string> Combinations = new List<string>();
                for (int i = 97; i <= 122; i++)
                    for (int j = 97; j <= 122; j++)
                        Combinations.Add((char)i + "" + (char)j);

                return Combinations.ToArray();
            }
        }


        public static string[] GetCharsWithCombinationsThree(bool Uppercase = true, int length = 2)
        {
            if (Uppercase)
            {
                List<string> Combinations = new List<string>();
                for (int i = 65; i <= 90; i++)
                    for (int j = 65; j <= 90; j++)
                        for (int k = 65; k <= 90; k++)
                            Combinations.Add((char)i + "" + (char)j + "" + (char)k);

                return Combinations.ToArray();
            }
            else
            {
                List<string> Combinations = new List<string>();
                for (int i = 97; i <= 122; i++)
                    for (int j = 97; j <= 122; j++)
                        Combinations.Add((char)i + "" + (char)j);

                return Combinations.ToArray();
            }
        }

        public static string[] GetCharsWithCombinationsFour(bool Uppercase = true, int length = 2)
        {
            if (Uppercase)
            {
                List<string> Combinations = new List<string>();
                for (int i = 65; i <= 90; i++)
                    for (int j = 65; j <= 90; j++)
                        for (int k = 65; k <= 90; k++)
                            for (int z = 65; z <= 90; z++)
                                Combinations.Add((char)i + "" + (char)j + "" + (char)k + "" + (char)z);

                return Combinations.ToArray();
            }
            else
            {
                List<string> Combinations = new List<string>();
                for (int i = 97; i <= 122; i++)
                    for (int j = 97; j <= 122; j++)
                        Combinations.Add((char)i + "" + (char)j);

                return Combinations.ToArray();
            }
        }



    }
}
