namespace Doublsb.Dialog
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using Cysharp.Threading.Tasks;

    public interface ICommand : IEnumerable<ICommand>
    {
        ICommand Root { get; set; }
        ICommand Parent { get; set; }
        List<ICommand> Children { get; set; }

        /// <summary>
        /// Returns a collection of the descendant ICommands for this command
        /// </summary>
        IEnumerable<ICommand> Descendants()
        {
            foreach (var child in Children)
            {
                yield return child;
                foreach (var grandChild in child.Descendants())
                    yield return grandChild;
            }
        }

        /// <summary>
        /// Called on opening tag and second time if node had any children
        /// </summary>
        public async UniTask Act(CancellationToken cancellationToken)
        {
            await Begin(cancellationToken);
            foreach (var child in Children)
                await child.Act(cancellationToken);
            if (Children.Count > 0)
                await End(cancellationToken);
        }

        /// <summary>
        /// Called on opening tag
        /// </summary>
        protected UniTask Begin(CancellationToken cancellationToken);

        /// <summary>
        /// Called on closing tag if node had any children
        /// </summary>
        protected UniTask End(CancellationToken cancellationToken) => UniTask.CompletedTask;

        IEnumerator<ICommand> IEnumerable<ICommand>.GetEnumerator()
        {
            return Descendants().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}