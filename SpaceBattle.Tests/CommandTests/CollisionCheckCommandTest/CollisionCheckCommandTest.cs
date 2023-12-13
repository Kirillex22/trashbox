using Hwdtech;
using Hwdtech.Ioc;
using SpaceBattle.Lib;
using System.Collections;
using Moq;

namespace SpaceBattle.Tests;

public class CollisionCheckCommandTest
{
    public CollisionCheckCommandTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();   

        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
             IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        var concreteTree = new Hashtable(){
                {0, new Hashtable(){
                    {0, new Hashtable(){
                        {0, new Hashtable(){
                            {1, new Hashtable()}
                    }
                }
                }
            }
            }
        }
        };

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Game.Struct.CollisionTree",
            (object[] args) => concreteTree
        ).Execute();
    }

    [Fact]
    public void SuccefulExecutingWithCollision()
    { 
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Game.UObject.GetProperty",
            (object[] args) => new int[] {1, 1}
        ).Execute();
        
        var collisionCommand = new Mock<SpaceBattle.Lib.ICommand>();
        collisionCommand.Setup(c => c.Execute()).Verifiable("collisionCommand wasn't called");

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Game.Event.Collision",
            (object[] args) => collisionCommand.Object
        ).Execute();

        var obj1 = new Mock<IUObject>().Object;
        var obj2 = new Mock<IUObject>().Object;

        var chm = new CheckCollisionCommand(obj1, obj2);

        chm.Execute();

        collisionCommand.VerifyAll();       
    }

}
