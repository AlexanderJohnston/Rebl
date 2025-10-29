using REBL.Commands;
using REBL.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REBL.Tests
{
    public class TesterHelper
    {
        public TesterHelper(REBLConsole console)
        {
            _console = console;
        }
        REBLConsole _console { get; set; }

        // This processes the user input into Rebel.
        public static Command GetInput(ref REBLConsole console, string userMessage)
        {
            var input = new Express("user.input", userMessage, console);
            return input;
        }

        public static Command MakeEchoTemplate(ref REBLConsole console, string templateName)
        {
            var inputTemplate = new Template(templateName, "{0}", console);
            return inputTemplate;
        }

        public static Command MakeTemplate (ref REBLConsole console, string templateName, string template)
        {
            var inputTemplate = new Template(templateName, template, console);
            return inputTemplate;
        }

        public static Command MakeBuffer(ref REBLConsole console, string bufferName)
        {
            var inputBuffer = new Commands.Buffer(bufferName, console);
            return inputBuffer;
        }

        public static Command AddTemplate(ref REBLConsole console, string bufferName, string templateName)
        {
            var addTemplate = new AddTemplate(bufferName, templateName, console);
            return addTemplate;
        }

        public static Command AddExpression(ref REBLConsole console, string bufferName, string expressionName)
        {
            var addExpression = new AddExpression(bufferName, expressionName, console);
            return addExpression;
        }

        public static Command MakeCLaim(ref REBLConsole console, string bufferName)
        {
            var claim = Claim.Test(bufferName, console);
            return claim;
        }

        public static Command MakeAgreement(ref REBLConsole console, string bufferName, string agreementName, string queryName)
        {
            var agreementQuery = new Express(agreementName, queryName, console);
            var command = $"agreement {agreementName} {bufferName} {queryName}";
            var commandName = "agreement";
            var action = console.Actions.First(a => string.Equals(a.Name.ToLower(), commandName));
            var type = action.GetType();
            var method = type.GetMethod("Create");
            // separate command from parsed command with the first space
            var index = command.IndexOf(' ');
            var parsedCommand = command.Substring(index + 1);
            var instance = method.Invoke(action, new object[] { parsedCommand, console });
            return (Command)instance;
        }

        public async Task RunTest()
        {
            var tests = new List<string>();
            var realTests = new List<string>();
            _console.IsReady = false;
            var hello = new Express("greeting", "hello", _console);
            tests.Add(await _console.RunHeadless(hello));
            var echo = new Template("echo", "{0}", _console);
            tests.Add(await _console.RunHeadless(echo));
            var makeBuffer = new Commands.Buffer("sayHello", _console);
            tests.Add(await _console.RunHeadless(makeBuffer));
            var addTemplate = new AddTemplate("sayHello", "echo", _console);
            tests.Add(await _console.RunHeadless(addTemplate));
            var addExpression = new AddExpression("sayHello", "greeting", _console);
            tests.Add(await _console.RunHeadless(addExpression));
            var prefix = new Express("greeting.user.prefix", "This is your first time meeting the user, ", _console);
            tests.Add(await _console.RunHeadless(prefix));
            var root = new Express("user.root", "sshado", _console);
            tests.Add(await _console.RunHeadless(root));
            var postfix = new Express("greeting.user.postfix", ", ask how they are and introduce yourself.", _console);
            tests.Add(await _console.RunHeadless(postfix));
            var greetTemplate = new Template("templates.greeting.user", "{0}{1}{2}", _console);
            tests.Add(await _console.RunHeadless(greetTemplate));
            var userGreetBuffer = new Commands.Buffer("ai.greeting.user", _console);
            tests.Add(await _console.RunHeadless(userGreetBuffer));
            var userGreetTemplate = new AddTemplate("ai.greeting.user", "templates.greeting.user", _console);
            tests.Add(await _console.RunHeadless(userGreetTemplate));
            var userGreetPrefix = new AddExpression("ai.greeting.user", "greeting.user.prefix", _console);
            tests.Add(await _console.RunHeadless(userGreetPrefix));
            var userGreetRoot = new AddExpression("ai.greeting.user", "user.root", _console);
            tests.Add(await _console.RunHeadless(userGreetRoot));
            var userGreetPostfix = new AddExpression("ai.greeting.user", "greeting.user.postfix", _console);
            tests.Add(await _console.RunHeadless(userGreetPostfix));
            var claimUserGreet = Claim.Test("ai.greeting.user", _console);
            tests.Add(await _console.RunHeadless(claimUserGreet));
            //await _console.Run(null, "run ai.greeting.user");
            var agreementQuery = new Express("ai.greeting.query", "Is this suitable for a first time greeting?", _console);
            tests.Add(await _console.RunHeadless(agreementQuery));
            var command = "agreement ai.greeting.agreement ai.greeting.user ai.greeting.query";
            var commandName = "agreement";
            var action = _console.Actions.First(a => string.Equals(a.Name.ToLower(), commandName));
            var type = action.GetType();
            var method = type.GetMethod("Create");
            // separate command from parsed command with the first space
            var index = command.IndexOf(' ');
            var parsedCommand = command.Substring(index + 1);
            var instance = method.Invoke(action, new object[] { parsedCommand, _console });
            tests.Add(await _console.RunHeadless((Command)instance));
            foreach (var test in tests)
            {
                if (!string.IsNullOrEmpty(test))
                {
                    realTests.Add(test);
                }
                else
                {
                    Colors.WriteLine(test, Color.Red);
                }
            }
            _console.IsReady = true;
        }
    }
}
