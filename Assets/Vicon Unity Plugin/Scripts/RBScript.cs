using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using ViconDataStreamSDK.CSharp;


namespace UnityVicon
{
  public class RBScript : MonoBehaviour
  {
    public string ObjectName = "";
    public ViconDataStreamClient Client;

    private Quaternion m_LastGoodRotation;
    private Vector3 m_LastGoodPosition;
    private bool m_bHasCachedPose = false;

    public RBScript()
    {
    }

    void Update()
    {

      Output_GetSubjectRootSegmentName OGSRSN = Client.GetSubjectRootSegmentName(ObjectName );
      string SegRootName = OGSRSN.SegmentName;

      // UNITY-49 - Don't apply root motion to parent object
      Transform Root = transform;
      if (Root == null)
      {
        throw new Exception( "fbx doesn't have root");
      }

      Output_GetSegmentLocalRotationQuaternion ORot = Client.GetSegmentRotation( ObjectName, SegRootName);
      Output_GetSegmentLocalTranslation OTran = Client.GetSegmentTranslation(ObjectName, SegRootName);
      if (ORot.Result == Result.Success && OTran.Result == Result.Success && !OTran.Occluded )
      {
        // Input data is in Vicon co-ordinate space; z-up, x-forward, rhs.
        // We need it in Unity space, y-up, z-forward lhs
        //           Vicon Unity
        // forward    x     z
        // up         z     y
        // right     -y     x
        // See https://gamedev.stackexchange.com/questions/157946/converting-a-quaternion-in-a-right-to-left-handed-coordinate-system

        Root.localRotation = new Quaternion((float)ORot.Rotation[1], -(float)ORot.Rotation[2], -(float)ORot.Rotation[0], (float)ORot.Rotation[3]);
        Root.localPosition = new Vector3(-(float)OTran.Translation[1] * 0.001f, (float)OTran.Translation[2] * 0.001f, (float)OTran.Translation[0] * 0.001f);

        m_LastGoodPosition = Root.localPosition;
        m_LastGoodRotation = Root.localRotation;
        m_bHasCachedPose = true;
      }
      else
      {
        if( m_bHasCachedPose )
        {
          Root.localRotation = m_LastGoodRotation;
          Root.localPosition = m_LastGoodPosition;
        }
      }

    }
  } //end of program
}// end of namespace

