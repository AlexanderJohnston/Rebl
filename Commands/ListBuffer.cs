using Newtonsoft.Json;
using REBL.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REBL.Commands
{
    // Command which lists all buffers if no string is passed in, or lists the contents of a buffer if it is.
    public class ListBuffer : Command, SetupCommand<ListBuffer>
    {
        public ListBuffer(REBLConsole console) : base(console)
        {
            Name = "listbuffer";
            Act = () => ListBuffers();
        }

        public ListBuffer(string key, REBLConsole console) : base(console)
        {
            Name = key;
            Act = () => ListBuffers();
        }

        public ListBuffer Create(string input, REBLConsole console) => new ListBuffer(input, console);

        public void ListBuffers()
        {
            // If no string is passed in, list all buffers
            if (Name == "listbuffer")
            {
                ListAll();
            }
            // Otherwise, list the contents of the buffer with the provided key
            else
            {
                if (!Rebel.Dynamics.Any(x => x.Key == Name)) 
                {
                    ListAll();
                    return;
                }

                var buffer = Rebel.Dynamics[Name];
                var isCommand = Buffers.Is<Command>(buffer);
                if (isCommand)
                {
                    var serial = Convert.ToString(buffer);
                }
                else
                {
                    var serial = Convert.ToString(buffer);
                }
            }
        }

        private void ListAll()
        {
            var sb = new StringBuilder();
            foreach (var buffer in Rebel.Dynamics)
            {
                var isCommand = Buffers.Is<Command>(buffer.Value);
                if (isCommand)
                {
                    Colors.WriteLine(buffer.Key, Color.Blue);
                }
                else
                {
                    Colors.WriteLine(buffer.Key, Color.Yellow);

                }
            }
        }
    }
}
