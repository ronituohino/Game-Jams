using UnityEngine;

public class Connection : MonoBehaviour
{
    public Node a;
    public Node b;

    public Connection(Node a, Node b)
    {
        this.a = a;
        this.b = b;
    }
}