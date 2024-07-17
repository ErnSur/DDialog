namespace Doublsb.Dialog
{
    using System.Collections.Generic;
    using System.Xml.Linq;

    public interface ICommandFactory
    {
        List<Command> GetCommands(XElement commandTree);
    }
}