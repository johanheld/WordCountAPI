using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace WordCountAPI.Services
{
    /// <summary>
    /// TextProcessor is a static class with the method EditText which
    /// takes the path to a text file uploaded by a client to the server,
    /// counts the words in it and returns the text as a string with the
    /// most used word surrounded by foo and bar. A boolean is used as an
    /// out variable for checking if the method succeeded.
    /// </summary>
    public static class TextProcessor
    {
        public static string EditText(string filepath, out bool success)
        {
            char[] delimiters =
            {
                ' ', '\n', '=', ',', '.', '/', '(', ')', '?', '!', '\t', '-', '"', ':', ';', '<', '>', '`', '´', '_',
                '@', '*', '[', ']'
            };
            string text = System.IO.File.ReadAllText(filepath);

            if (String.IsNullOrEmpty(text))
            {
                success = false;
                return "Error: Text file is empty";
            }

            string textForCounting = text.ToLower();
            textForCounting = Regex.Replace(textForCounting, @"_ ", "");

            string[] words = textForCounting.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 0)
            {
                success = false;
                return "Error: No words found";
            }

            Dictionary<String, int> occurences = new Dictionary<string, int>();

            foreach (string s in words)
            {
                if (!occurences.ContainsKey(s))
                    occurences.Add(s, 1);

                else
                    occurences[s]++;
            }

            // Get keys with most occurences.
            var max = from x in occurences where x.Value == occurences.Max(v => v.Value) select x.Key;

            // Two replaces are made with Regex to get correct casing.
            foreach (var key in max)
            {
                string keyWithUpperCase = Char.ToUpper(key[0]) + key.Substring(1);
                string wordToFind1 = @"\b" + key + @"\b";
                string replacement1 = "foo" + key + "bar";
                string wordToFind2 = @"\b" + keyWithUpperCase + @"\b";
                string replacement2 = "foo" + keyWithUpperCase + "bar";

                text = Regex.Replace(text, wordToFind1, replacement1);
                text = Regex.Replace(text, wordToFind2, replacement2);
            }

            success = true;
            return text;
        }
    }
}