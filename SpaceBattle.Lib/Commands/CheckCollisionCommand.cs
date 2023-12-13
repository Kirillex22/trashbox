using Hwdtech;
using Hwdtech.Ioc;
using System.Collections;
using Linq;

namespace SpaceBattle.Lib;

class CheckCollisionCommand
{
    private int[] _state;
    private IUObject _obj1;
    private IUObject _obj2;

    public CheckCollisionCommand(IUObject obj1, IUObject obj2)
    {
        _obj1 = obj1;
        _obj2 = obj2;

        var properties = new int[][]() {
            IoC.Resolve<int[]>("Game.UObject.GetProperty", obj1, "Position"), 
            IoC.Resolve<int[]>("Game.UObject.GetProperty", obj1, "Velocity");
            IoC.Resolve<int[]>("Game.UObject.GetProperty", obj2, "Position");
            IoC.Resolve<int[]>("Game.UObject.GetProperty", obj2, "Velocity"); 
        };

        _state = new int[] {
            properties[2][0] - properties[0][0],
            properties[2][1] - properties[0][1],
            properties[3][0] - properties[1][0],
            properties[3][1] - properties[1][1]
        };

    }

    public void Execute()
    {
        var collTree = IoC.Resolve<IDictionary<int, object>>("Game.Struct.CollisionTree");

        _state.ToList().ForEach(parameter => collTree = collTree[parameter]);

        IoC.Resolve<ICommand>("Game.Event.Collision", _objFirst, _objSecond).Execute();   
    }
}
