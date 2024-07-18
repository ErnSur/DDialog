namespace Doublsb.Dialog
{
    using System.Collections.Generic;

    public interface ICommandFactory
    {
        ICommand CreateCommandTree(CommandTag rootTag)
        {
            return CreateCommandTree(rootTag, null);
        }
        
        private ICommand CreateCommandTree(CommandTag rootTag, ICommand parent)
        {
            if(!TryGetCommand(rootTag.name, rootTag.args, parent, out var root))
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

        protected bool TryGetCommand(string commandId, string[] args, ICommand parent, out ICommand command);
    }
}