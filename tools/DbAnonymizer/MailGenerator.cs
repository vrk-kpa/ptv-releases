using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;

namespace DbAnonymizer
{
    public class MailGenerator
    {
        private const string FirstNamesFileName = "firstnames.txt";
        private const string LastNamesFileName = "lastnames.txt";
        private const string ResourcePath = "DbAnonymizer.data";
        
        private readonly Random _firstNameRandom = new Random();
        private readonly Random _lastNameRandom = new Random();
        
        private readonly string[] _firstNames;
        private readonly string[] _lastNames;

        private readonly int _firstNamesCount;
        private readonly int _lastNamesCount;
        
        private static readonly HashSet<string> _uniqueEmails = new HashSet<string>();

        public MailGenerator()
        {
            _firstNames = LoadResource(FirstNamesFileName);
            _lastNames = LoadResource(LastNamesFileName);

            _firstNamesCount = _firstNames.Length;
            _lastNamesCount = _lastNames.Length;
        }

        public string GetUniqueMail()
        {
            while(true)
            {
                var mail = GetMail();
                if (_uniqueEmails.Add(mail)) return mail;
            }
        }
        
        private string GetMail()
        {
            var firstName = _firstNames[_firstNameRandom.Next(_firstNamesCount-1)];
            var lastName = _lastNames[_lastNameRandom.Next(_lastNamesCount-1)];
            return $"{firstName.ToLower()}.{lastName.ToLower()}@anonymized.com";
        }
        
        private static string[] LoadResource(string resourceName)
        {
            var fileName = $"{ResourcePath}.{resourceName}";
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName);
            if (stream == null) throw new MissingManifestResourceException($"Resource {resourceName} is missing.");
            var reader = new StreamReader(stream);
            var content = reader.ReadToEnd();
            return content.Split(Environment.NewLine);
        }
    }
}