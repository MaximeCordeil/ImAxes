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

    [Tooltip("Set the column name of the suburb or postcode column for geocoding, empty string = no column exists")]
    public string GeocodeIdentifier = "";

    [Tooltip("Adds a suffix after each suburb or postcode data entry e.g. \" Australia\" will make the geocoder to only consider Australia")]
    public string GeocodeSuffix = " Australia";

    //[Tooltip("index of the dimension that will categorize the color")]
    //public int dimensionColor = 0;

    //[Tooltip("index of the dimension that will link the datapoints")]
    //public int dimensionLinking = -1;
}
