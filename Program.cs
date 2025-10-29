// See https://aka.ms/new-console-template for more information
using REBL;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var rebl = new REBLConsole();
await rebl.RunHeadless();
await rebl.Run();