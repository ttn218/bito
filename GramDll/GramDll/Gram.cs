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

        public static List<Gram> Grams { get => grams; set => grams = value; }
        public string Grammar { get => grammar; set => grammar = value; }
        public string Command { get => command; set => command = value; }

        private Gram(string grammar, string command)
        {
            this.Grammar = grammar;
            this.Command = command;
        }

        public static void SetGram(string grammar, string command)
        {
            Gram gram = new Gram(grammar, command);
            Grams.Add(gram);
        }

        public static string GetCommand(string grammar)
        {
            Gram gram = Grams.Find((o) => o.Grammar.Equals(grammar));
            
            return gram.Command;
        }
        public static List<string> GetComaands()
        {
            return Grams.Select(o => o.Command).Distinct().ToList();
        }
        public static void ToFile(Stream stream)
        {
            
            JObject jObject = new JObject();
            using (StreamWriter Sw = new StreamWriter(stream, Encoding.UTF8))
            {
                foreach (var com in GetComaands())
                {
                    List<string> ls = Grams.FindAll(o => o.Command.Equals(com)).Select(s => s.Grammar).Distinct().ToList();
                    
                    jObject.Add(com, new JArray(ls));
                    
                }
                Sw.Write(jObject.ToString());
            }
            
        }

        public static Grammar CreateGram(Stream stream)
        {
            if (stream == null) return null;
            if (!ReadGrammar(stream)) return null;
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
            foreach(var list in Grams)
            {
                choices.Add(list.Grammar);
            }
            return choices;
        }
        private static bool ReadGrammar(Stream stream)
        {
            if (fileread == true) return false;

            using (StreamReader sr = new StreamReader(stream, Encoding.UTF8))
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
                
            }
            fileread = true;
            return true;
        }
    }
}
