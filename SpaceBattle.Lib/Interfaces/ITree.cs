using System.Collections;

namespace SpaceBattle.Lib;

public interface ITree
{
    public Hashtable Tree {get;}

    public void BuildTree(int[,] states);

}
