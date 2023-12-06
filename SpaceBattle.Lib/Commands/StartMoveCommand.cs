using Hwdtech.Ioc;

namespace SpaceBattle.Lib;

public class StartMoveCommand : ICommand
{
    private IOrder _order;

    public StartMoveCommand(IOrder order)
    {
       _order = order;
    }

    public void Execute()
    {
        _order.InitialValues.ToList().ForEach(value => IoC.Resolve<object>(
            "Game.IUObject.SetProperty",
            _order.Target,
            value.Key,
            value.Value
        ));

        var cmd = IoC.Resolve<ICommand>("Game.Operations.Movement", _order.Target);

        IoC.Resolve<ICommand>("IUObject.Property.Set", _order.Target, "Movement", cmd);
        IoC.Resolve<IQueue>("Game.Queue").Push(cmd);
    }  
}
