using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using ViconDataStreamSDK.CSharp;


namespace UnityVicon
{
  public class SubjectScript : MonoBehaviour
  {
    public string SubjectName = "";
    
    private bool IsScaled = true;

    public ViconDataStreamClient Client;

    public SubjectScript()
    {
    }

    void LateUpdate()
    {
      Output_GetSubjectRootSegmentName OGSRSN = Client.GetSubjectRootSegmentName(SubjectName);
      Transform Root = transform.root;
      FindAndTransform( Root, OGSRSN.SegmentName);
    }

    string strip(string BoneName)
    {
      if (BoneName.Contains(":"))
      {
        string[] results = BoneName.Split(':');
        return results[1];
      }
      return BoneName;
    }
    void FindAndTransform(Transform iTransform, string BoneName )
    {
      int ChildCount = iTransform.childCount;
      for (int i = 0; i < ChildCount; ++i)
      {
        Transform Child = iTransform.GetChild(i);
        if( strip( Child.name) == BoneName )
        { 
          ApplyBoneTransform(Child);
          TransformChildren(Child);
          break;
        }
        // if not finding root in this layer, try the children
        FindAndTransform(Child, BoneName);
      }
    }
    void TransformChildren(Transform iTransform )
    {
      int ChildCount = iTransform.childCount;
      for (int i = 0; i < ChildCount; ++i)
      {
        Transform Child = iTransform.GetChild(i);
        ApplyBoneTransform(Child);
        TransformChildren(Child);
      }
    }
      // map the orientation back for forward

    private void ApplyBoneTransform(Transform Bone )
    {
      string BoneName = strip(Bone.gameObject.name);
      // update the bone transform from the data stream
      Output_GetSegmentLocalRotationQuaternion ORot = Client.GetSegmentRotation(SubjectName, BoneName );
      if (ORot.Result == Result.Success)
      {
        // mapping back to default data stream axis
        //Quaternion Rot = new Quaternion(-(float)ORot.Rotation[2], -(float)ORot.Rotation[0], (float)ORot.Rotation[1], (float)ORot.Rotation[3]);
        Quaternion Rot = new Quaternion((float)ORot.Rotation[0], (float)ORot.Rotation[1], (float)ORot.Rotation[2], (float)ORot.Rotation[3]);
        // mapping right hand to left hand flipping x
        Bone.localRotation = new Quaternion(Rot.x, -Rot.y, -Rot.z, Rot.w);
      }

      Output_GetSegmentLocalTranslation OTran;
      if (IsScaled)
      {
        OTran = Client.GetScaledSegmentTranslation(SubjectName, BoneName);
      }
      else
      {
        OTran = Client.GetSegmentTranslation(SubjectName, BoneName);
      }

      if (OTran.Result == Result.Success)
      {
        //Vector3 Translate = new Vector3(-(float)OTran.Translation[2] * 0.001f, -(float)OTran.Translation[0] * 0.001f, (float)OTran.Translation[1] * 0.001f);
        Vector3 Translate = new Vector3((float)OTran.Translation[0] * 0.001f, (float)OTran.Translation[1] * 0.001f, (float)OTran.Translation[2] * 0.001f);
        Bone.localPosition = new Vector3(-Translate.x, Translate.y, Translate.z);
      }

      // If there's a scale for this subject in the datastream, apply it here.
      if (IsScaled)
      {
        Output_GetSegmentStaticScale OScale = Client.GetSegmentScale(SubjectName, BoneName);
        if (OScale.Result == Result.Success)
        {
          Bone.localScale = new Vector3((float)OScale.Scale[0], (float)OScale.Scale[1], (float)OScale.Scale[2]);
        }
      }
    }
  } //end of program
}// end of namespace

