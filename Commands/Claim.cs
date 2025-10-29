using System.Drawing;
using System.Linq;
using REBL.Utilities;

namespace REBL.Commands
{
    public class Claim : Command, SetupCommand<Claim>
    {
        public Claim(REBLConsole console) : base(console)
        {
            Name = "claim";
            Act = () => MakeClaim();
        }

        public Claim(string name, Template template, Express[] expressions, REBLConsole console) : base(console)
        {
            Name = name;
            var expressionParsed = new Dialogue.Expression[expressions.Length];
            // Change this to an indexed for loop
            for (int i = 0; i < expressions.Length; i++)
            {
                expressionParsed[i] = expressions[i].Expression;
            }
            Claims = new Dialogue.Claim(template.Structure, expressionParsed);
            Act = () => MakeClaim();
        }

        public Dialogue.Claim Claims { get; set; }

        public Claim Create(string key, REBLConsole console)
        {
            // Use the key to lookup the buffer containing a tuple of the Template and the Expression array
            var buffer = console.Dynamics[key];
            var unpackedBuffer = (Tuple<Template, Express[]>)buffer;
            var claim = new Claim(key, unpackedBuffer.Item1, unpackedBuffer.Item2, console);
            base.Rebel.Dynamic(key, claim);
            return claim;
        }

        public string MakeClaim()
        {
            var output = Claims.WriteSafe();
            Colors.WriteLine(output, Color.White);
            return output;
        }

        public override string ReadAct() => MakeClaim();

        public static Claim Test(string key, REBLConsole console)
        {
            // Use the key to lookup the buffer containing a tuple of the Template and the Expression array
            var buffer = console.Dynamics[key];
            var unpackedBuffer = (Tuple<Template, Express[]>)buffer;
            var claim = new Claim(key, unpackedBuffer.Item1, unpackedBuffer.Item2, console);
            console.Dynamic(key, claim);
            return claim;
        }
    }





}