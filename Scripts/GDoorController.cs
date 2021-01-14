using Pathfinding;
using UnityEngine;
using GameCreator.Variables;

public class GDoorController : MonoBehaviour
{
    private bool open = false;
    public GworldDoor currentDoor = null;
    public int opentag = 1;
    public int closedtag = 1;
    public bool updateGraphsWithGUO = true;
    public float yOffset = 5;

    Bounds bounds;

    public void Start()
    {
        // Capture the bounds of the collider while it is closed
        bounds = GetComponent<Collider>().bounds;

        // Initially open the door
        SetState(open);
    }

    void OnGUI()
    {
        // Show a UI button for opening and closing the door
        if (GUI.Button(new Rect(5, yOffset, 100, 22), "Toggle Agent Door"))
        {
            SetState(!open);
        }
    }

    public void SetState(bool open)
    {
        this.open = open;
        VariablesManager.SetLocal(gameObject, "DoorOpen", open, true);

        if(currentDoor != null)
        {
            currentDoor.ObjectFactState = open;
        }

        if (updateGraphsWithGUO)
        {
            // Update the graph below the door
            // Set the tag of the nodes below the door
            // To something indicating that the door is open or closed
            GraphUpdateObject guo = new GraphUpdateObject(bounds);
            int tag = open ? opentag : closedtag;

            // There are only 32 tags
            if (tag > 31) { Debug.LogError("tag > 31"); return; }

            guo.modifyTag = true;
            guo.setTag = tag;
            guo.updatePhysics = false;

            AstarPath.active.UpdateGraphs(guo);
        }

       
    }

    public bool DoorState()
    {
        return open;
    }
}

