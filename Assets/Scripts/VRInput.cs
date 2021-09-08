using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;



public class VRInput : BaseInputModule
{
    //from StreamVR Canvas Pointer by VR with Andrew https://github.com/C-Through/VR-CanvasPointer

    public Camera m_Camera;

    public SteamVR_Input_Sources m_TargetSource;
    public SteamVR_Action_Boolean m_ClickAction;

    private PointerEventData m_Data = null;

    protected override void Awake()
    {
        base.Awake();
        m_Data = new PointerEventData(eventSystem);
    }

    public override void Process()
    {
        //Reset data, set camera
        m_Data.Reset();
        m_Data.position = new Vector3(m_Camera.pixelWidth / 2, m_Camera.pixelHeight / 2);

        //Raycast
        eventSystem.RaycastAll(m_Data, m_RaycastResultCache);
        m_Data.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
        m_RaycastResultCache.Clear();

        HandlePointerExitAndEnter(m_Data, m_Data.pointerCurrentRaycast.gameObject);
        

        //press
        if (m_ClickAction.GetStateDown(m_TargetSource))
        {
            ProcessPress(m_Data);
        }
        //release
        if (m_ClickAction.GetStateUp(m_TargetSource))
        {
            ProcessRelease(m_Data);
        }

        ExecuteEvents.Execute(m_Data.pointerDrag, m_Data, ExecuteEvents.dragHandler);
    }

    public PointerEventData GetData()
    {
        return m_Data;
    }

    private void ProcessPress(PointerEventData data)
    {
        /*
        data.pointerPressRaycast = data.pointerCurrentRaycast;
        
        GameObject newPointerPress = ExecuteEvents.ExecuteHierarchy(data.pointerCurrentRaycast.gameObject, data, ExecuteEvents.pointerDownHandler);


        if(newPointerPress == null)
        {
            newPointerPress = ExecuteEvents.GetEventHandler<IPointerClickHandler>(data.pointerCurrentRaycast.gameObject);
        }

        data.pressPosition = data.position;
        data.pointerPress = newPointerPress;
        */

        //data.pointerPress = ExecuteEvents.GetEventHandler<IPointerClickHandler>(data.pointerPressRaycast.gameObject());
        //data.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(data.pointerPressRaycast.gameObject());
        
        data.pointerPressRaycast = data.pointerCurrentRaycast;

        data.pointerPress = ExecuteEvents.GetEventHandler<IPointerClickHandler>(data.pointerPressRaycast.gameObject);
        data.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(data.pointerPressRaycast.gameObject);

        ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.Execute(data.pointerDrag, data, ExecuteEvents.beginDragHandler);
    }

    private void ProcessRelease(PointerEventData data)
    {
        /*
        ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerUpHandler);

        GameObject pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(data.pointerCurrentRaycast.gameObject);

        if(data.pointerPress == pointerUpHandler)
        {
            ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerClickHandler);
        }

        eventSystem.SetSelectedGameObject(null);

        data.pressPosition = Vector2.zero;
        data.pointerPress = null;
        */

        GameObject pointerRelease = ExecuteEvents.GetEventHandler<IPointerClickHandler>(data.pointerCurrentRaycast.gameObject);

        if (data.pointerPress == pointerRelease)
            ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerClickHandler);

        ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerUpHandler);
        ExecuteEvents.Execute(data.pointerDrag, data, ExecuteEvents.endDragHandler);

        data.pointerPress = null;
        data.pointerDrag = null;

        data.pointerCurrentRaycast.Clear();

    }

}
