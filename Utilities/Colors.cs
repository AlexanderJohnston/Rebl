using System.Drawing;
using Console = Colorful.Console;

namespace REBL.Utilities
{
    public static class Colors
    {
        public struct RichMessage
        {
            public string Text;
            public Color Color;
        }

        // Track the history of commands on a stack
        public static Stack<RichMessage> History { get; set; } = new Stack<RichMessage>();

        public static void Read(int count)
        {
            int minMax = count > History.Count ? History.Count : count;
            
            for (int i = 0; i < minMax; i++)
            {
                var message = History.ElementAt(i);
                Console.Write(message.Text, message.Color);
            }
        }

        // Clear
        public static void Clear()
        {
            History.Clear();
        }

        internal static void Write(string text, Color color)
        {
            var message = new RichMessage { Text = text, Color = color };
            History.Push(message);
            //Console.Write(text, color);
        }
        internal static void WriteAbove(string text, Color color)
        {
            var message = new RichMessage { Text = text, Color = color };
            Console.Write(text, color);
        }

        internal static void WriteLine(string text, Color color)
        {
            var linedText = text + Environment.NewLine;
            var message = new RichMessage { Text = linedText, Color = color };
            History.Push(message);
            //Console.WriteLine(text, color);
        }

        internal static void WriteAboveLine(string text, Color color)
        {
            Console.WriteLine(text, color);
        }

        internal static void WriteRebel(string lastCommand = "", string lastBuffer = "")
        {
            // check if command is 24 or longer and substring it
            lastCommand = lastCommand.Length > 24 ? lastCommand.Substring(0, 24) : lastCommand;

            lastCommand = lastCommand.Length > 0 ? lastCommand : " ";
            var padRight = 24 - lastCommand.Length;
            lastCommand = lastCommand.PadRight(padRight, ' ');
            
            Console.Write("|-", Color.Gray);
            Console.Write("-------------------------------------------------------", Color.White);
            Console.Write("-|", Color.Gray);
            Console.Write(Environment.NewLine);
            Console.Write("| ", Color.Gray);
            Console.Write("  :: <> == ", Color.Red);
            Console.Write("Read", Color.Teal);
            Console.Write(" - ", Color.White);
            Console.Write("Extract", Color.Teal);
            Console.Write(" - ", Color.White);
            Console.Write("Behavior", Color.Teal);
            Console.Write(" - ", Color.White);
            Console.Write("Loop", Color.Teal);
            Console.Write(" == :: <>   ", Color.Red);
            Console.Write(" |", Color.Gray);
            Console.Write(Environment.NewLine);
            Console.Write("|-", Color.Gray);
            Console.Write("-------------------------------------------------------", Color.White);
            Console.Write("-|", Color.Gray);
            Console.Write(Environment.NewLine);
            Console.Write("| ", Color.Gray);
            Console.Write("      ", Color.White);
            Console.Write("Last Command", Color.Gray);
            Console.Write("      ||        ", Color.White);
            Console.Write("Last Buffer", Color.Gray);
            Console.Write("          ", Color.White);
            Console.Write(" |", Color.Gray);
            Console.Write(Environment.NewLine);
            Console.Write("| ", Color.Gray);
            Console.Write($" {lastCommand}||                             ", Color.White);
            Console.Write(" |", Color.Gray);
            Console.Write(Environment.NewLine);
            Console.Write("|-", Color.Gray);
            Console.Write("-------------------------------------------------------", Color.White);
            Console.Write("-|", Color.Gray);
            Console.Write(Environment.NewLine);
        }
    }
}
