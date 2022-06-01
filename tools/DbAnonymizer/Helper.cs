using System;

namespace DbAnonymizer
{
    public static class Helper
    {
        public static void Write(string text, ConsoleColor color)
        {
            WriteInternal(color, () => Console.Write(text));
        } 
        
        public static void WriteLine(string text, ConsoleColor color)
        {
            WriteInternal(color, () => Console.WriteLine(text));
        }
        
        private static void WriteInternal(ConsoleColor color, Action action)
        {
            var colorBackup = Console.ForegroundColor;
            Console.ForegroundColor = color;
            action();
            Console.ForegroundColor = colorBackup;
        }

        public static void AddToProcessingMessage(ref string processingMessage, string text)
        {
            if (processingMessage == null) return;
            CleanLineText(processingMessage.Length);
            processingMessage += text;
            Console.Write(processingMessage);
        }

        public static void RemoveFromProcessingMessage(ref string processingMessage, int length)
        {
            if (processingMessage == null) return;
            CleanLineText(processingMessage.Length);
            processingMessage = processingMessage[..^length];
            Console.Write(processingMessage);
        }

        public static void CleanLineText(int length)
        {
            Console.SetCursorPosition(Console.CursorLeft - length, Console.CursorTop);
            Console.Write(new string(' ', length));
            Console.SetCursorPosition(Console.CursorLeft - length, Console.CursorTop);
        }
    }
}