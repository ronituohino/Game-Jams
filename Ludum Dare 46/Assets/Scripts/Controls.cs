using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : Singleton<Controls>
{
    public Camera cam;

    public Transform connectionsParent;
    public GameObject connection;

    GameObject connectionInstance;
    LineRenderer connectionLineRenderer;
    EdgeCollider2D connectionCollider;

    bool validConnectionStart = false;
    bool hoveringValid = false;

    bool holdingMouse = false;

    Node from;
    Node to;

    Node previousHoveredNode;

    public int effectIterations;

    bool onSource = false;
    public float dragThreshold;
    Vector2 originalMousePos;

    

    void Update()
    {
        if(!LevelManager.Instance.simulating)
        {
            Ray r = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(r, Mathf.Infinity);

            //Hovering effect
            Node n1 = SearchForNode(hits);

            //We moved off from a node
            if ((n1 is null) && !(previousHoveredNode is null))
            {
                if (!holdingMouse)
                {
                    previousHoveredNode.inOrOut = false;
                    previousHoveredNode = null;
                }
                else
                {
                    if (!(to is null))
                    {
                        to.inOrOut = false;
                    }
                }
            }
            //We moved to a new node
            if (!(n1 is null) && previousHoveredNode != n1)
            {
                if (!holdingMouse)
                {
                    previousHoveredNode = n1;
                    if (!previousHoveredNode.goalNode)
                    {
                        previousHoveredNode.inOrOut = true;
                    }
                }
                else
                {
                    if (hoveringValid)
                    {
                        n1.inOrOut = true;
                    }
                }
            }

            //On initial mouse click find a starting node
            if (Input.GetMouseButtonDown(0))
            {
                if (!(n1 is null))
                {
                    if (!n1.goalNode)
                    {
                        //Node
                        from = n1;
                        validConnectionStart = true;

                        //Connection
                        connectionInstance = Instantiate(connection, connectionsParent);

                        connectionLineRenderer = connectionInstance.GetComponent<LineRenderer>();
                        connectionLineRenderer.positionCount = 2;
                        connectionLineRenderer.SetPosition(0, previousHoveredNode.transform.position);

                        Material m = connectionLineRenderer.material;
                        m.SetFloat("_Fade", 1.5f);
                        m.SetFloat("_Glow", 0.3f);

                        connectionCollider = connectionInstance.GetComponent<EdgeCollider2D>();
                    }
                }
                else
                {
                    //On source?
                    foreach (RaycastHit2D hit in hits)
                    {
                        if (hit.collider.tag == "Source")
                        {
                            onSource = true;
                            originalMousePos = Input.mousePosition;
                            break;
                        }
                    }
                }
            }
            //Dragging mouse, search for other nodes to connect to
            if (Input.GetMouseButton(0))
            {
                holdingMouse = true;

                if (validConnectionStart)
                {
                    Node n = SearchForNode(hits);

                    Vector2 pos = GetCollisionPlanePosition(hits);
                    connectionLineRenderer.SetPosition(1, pos);

                    hoveringValid = n != from && !(n is null) && !n.goalNode && !ConnectionExists(from, n) && !CollidingWithWalls(from.transform.position, pos);

                    if (hoveringValid)
                    {
                        //We found another node, that still has space and that we aren't already connected to
                        to = n;
                        connectionLineRenderer.SetPosition(1, to.transform.position);
                    }
                    else
                    {
                        to = null;
                    }

                }
            }
            else
            {
                holdingMouse = false;
            }
            //On mouse release, connect nodes
            if (Input.GetMouseButtonUp(0))
            {
                if (validConnectionStart)
                {
                    from.inOrOut = false;
                    StartCoroutine(ApplyEffectToObject(connectionInstance, false, GlowEffectOut, false, 0.6f));

                    if (hoveringValid)
                    {
                        CreateConnection(from, to);
                    }
                    else
                    {
                        StartCoroutine(ApplyEffectToObject(connectionInstance, false, FadeEffectOut, true, 0.6f));
                    }
                }
                else
                {
                    if (onSource)
                    {
                        Vector2 mousePos = Input.mousePosition;
                        if ((mousePos - originalMousePos).magnitude < dragThreshold)
                        {
                            LevelManager.Instance.Launch();
                        }
                    }
                }

                ResetVariables();
            }

            //Right click to remove connections
            if (Input.GetMouseButton(1))
            {
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider.tag == "Connection")
                    {
                        RemoveConnection(hit.collider.gameObject);
                    }
                }
            }
        }
    }

    public void RemoveConnection(GameObject g)
    {
        Connection c = g.GetComponent<Connection>();
        c.a.connections.Remove(c);
        c.b.connections.Remove(c);

        StartCoroutine(ApplyEffectToObject(g, false, FadeEffectOut, true, 0.6f));
    }

    void ResetVariables()
    {
        connectionInstance = null;
        connectionCollider = null;
        connectionLineRenderer = null;

        validConnectionStart = false;

        from = null;
        to = null;

        originalMousePos = Vector2.zero;
        onSource = false;
    }

    Node SearchForNode(RaycastHit2D[] hits)
    {
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.tag == "Node")
            {
                Node n = hit.collider.GetComponent<Node>();
                return n;
            }
        }

        return null;
    }

    Vector2 GetCollisionPlanePosition(RaycastHit2D[] hits)
    {
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.name == "CollisionPlane")
            {
                return hit.point;
            }
        }

        return Vector2.zero;
    }

    void CreateConnection(Node from, Node to)
    {
        Connection c = connectionInstance.AddComponent<Connection>();
        c.a = from;
        c.b = to;

        int aConnectionCount = from.connections.Count;
        from.connections.Add(c);

        int bConnectionCount = to.connections.Count;
        to.connections.Add(c);

        //Connection object
        connectionLineRenderer.SetPosition(1, to.transform.position);

        //Collider
        connectionCollider.points = new Vector2[] { from.transform.position, to.transform.position };
        connectionInstance.tag = "Connection";
    }



    bool ConnectionExists(Node a, Node b)
    {
        foreach (Connection c in a.connections)
        {
            if ((c.a != a && c.a == b) || (c.b != a && c.b == b))
            {
                return true;
            }
        }

        return false;
    }

    bool CollidingWithWalls(Vector2 a, Vector2 b)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(a, b - a, (b-a).magnitude);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.tag == "Wall")
            {
                return true;
            }
        }
        return false;
    }



    public IEnumerator ApplyEffectToObject(GameObject g, bool spriteRenderer, Func<Material, float, bool> function, bool destroyObject, float time, Triangle t = null)
    {
        Material m;
        if (spriteRenderer)
        {
            m = g.GetComponent<SpriteRenderer>().material;
        }
        else
        {
            LineRenderer lr = g.GetComponent<LineRenderer>();
            m = lr.material;

            Vector3 pos1 = lr.GetPosition(0);
            Vector3 pos2 = lr.GetPosition(1);

            float xLen = Mathf.Abs(pos1.x - pos2.x) + 1;
            float yLen = Mathf.Abs(pos1.y - pos2.y) + 1;

            m.SetVector("_TexScale", new Vector2(xLen, yLen));
            m.SetFloat("_Scale", xLen + yLen / 2);
        }

        for(float i = 0; i < effectIterations; i++)
        {
            function(m, i);
            yield return new WaitForSeconds(time / effectIterations);
        }

        if(destroyObject)
        {
            Destroy(g);
        }

        if(!(t is null))
        {
            t.lerp = 0.2f;
        }
    } 

    public bool FadeEffectOut(Material m, float i)
    {
        m.SetFloat("_Fade", i.Remap(0, effectIterations, 1.5f, -0.5f));
        return true;
    }

    public bool FadeEffectIn(Material m, float i)
    {
        m.SetFloat("_Fade", i.Remap(0, effectIterations, -0.5f, 1.5f));
        return true;
    }

    public bool GlowEffectOut(Material m, float i)
    {
        m.SetFloat("_Glow", i.Remap(0, effectIterations, 0.3f, 0f));
        return true;
    }
}
