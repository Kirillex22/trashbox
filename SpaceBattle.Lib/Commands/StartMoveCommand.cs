using Hwdtech.Ioc;

namespace SpaceBattle.Lib;

public class StartMoveCommand : ICommand
{
    private IMoveStartable _startable;

    public StartMoveCommand(IMoveStartable startable)
    {
       _startable = startable;
    }

    public void Execute()
    {
        _startable.InitialValues.ToList().ForEach(value => IoC.Resolve<object>(
            "Game.IUObject.SetProperty",
            _startable.Target,
            value.Key,
            value.Value
        ));

        var cmd = IoC.Resolve<ICommand>("Game.Command.LongMove", _startable.Target);

        IoC.Resolve<ICommand>("Game.IUObject.SetProperty", _startable.Target, "command", cmd);
        IoC.Resolve<IQueue>("Game.Queue").Push(cmd);
    }  
}
