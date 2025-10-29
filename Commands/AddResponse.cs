using Dialogue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REBL.Commands
{
    // Adds the last string from REBLConsole's History to a new expression given by the key passed in.
    public class AddResponse : Command, SetupCommand<AddResponse>
    {
        public AddResponse(REBLConsole console) : base(console)
        {
            Name = "addresponse";
            Act = () => AddResponseToExpression();
        }

        public AddResponse(string key, REBLConsole console) : base(console)
        {
            Name = key;
            Act = () => AddResponseToExpression();
        }

        public Expression Expression { get; set; }

        public AddResponse Create(string input, REBLConsole console) => new AddResponse(input, console);

        public void AddResponseToExpression()
        {
            // Get the last string from the History
            var lastString = Rebel.History.Last();
            var expression = new Express(Name, lastString, Rebel);
            // Get the Expression from the console's Dynamics using the provided key
            Rebel.Dynamics[Name] = expression;
        }
    }
}
