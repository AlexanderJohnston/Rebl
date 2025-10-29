using System;
using System.Drawing;
using Memory;
using Memory.Chat;
using REBL.Commands;
using REBL.Tests;
using REBL.Utilities;

namespace REBL
{

    

    // Track the history of commands and their responses. Should be able to track commands run, run those commands and record the results.
    public class CommandRunner
    {
        private readonly List<Tuple<string, string>> _commandHistory = new();
        private readonly ResponsePredictionEngine _responseEngine;

        public CommandRunner(ResponsePredictionEngine responseEngine)
        {
            _responseEngine = responseEngine;
        }

        public async Task<string> ExecuteCommand(Command command)
        {
            command.Act();
            var result = command.ReadAct();
            _commandHistory.Add(Tuple.Create(command.Name, result));
            return result; //todo
        }

        public async Task<string> RunCommand(Command command)
        {
            string result = await command.Run(_responseEngine);
            _commandHistory.Add(Tuple.Create(command.Name, result));
            return result;
        }

        public IReadOnlyList<Tuple<string, string>> CommandHistory => _commandHistory.AsReadOnly();
    }

    // Create a simple REPL for a C# console
    public class REBLConsole
    {
        public REBLConsole()
        {
            Name = "Realization.REBL";
            Actions = new Command[2];
            Actions[0] = new Clear(this);
            Actions[1] = new Help(this);
            GetActions();
            var token = File.ReadAllText("./key.openAI");
            Respond = new ResponsePredictionEngine(token);
            _runner = new CommandRunner(Respond);
            // TODO maybe unit tests instead eh?
            var tester = new TesterHelper(this);
            tester.RunTest().Wait();
        }

        public bool IsReady = true;
        string Name;
        string? CurrentCommand;
        public Command[] Actions;
        public CommandRunner _runner;
        Command[] Commands;
        ResponsePredictionEngine Respond { get; set; }
        public Dictionary<string, dynamic> Dynamics = new();
        public List<string> History = new();

        public void Dynamic(string key, dynamic thing) => Dynamics[key] = thing;

        public async Task<string> RunHeadless(Command? command = null, string? input = "")
        {
            if (command != null)
            {
                return ReportHeadless(command);
            }
            if (!string.IsNullOrEmpty(input))
            {
                return await ReadInputHeadless(input);
            }
            else return string.Empty;
        }

        public async Task Run(Command? command = null, string? input = "")
        {
            //ClearAct();
            Console.Clear();
            if (command != null)
            {
                Colors.WriteRebel(command.Name);
            }
            else
            {
                Colors.WriteRebel();
            }
            if (command != null)
            {
                Report(command);
            }
            if (!string.IsNullOrEmpty(input))
            {
                await ReadInput(input);
            }
            if (IsReady)
            {
                Ready();
                await ReadInput(null);
            }
        }

        public void ClearAct()
        {
            var command = new Clear(this);
            Report(command);
        }

        public void GetActions()
        {
            var actions = ActionBuilder.GetInstances<Command>();
            Actions = new Command[actions.Count];
            //Colors.Write("Command List:", Color.White);
            for (int i = 0; i < actions.Count; i++)
            {
                Command instance = Create<Command>(actions[i]);
                Actions[i] = instance;
                //Colors.WriteLine(actions[i].Name, Color.Green);
            }
        }

        public T Create<T>(Type selected)
        {
            if (selected.IsSubclassOf(typeof(T)))
            {
                T instance = (T)Activator.CreateInstance(selected, this);
                return instance;
            }
            else return default(T);
        }

        public void Report(Command? command = null)
        {
            if (command != null)
            {
                command.Act();
            }
        }

        public string ReportHeadless(Command? command = null)
        { 
            if (command != null)
            {
                command.Act();
            }
            return command.ReadAct();
        }

        public void Ready()
        {
            string prefix = string.Format("{0}> ", Name);
            Colors.WriteAboveLine(prefix, Color.Red);
            Colors.Read(10);
            Console.SetCursorPosition(18, 6);
        }

        public async Task<string> ReadInputHeadless(string? optOverride = "")
        {
            string command;
            if (!string.IsNullOrEmpty(optOverride))
            {
                command = optOverride;
            }
            else
            {
                return string.Empty;
            }

            CurrentCommand = command;
            string commandName;
            string[] parsedCommand;
            BreakOutCommand(command, out commandName, out parsedCommand);

            if (commandName.StartsWith("run"))
            {
                return await RunCommandHeadless(command);
            }
            else if (Actions.Any(a => string.Equals(a.Name, commandName, StringComparison.OrdinalIgnoreCase)))
            {
                return await ReadCommandHeadless(commandName, parsedCommand);
            }
            else
            {
                // Check if any of the dynamics have a key matching the command
                if (Dynamics.Any(entry => string.Equals(entry.Key, command, StringComparison.OrdinalIgnoreCase)))
                {
                    return Read(command);
                }

                // Get the Unknown action from Actions
                var action = Actions.First(a => string.Equals(a.Name.ToLower(), "unknown"));
                var type = typeof(Unknown);
                var method = type.GetMethod("Create");
                var instance = method.Invoke(action, new object[] { parsedCommand[0], this });
                return await RunHeadless((Command)instance);
            }
        }

        public async Task ReadInput(string? optOverride = "")
        {
            string command;
            if (!string.IsNullOrEmpty(optOverride))
            {
                command = optOverride;
            }
            else
            {
                command = Console.ReadLine();
            }

            CurrentCommand = command;
            string commandName;
            string[] parsedCommand;
            BreakOutCommand(command, out commandName, out parsedCommand);

            if (commandName == "exit")
            {
                Colors.WriteAboveLine("Goodbye!", Color.Red);
                return;
            }
            else if (commandName.StartsWith("run"))
            {
                await RunCommand(command);
            }
            else if (Actions.Any(a => string.Equals(a.Name, commandName, StringComparison.OrdinalIgnoreCase)))
            {
                await ReadCommand(commandName, parsedCommand);
            }
            else
            {
                // Check if any of the dynamics have a key matching the command
                if (Dynamics.Any(entry => string.Equals(entry.Key, command, StringComparison.OrdinalIgnoreCase)))
                {
                    Read(command);

                    //if (command is Claim)
                    //{
                    //    Claim(command);
                    //}
                    //else
                    //{
                    //    Read(command);
                    //}
                    await Run();
                }
                // Get the Unknown action from Actions
                var action = Actions.First(a => string.Equals(a.Name.ToLower(), "unknown"));
                var type = typeof(Unknown);
                var method = type.GetMethod("Create");
                var instance = method.Invoke(action, new object[] { parsedCommand[0], this });
                await Run((Command)instance);
            }
        }

        private async Task<string> ReadCommandHeadless(string commandName, string[] parsedCommand)
        {
            var action = Actions.First(a => string.Equals(a.Name.ToLower(), commandName));
            var type = action.GetType();
            var method = type.GetMethod("Create");
            var instance = method.Invoke(action, new object[] { parsedCommand[0], this });
            var result = await _runner.ExecuteCommand((Command)instance);
            return result;
        }

        private async Task ReadCommand(string commandName, string[] parsedCommand)
        {
            var action = Actions.First(a => string.Equals(a.Name.ToLower(), commandName));
            var type = action.GetType();
            var method = type.GetMethod("Create");
            var instance = method.Invoke(action, new object[] { parsedCommand[0], this });
            await _runner.ExecuteCommand((Command)instance);
            Run();
        }

        private async Task RunCommand(string command)
        {
            var key = command.Substring(4);
            if (!Dynamics.Any(x => x.Key == key))
            {
                Colors.WriteLine("Unknown key requested.", Color.Orange);
                await Run();
            }
            var evalCommand = Dynamics[key];
            var isCommand = Buffers.Is<Command>(evalCommand);
            if (isCommand)
            {
                var getBehavior = (Command)evalCommand;
                var result = await _runner.RunCommand(getBehavior);
                if (string.IsNullOrEmpty(result))
                {
                    Colors.WriteLine($"{key} failed.", Color.Red);
                }
                History.Add(result);
                await Run();
            }
        }

        private async Task<string> RunCommandHeadless(string command)
        {
            var key = command.Substring(4);
            if (!Dynamics.Any(x => x.Key == key))
            {
                return "Unknown Key";
                //Colors.WriteLine("Unknown key requested.", Color.Orange);
            }
            var evalCommand = Dynamics[key];
            var isCommand = Buffers.Is<Command>(evalCommand);
            if (isCommand)
            {
                var getBehavior = (Command)evalCommand;
                var result = await _runner.RunCommand(getBehavior);
                if (string.IsNullOrEmpty(result))
                {
                    //Colors.WriteLine($"{key} failed.", Color.Red);
                    return "Failed Execution";
                }
                History.Add(result);
                return result;
            }
            return "No Result";
        }

        private static void BreakOutCommand(string command, out string commandName, out string[] parsedCommand)
        {
            var parsed = CommandParser.Parse(command);
            commandName = parsed[0];
            if (CommandParser.HasParameters(command))
            {
                //parsedCommand = CommandParser.SplitParameters(parsed[1]);
                parsedCommand = new[] { parsed[1] };
            }
            else
            {
                parsedCommand = new[] { command };
            }
        }

        private bool HasParameters(string command)
        {
            var trimmed = command.Trim();
            var index = trimmed.IndexOf(' ');
            if (index > 0)
            {
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Reads a dynamic from the Dynamics dictionary as a List of Tuple<Dialogue.Template, Dialogue.Expression[]>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private void Claim(string key)
        {
            var unpackedBuffer = (Tuple<Dialogue.Template, Dialogue.Expression[]>)Dynamics[key];
            var template = unpackedBuffer.Item1;
            var expressions = unpackedBuffer.Item2;
            var claim = new Dialogue.Claim(template, expressions);
            var makeClaim = claim.WriteSafe();
            Colors.WriteLine($"Claim: {makeClaim}", Color.Red);
        }

        private string Read(string key)
        {
            var currentLines = Colors.History.Count;
            var packedBuffer = Dynamics[key];
            var isCommand = Buffers.Is<Command>(packedBuffer);
            if (!isCommand)
            {
                //Colors.WriteLine("Can not run buffers, must be a command.", Color.Red);
                return "No Buffer";
            }
            var unpackedBuffer = (Command)Dynamics[key];
            return unpackedBuffer.ReadAct();
            //Colors.Read(Colors.History.Count - currentLines);
            //Colors.WriteLine($"Expression: {makeClaim}", Color.Red);
        }
    }
}