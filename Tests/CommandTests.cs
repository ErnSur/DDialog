// namespace Tests
// {
//     using System.Collections.Generic;
//     using System.Linq;
//     using NUnit.Framework;
//     using Doublsb.Dialog;
//     using UnityEngine;
//
//     [TestOf(typeof(CommandParser))]
//     public class CommandParserTests
//     {
//         private const string ColorTag = "color";
//         private const string Red = "red";
//         private const string PrintContent = "Content";
//         private readonly string _script = $"print{Tag(ColorTag, Red, PrintContent)}print";
//         private IEnumerable<ICommand> _commands;
//         private ICommandFactory _commandFactory;
//
//         [SetUp]
//         public void SetUp()
//         {
//             _printer = new TestPrinter();
//             _commandFactory = new BasicCommandFactory(_printer);
//             _commands = CommandParser.ParseCommands(_script,_commandFactory);
//         }
//
//         [Test]
//         public void commands_are_in_correct_order()
//         {
//             var commands = _commands.ToList();
//             Assert.AreEqual(5, commands.Count);
//             Assert.IsInstanceOf<PrintCommand>(commands[0]);
//             Assert.IsInstanceOf<ColorCommand>(commands[1]);
//             Assert.IsInstanceOf<PrintCommand>(commands[2]);
//             Assert.IsInstanceOf<ColorCommand>(commands[3]);
//             Assert.IsInstanceOf<PrintCommand>(commands[4]);
//             Assert.AreSame(commands[1], commands[3]);
//         }
//
//         private static string Tag(string tag, string content) => Tag(tag, null, content);
//
//         private static string Tag(string tag, string arg, string content)
//         {
//             var argStr = string.IsNullOrEmpty(arg) ? "" : $"={arg}";
//             return $"<{tag}{argStr}>{content}</{tag}>";
//         }
//
//         private string NoParse(string content) => $"<noparse>{content}</noparse>";
//
//         [HideInCallstack]
//         private void AssertPrinterText(string expected)
//         {
//             Assert.AreEqual(NoParse(expected), _printer.TextNoParse);
//         }
//     }
// }