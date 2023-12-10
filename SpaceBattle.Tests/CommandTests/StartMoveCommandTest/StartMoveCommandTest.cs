using Hwdtech;
using Hwdtech.Ioc;
using SpaceBattle.Lib;

namespace SpaceBattle.Tests;

public class StartMoveCommandTest
{
    private IQueue queue = new Mock<ICommand>().Object;
    private Queue realQueue = new Queue<ICommand>();

    public StartMoveCommandTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();   

        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
             IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Game.IUObject.SetProperty",
            (object[] args) =>
            {
                var target = (IUObject)args[0];
                var key = (string)args[1];
                var value = args[2];

                target.SetProperty(key, value);
                return new object();
            }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Game.Command.Move",
            (object[] args) =>
            {
                return new MoveCommand(new Mock<IMovable>().Object);
            }
        ).Execute();

        queue.Setup(q => q.Push(It.IsAny<ICommand>())).Callback(realQueue.Enqueue);

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Game.Queue",
            (object[] args) =>
            {
                return queue;
            }
        ).Execute();
    }

    [Fact]
    public void SuccefulExecuting()
    {        
        var startable = new Mock<IMoveStartable>();
        var target = new Mock<IUObject>();
        var initialValues = new Dictionary<string, object> {, 
            {"position", new object()},
            {"command", "Move"}
        };

        var settedValues = new Dictionary<string, object>();

        startable.SetupGet(s => s.InitialValues).Returns(initialValues);
        startable.SetupGet(s => s.Target).Returns(target.Object);

        target.Setup(
            t => t.SetProperty(
                It.IsAny<string>(),
                It.IsAny<object>()
                )
        ).Callback<string, object>(settedValues.Add);

        var smc = new StartMoveCommand(startable.Object);  

        smc.Execute();

        Assert.True(settedValues.Contains("position") && settedValues.Contains("Move") && realQueue.TryDequeue());
    }

    [Fact]
    public void InitialValuesSetException()
    {
        var startable = new Mock<IMoveStartable>();
        var target = new Mock<IUObject>();
        var initialValues = new Dictionary<string, object> {
            {"velocity", new object()},
            {"command", "move"}
        };
        var settedValues = new Dictionary<string, object>();

        startable.SetupGet(s => s.InitialValues).Returns(initialValues);
        startable.SetupGet(s => s.Target).Returns(order.Object);

        target.Setup(
            t => t.SetProperty(
                It.IsAny<string>(),
                It.IsAny<object>()
                )
        ).Callback(() => throw new Exception());

        var smc = new StartMoveCommand(startable.Object);

        Assert.Throws<Exception>(() => startMoveCommand.Execute()); 
    }

    /*[Fact]
    public void StartMoveCommand_CantReadPropertiesOfOrder_FromStartable()
    {
        var startable = new Mock<IMoveStartable>();
        startable.SetupGet(s => s.PropertiesOfOrder).Throws(new NotImplementedException());
        var startMoveCommand = new StartMoveCommand(startable.Object);

        Assert.Throws<NotImplementedException>(startMoveCommand.Execute);
    }

    [Fact]
    public void StartMoveCommand_CantReadOrder_FromStartable()
    {
        var startable = new Mock<IMoveStartable>();
        startable.SetupGet(s => s.Order).Throws(new NotImplementedException());
        var properties = new Dictionary<string, object> {
            { "position", new Vector( new int[] { 2, 1 }) },
        };

        startable.SetupGet(s => s.PropertiesOfOrder).Returns(properties);
        var startMoveCommand = new StartMoveCommand(startable.Object);

        Assert.Throws<NotImplementedException>(startMoveCommand.Execute);
    }*/
}