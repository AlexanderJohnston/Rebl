using Dialogue;
using REBL.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REBL.Commands
{
    public class Agreement : Command, SetupCommand<Agreement>
    {
        public Agreement(REBLConsole console) : base(console)
        {
            Name = "agreement";
            Act = () => MakeAgreement();
        }

        public Agreement(Dialogue.Claim claim, Dialogue.Expression expression, REBLConsole console) : base(console)
        {
            Name = $"{claim}.{expression}";
            Agreements = new Dialogue.Agreement(claim, expression);
            Act = () => MakeAgreement();
        }

        public Dialogue.Agreement Agreements { get; set; }

        public Agreement Create(string input, REBLConsole console)
        {
            // Split the input string to get the key for the claim and expression
            var parts = input.Split(new[] { ' ' }, 3);
            if (parts.Length < 3) throw new ArgumentException("Input must contain at least 3 parts separated by spaces.");

            string key = parts[0];
            string claimKey = parts[1];
            string expressionKey = parts[2];

            // Unpack the Claim from the console's Dynamics using the provided keys
            var claimBuffer = console.Dynamics[claimKey];
            if (!(claimBuffer is Claim claimInstance))
            {
                throw new InvalidOperationException($"No Claim found in Dynamics with the key '{claimKey}'.");
            }

            // Unpack the Expression from the console's Dynamics using the provided keys
            var expressionBuffer = console.Dynamics[expressionKey];
            if (!(expressionBuffer is Express))
            {
                throw new InvalidOperationException($"No Expression found in Dynamics with the key '{expressionKey}'.");
            }
            var expressionInstance = (Express)expressionBuffer;

            // Create a new Agreement using the unpacked Claim and Expression
            var agreement = new Agreement(claimInstance.Claims, expressionInstance.Expression, console);
            console.Dynamic(key, agreement);
            return agreement;
        }

        // Old method
        //public Agreement Create(string input, REBLConsole console)
        //{
        //    // Find index of first space, then separate out the preceding string as the key, the second word as the claim, and third the query
        //    string key, claim, query;
        //    int index = input.IndexOf(' ');
        //    if (index > 0)
        //    {
        //        key = input.Substring(0, index);
        //        var remaining = input.Substring(index + 1);

        //        int secondIndex = remaining.IndexOf(' ');
        //        claim = remaining.Substring(0, secondIndex);
        //        query = remaining.Substring(secondIndex + 1);
        //    }
        //    else
        //    {
        //        key = input;
        //        claim = "";
        //        query = "";
        //    }

        //    // Use the key to lookup the buffer containing a tuple of the Claim and the Expression
        //    var claimBuffer = console.Dynamics[claim];
        //    var unpackedClaimBuffer = (Claim)claimBuffer;
        //    var expressionBuffer = console.Dynamics[query];
        //    var unpackedExpressionBuffer = (Express)expressionBuffer;
        //    var agreement = new Agreement(unpackedClaimBuffer.Claims, unpackedExpressionBuffer.Expression, console);
        //    base.Rebel.Dynamic(key, agreement);
        //    return agreement;
        //}

        public override string ReadAct() => MakeAgreement();

        public string MakeAgreement()
        {
            var output = Agreements.WriteSafe();
            Colors.WriteLine(output, Color.White);
            return output;
        }
        public static Agreement Test(string key, REBLConsole console)
        {
            // Use the key to lookup the buffer containing a Claim
            var claimBuffer = console.Dynamics[key];
            if (!(claimBuffer is Claim claimInstance))
            {
                throw new InvalidOperationException($"No Claim found in Dynamics with the key '{key}'.");
            }

            // As expressions are part of Claim, no separate buffer is needed for expression
            var agreement = new Agreement(claimInstance.Claims, claimInstance.Claims.Expressions.FirstOrDefault(), console);
            console.Dynamic(key, agreement);
            return agreement;
        }
    }
}
