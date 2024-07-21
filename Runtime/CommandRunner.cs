namespace Doublsb.Dialog
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Cysharp.Threading.Tasks;
    using JetBrains.Annotations;
    using UnityEngine;

    // TODO: Maybe command text content should be passed here (use-case: note command handler can easily access the note text)
    public delegate UniTask CommandCallback(string[] args, CancellationToken cancellationToken);

    public class CommandRunner
    {
        private readonly Dictionary<string, CommandData> _commandCallbacks = new();

        // TODO: Add support for floating point indexing
        /// <param name="commandName"> The name of the command to register the callback for. </param>
        /// <param name="beginCallback"> The callback to be executed when the command is encountered. </param>
        /// <param name="endCallback"> The callback to be executed when the command is finished. </param>
        /// <param name="options"> The options for the callback. </param>
        public void RegisterCommandCallback([NotNull] string commandName, [CanBeNull] CommandCallback beginCallback,
            [CanBeNull] CommandCallback endCallback = null, CommandCallbackOptions options = default, [CallerFilePath] string callerFilePath = "")
        {
            if (!_commandCallbacks.TryGetValue(commandName, out var commandData))
            {
                _commandCallbacks[commandName] = commandData = new CommandData();
            }
            callerFilePath = Path.GetFileNameWithoutExtension(callerFilePath);

            if (beginCallback != null)
            {
                // Insert the callback at the specified index
                // Adjust the index to be within the bounds of the list
                var newIndex = Mathf.Clamp((int) options.BeginIndex, 0, commandData.BeginCallbacks.Count);
                commandData.BeginCallbacks.Insert(newIndex, beginCallback);
                commandData.Debug_BeginCallbackNames.Insert(newIndex, callerFilePath);
            }

            if (endCallback != null)
            {
                // Insert the callback at the specified index
                // Adjust the index to be within the bounds of the list
                var newIndex = Mathf.Clamp((int) options.EndIndex, 0, commandData.EndCallback.Count);
                commandData.EndCallback.Insert(newIndex, endCallback);
                commandData.Debug_EndCallbackNames.Insert(newIndex, callerFilePath);
            }
        }

        public async UniTask Execute(string script, CancellationToken cancellationToken = default)
        {
            // TODO: set actor ID in the `WriteSo` method to the actor tag
            var commandTree = CommandParser.Parse(script);
            await Execute(commandTree, cancellationToken);
        }
        
        public async Task ExecuteBegin(string commandName, CancellationToken cancellationToken, params string[] args)
        {
            await ExecuteCallbacks(new CommandTag(commandName, args), _commandCallbacks[commandName].BeginCallbacks, cancellationToken);
        }
        
        public async Task ExecuteEnd(string commandName, CancellationToken cancellationToken,params string[] args)
        {
            await ExecuteCallbacks(new CommandTag(commandName, args), _commandCallbacks[commandName].EndCallback, cancellationToken);
        }

        private async UniTask Execute(CommandTag commandTree, CancellationToken cancellationToken)
        {
            _commandCallbacks.TryGetValue(commandTree.name, out var commandData);

            if (commandData != null)
                await ExecuteCallbacks(commandTree, commandData.BeginCallbacks, cancellationToken);

            foreach (var child in commandTree.children)
                await Execute(child, cancellationToken);

            if (commandData != null && commandTree.children.Count > 0)
                await ExecuteCallbacks(commandTree, commandData.EndCallback, cancellationToken);
        }

        private static async UniTask ExecuteCallbacks(CommandTag command, List<CommandCallback> callbacks,
            CancellationToken cancellationToken)
        {
            foreach (var callback in callbacks)
            {
                try
                {
                    await callback.Invoke(command.args, cancellationToken);
                }
                catch (Exception e)
                {
                    if(e is not OperationCanceledException)
                        Debug.LogException(e);
                }
            }
        }


        // TODO: implement. This method should be used to jump to a specific command in the execution tree. i.e., go to label in renpy
        private void GoTo(string commandLabel)
        {
            throw new NotImplementedException();
        }

        private class CommandData
        {
            [NotNull]
            public readonly List<CommandCallback> BeginCallbacks = new();
            public readonly List<string> Debug_BeginCallbackNames = new();
            [NotNull]
            public readonly List<CommandCallback> EndCallback = new();
            public readonly List<string> Debug_EndCallbackNames = new();
        }
    }
}