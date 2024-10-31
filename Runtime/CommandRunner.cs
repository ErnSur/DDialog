namespace QuickEye.PeeDialog
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using UnityEngine;

    // TODO: Maybe command text content should be passed here (use-case: note command handler can easily access the note text)
    // TODO: Should sync commands be supported?
    public delegate Task CommandCallback(string[] args, CancellationToken cancellationToken);

    public interface ICommandHandler
    {
        Task OnBegin(string[] args, CancellationToken cancellationToken);
        Task OnEnd(string[] args, CancellationToken cancellationToken);
    }
    
    // Other names could be:
    // - ScriptRunner
    // - ScriptExecutor
    // - ScriptInterpreter
    public class CommandRunner
    {
        private readonly Dictionary<string, CommandData> _commandCallbacks = new();
        
        private CancellationTokenSource _cancellationTokenSource;

        public void CancelExecution() 
        {
            _cancellationTokenSource?.Cancel();
        }
        
        public void RegisterCommandHandler([NotNull] string commandName, [NotNull] ICommandHandler handler)
        {
            RegisterCommandCallback(commandName, handler.OnBegin, handler.OnEnd);
        }

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

        public void AddAlias(string alias, string commandId, params string[] commandArgs)
        {
            RegisterCommandCallback(alias, BeginCommand, EndCommand);
            return;

            async Task BeginCommand(string[] _, CancellationToken cancellationToken)
            {
                await ExecuteBegin(commandId, cancellationToken, commandArgs);
            }

            async Task EndCommand(string[] _, CancellationToken cancellationToken)
            {
                await ExecuteEnd(commandId, cancellationToken);
            }
        }

        public async Awaitable Execute(string script, CancellationToken cancellationToken = default)
        {
            using(_cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                var commandTree = CommandParser.Parse(script);
                //Debug.Log($"Executing command tree: {JsonUtility.ToJson(commandTree,true)}");
                await Execute(commandTree, _cancellationTokenSource.Token);
            }
        }

        public async Task ExecuteBegin(string commandName, CancellationToken cancellationToken, params string[] args)
        {
            await ExecuteCallbacks(new CommandTag(commandName, args), _commandCallbacks[commandName].BeginCallbacks, cancellationToken);
        }

        public async Task ExecuteEnd(string commandName, CancellationToken cancellationToken,params string[] args)
        {
            await ExecuteCallbacks(new CommandTag(commandName, args), _commandCallbacks[commandName].EndCallback, cancellationToken);
        }

        private async Awaitable ExecuteTest()
        {
            await Awaitable.WaitForSecondsAsync(4);
        }
        private async Awaitable Execute(CommandTag commandTree, CancellationToken cancellationToken)
        {
            _commandCallbacks.TryGetValue(commandTree.name, out var commandData);

            if (commandData != null)
                await ExecuteCallbacks(commandTree, commandData.BeginCallbacks, cancellationToken);

            foreach (var child in commandTree.children)
                await Execute(child, cancellationToken);

            if (commandData != null && commandTree.children.Count > 0)
                await ExecuteCallbacks(commandTree, commandData.EndCallback, cancellationToken);
        }

        private static async Awaitable ExecuteCallbacks(CommandTag command, List<CommandCallback> callbacks,
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