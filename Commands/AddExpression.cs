using Dialogue;
using REBL.Utilities;

namespace REBL.Commands
{
    //AddExpression adds an expression to the buffer, it looks up the template name from the buffer and adds the expression to it
    public class AddExpression : Command, SetupCommand<AddExpression>
    {
        public AddExpression(REBLConsole console) : base(console)
        {
            Name = "addexpression";
            Act = () => { };
        }
        public AddExpression(string key, string expression, REBLConsole console) : base(console)
        {
            Name = $"{key}.addexpression";
            Key = key;
            Expression = expression;
            Act = () => AddExpressionToBuffer();
        }

        public string Key { get; set; }
        public string Expression { get; set; }

        public AddExpression Create(string input, REBLConsole console)
        {
            // Get the key and expression from the string
            string key, expression;
            int index = input.IndexOf(' ');
            if (index > 0)
            {
                key = input.Substring(0, index);
                expression = input.Substring(index + 1);
            }
            else
            {
                key = input;
                expression = "";
            }
            return new AddExpression(key, expression, console);
        }

        private void AddExpressionToBuffer()
        {
            var packedBuffer = Rebel.Dynamics[Key];
            var unpack = DynamicBuffer<Tuple<Template, Express[]>>.Unpack(packedBuffer);
            var template = unpack.Item1;
            var expressions = unpack.Item2;
            var prefixed = string.Format("{0}", Expression);
            var newExpression = (Express)Rebel.Dynamics[prefixed];
            var newExpressions = new Express[expressions.Length + 1];
            for (int i = 0; i < expressions.Length; i++)
            {
                newExpressions[i] = expressions[i];
            }
            newExpressions[^1] = newExpression;
            var newTuple = new Tuple<Template, Express[]>(template, newExpressions);
            Rebel.Dynamic(Key, newTuple);
        }
    }
}