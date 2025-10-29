using Azure;
using Memory;
using Memory.Chat;
using REBL.Utilities;
using System.Drawing;

namespace REBL.Commands
{
    public abstract class Command
    {
        public Command(REBLConsole console) => Rebel = console;

        public REBLConsole Rebel { get; set; }
        public virtual Action Act { get; set; }
        public virtual string ReadAct() => string.Empty;
        public virtual string Name { get; set; }
        public virtual string Behavior { get; set; }

        public virtual async Task<string> Run(ResponsePredictionEngine Respond)
        {
            var behavior = ReadAct();
            var chat = new List<GptChatMessage>() { new GptChatMessage("assistant", behavior) };
            var task = Respond.PredictResponse(chat, "gpt-4", 0.1f, 256);
            task.Wait();
            if (task.IsCompleted)
            {
                Colors.WriteLine(task.Result, Color.Yellow);
                Behavior = task.Result;
                return task.Result;
            }
            return string.Empty;
        }

        public override string ToString() => ReadAct();
    }
}