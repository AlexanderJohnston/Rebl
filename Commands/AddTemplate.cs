using REBL.Utilities;

namespace REBL.Commands
{
    //Does the same as AddExpression but this one adds Templates to a buffer with the "templates." prefix
    public class AddTemplate : Command, SetupCommand<AddTemplate>
    {
        public AddTemplate(REBLConsole console) : base(console)
        {
            Name = "addtemplate";
            Act = () => AddTemplateToBuffer();
        }
        public AddTemplate(string key, string templateName, REBLConsole console) : base(console)
        {
            Name = $"{key}.addtemplate";
            Key = key;
            TemplateName = templateName;
            Act = () => AddTemplateToBuffer();
        }

        public string Key { get; set; }
        public string TemplateName { get; set; }

        public AddTemplate Create(string input, REBLConsole console)
        {
            // Get the key and template name from the string
            string key, templateName;
            int index = input.IndexOf(' ');
            if (index > 0)
            {
                key = input.Substring(0, index);
                templateName = input.Substring(index + 1);
            }
            else
            {
                key = input;
                templateName = "";
            }
            return new AddTemplate(key, templateName, console);
        }

        private void AddTemplateToBuffer()
        {
            // Get the template
            var prefixed = string.Format("{0}", TemplateName);
            var newTemplate = (Template)Rebel.Dynamics[prefixed];
            // Unpack the buffer
            var packedBuffer = Rebel.Dynamics[Key];
            var unpack = DynamicBuffer<Tuple<Template, Express[]>>.Unpack(packedBuffer);
            Tuple<Template, Express[]> newTuple;
            if (packedBuffer.Count == 0)
            {
                newTuple = new Tuple<Template, Express[]>(newTemplate, new Express[0]);
            }
            else
            {
                var expressions = unpack.Item2;
                newTuple = new Tuple<Template, Express[]>(newTemplate, expressions);
            }
            Rebel.Dynamic(Key, newTuple);
        }
    }





}