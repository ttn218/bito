using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GramDll
{
    public class Gram
    {
        private static List<Gram> grams = new List<Gram>();
        private string grammar;
        private string command;
        private static bool fileread = false;


        private Gram(string grammar, string command)
        {
            this.grammar = grammar;
            this.command = command;
        }

        public static void SetGram(string grammar, string command)
        {
            Gram gram = new Gram(grammar, command);
            grams.Add(gram);
        }

        public static string GetCommand(string grammar)
        {
            Gram gram = grams.Find((o) => o.grammar.Equals(grammar));
            
            return gram.command;
        }
        public static List<string> GetComaands()
        {
            return grams.Select(o => o.command).Distinct().ToList();
        }
        public static void ToFile()
        {
            string path = @"C:\\gram";
            JObject jObject = new JObject();
            jObject.RemoveAll();
            
            FileInfo file;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);

            }
            file = new FileInfo(Path.Combine(path, "gram.json"));
            if (!file.Exists)
            {
                file.Create();
            }
            
            using (StreamWriter Sw = new StreamWriter(file.OpenWrite(), Encoding.UTF8))
            {
                
                foreach (var com in GetComaands())
                {
                    List<string> ls = grams.FindAll(o => o.command.Equals(com)).Select(s => s.grammar).Distinct().ToList();
                    
                    jObject.Add(com, new JArray(ls));
                    
                }
                
                Sw.Write(jObject.ToString());
            }
            
        }

        public static Grammar CreateGram()
        {
            ReadGrammar();
            Choices choices = SetChoices();

            GrammarBuilder gb = new GrammarBuilder
            {
                Culture = new System.Globalization.CultureInfo("ko-KR")
            };
            gb.Append(choices);

            return new Grammar(gb);
        }
        private static Choices SetChoices()
        {
            Choices choices = new Choices();
            foreach(var list in grams)
            {
                choices.Add(list.grammar);
            }
            return choices;
        }
        private static void ReadGrammar()
        {
            if (fileread == true) return;
            string path = @"C:\\gram";
            FileInfo file;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);

            }
            file = new FileInfo(System.IO.Path.Combine(path, "gram.json"));
            if(!file.Exists)
            {
                file.Create();
            }
            using (StreamReader sr = new StreamReader(file.OpenRead(), Encoding.UTF8))
            {
                JObject jObject = new JObject();
                string strdata= sr.ReadToEnd();
                jObject = JObject.Parse(strdata);
                List<string> list = jObject.Properties().Select((o) => o.Name).ToList();
                foreach(string str in list)
                {
                    JArray jArray = (JArray)jObject[str];
                    foreach(string grammer in jArray)
                    {
                        SetGram(grammer, str);
                    }
                }
                /*while (!sr.EndOfStream)
                {
                    string[] strdata = sr.ReadLine().Split(':');
                    string[] text = strdata[1].Split(',');
                    foreach (string str in text)
                    {
                        Gram.SetGram(str, strdata[0]);
                    }
                }*/
                
            }
            fileread = true;

        }
    }
}
