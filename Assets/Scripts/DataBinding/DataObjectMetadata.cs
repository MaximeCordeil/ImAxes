using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[CreateAssetMenuAttribute(menuName="ImAxes/Data Object Metadata")]
public class DataObjectMetadata : ScriptableObject {

    [Serializable]
    public struct BinSize {
        public int index;
        public int binCount;
    }

    [Tooltip("Override a particular dimension's bin size")]    
    public List<BinSize> BinSizePreset = new List<BinSize>();

    //[Tooltip("index of the dimension that will categorize the color")]
    //public int dimensionColor = 0;

    //[Tooltip("index of the dimension that will link the datapoints")]
    //public int dimensionLinking = -1;
}
