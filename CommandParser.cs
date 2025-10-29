namespace REBL
{
    // This class is responsible for handling all of the different parsing which goes on in REBLConsole to run and manipulate commands and buffers.
    // It's only job is to break the strings out into their components and return them.
    public static class CommandParser
    {
        public static string[] Parse(string command)
        {
            var commandIndex = command.IndexOf(' ');
            if (commandIndex < 0)
            {
                commandIndex = command.Length;
            }
            var commandName = command.Substring(0, commandIndex).ToLower();
            string parsedCommand;

            if (HasParameters(command))
            {
                parsedCommand = command.Substring(commandIndex + 1);
            }
            else
            {
                parsedCommand = command;
            }
            return new string[] { commandName, parsedCommand };
        }

        public static string[] SplitParameters(string parsedCommand)
        {
            var parameters = parsedCommand.Split(' ');
            return parameters;
        }

        public static bool HasParameters(string command)
        {
            var trimmed = command.Trim();
            var index = trimmed.IndexOf(' ');
            if (index > 0)
            {
                return true;
            }
            else return false;
        }
    }
}