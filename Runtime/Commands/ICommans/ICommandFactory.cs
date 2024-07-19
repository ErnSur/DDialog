namespace Doublsb.Dialog
{
    using System.Collections.Generic;

    public interface ICommandFactory
    {
        ICommand CreateCommandTree(CommandTag rootTag)
        {
            if(!TryGetCommand(rootTag.name, rootTag.args, out var root))
                return null;

            root.Children = new List<ICommand>();
            foreach (var tagChild in rootTag.children)
            {
                var child = CreateCommandTree(tagChild);
                if (child == null)
                    continue;
                child.Parent = root;
                root.Children.Add(child);
            }

            return root;
        }

        protected bool TryGetCommand(string commandId, string[] args,  out ICommand command);
    }
}