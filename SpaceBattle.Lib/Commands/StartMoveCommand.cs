using Hwdtech.Ioc;

namespace SpaceBattle.Lib;

public class StartMoveCommand : ICommand
{
    private IOrder _startable;

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

        var cmd = IoC.Resolve<ICommand>("Game.Commands.Move", _startable.Target);

        IoC.Resolve<ICommand>("Game.IUObject.SetProperty", _startable.Target, "Game.Commands.Move", cmd);
        IoC.Resolve<IQueue>("Game.Queue").Push(cmd);
    }  
}
