namespace Doublsb.Dialog
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using JetBrains.Annotations;
    using UnityEngine;

    public delegate UniTask CommandCallback(string[] args, CancellationToken cancellationToken);

    public struct CommandCallbackOptions
    {
        // TODO: `BeginAfter` and `EndAfter` methods
        // public static CommandCallbackOptions BeginBefore(Type type) => new CommandCallbackOptions
        // {
        // };
        public float BeginIndex { get; set; }
        public float EndIndex { get; set; }
        public CommandCallbackOptions(float beginIndex = 0, float endIndex = 0)
        {
            BeginIndex = beginIndex;
            EndIndex = endIndex;
        }
    }
    
    public class CommandRunner
    {
        private readonly Dictionary<string, CommandData> _commandCallbacks = new();

        // TODO: Add support for floating point indexing
        /// <param name="commandName"> The name of the command to register the callback for. </param>
        /// <param name="beginCallback"> The callback to be executed when the command is encountered. </param>
        /// <param name="endCallback"> The callback to be executed when the command is finished. </param>
        /// <param name="options"> The options for the callback. </param>
        public void RegisterCommandCallback([NotNull] string commandName, [CanBeNull] CommandCallback beginCallback,
            [CanBeNull] CommandCallback endCallback = null, CommandCallbackOptions options = default)
        {
            if (!_commandCallbacks.TryGetValue(commandName, out var commandData))
            {
                _commandCallbacks[commandName] = commandData = new CommandData();
            }

            if (beginCallback != null)
            {
                // Insert the callback at the specified index
                // Adjust the index to be within the bounds of the list
                var newIndex = Mathf.Clamp((int) options.BeginIndex, 0, commandData.BeginCallbacks.Count);
                commandData.BeginCallbacks.Insert(newIndex, beginCallback);
            }

            if (endCallback != null)
            {
                // Insert the callback at the specified index
                // Adjust the index to be within the bounds of the list
                var newIndex = Mathf.Clamp((int) options.EndIndex, 0, commandData.EndCallback.Count);
                commandData.EndCallback.Insert(newIndex, endCallback);
            }
        }

        public async UniTask Execute(string script, CancellationToken cancellationToken = default)
        {
            // TODO: set actor ID in the `WriteSo` method to the actor tag
            var commandTree = CommandParser.Parse(script);
            await Execute(commandTree, cancellationToken);
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
                    Debug.LogError(e);
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
            [NotNull]
            public readonly List<CommandCallback> EndCallback = new();
        }
    }
}