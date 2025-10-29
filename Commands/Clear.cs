using System.Drawing;
using REBL.Utilities;

namespace REBL.Commands
{
    public class Clear : Command, SetupCommand<Clear>
    {
        public Clear(REBLConsole console) : base(console)
        {
            Name = "clear";
            Act = () =>
            {
                Console.Clear();
                Colors.Clear();
                Colors.WriteRebel("clear");
            };
        }

        public Clear Create(string input, REBLConsole console) => new Clear(console);
    }
}