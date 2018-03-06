using UnityEngine;
using System.Collections;

public class DisplayMenu {

    string[] labels;

    public DisplayMenu(string[] items)
    {
        labels = items;
       
    }

    public void createTextMenu(GameObject parent, Color textColor, Color backgroundColor)
    {

        int k = 0;
        foreach (string item in labels)
        {
            //Make quad 
            GameObject TextObject = new GameObject(item);
            //GameObject BackGround = GameObject.CreatePrimitive(PrimitiveType.Quad);
            //BackGround.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            TextObject.AddComponent<TextMesh>();
            TextMesh tm = TextObject.GetComponent<TextMesh>();
            tm.text = item;
            TextObject.transform.position = new Vector3(0f, k, 0f);
            TextObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            tm.fontSize = 108;
            tm.color = textColor;
            TextObject.AddComponent<BoxCollider>();
            //TextObject.AddComponent<GUIEvents>();

            TextObject.transform.parent = parent.transform;
            //BackGround.transform.parent = parent.transform;
            k++;
        }

    }

    public void createSnaxes(float scale)
    {
        float k = 0;
        foreach (string item in labels)
        {
            //Make quad 
            GameObject snaxHost = GameObject.CreatePrimitive(PrimitiveType.Cube);
            snaxHost.transform.position = new Vector3(scale*3f/2f, k*1.5f, 0f);
            snaxHost.transform.localScale = new Vector3(scale*3f, scale, scale);
            snaxHost.GetComponent<Renderer>().material.color = Color.gray;

            //snaxHost.AddComponent<SnaxsCollisions>();
            //snaxHost.AddComponent<Rigidbody>().useGravity = false;
            //snaxHost.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

            GameObject TextObject = new GameObject(item);
            //GameObject BackGround = GameObject.CreatePrimitive(PrimitiveType.Quad);
            //BackGround.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            TextObject.AddComponent<TextMesh>();
            TextMesh tm = TextObject.GetComponent<TextMesh>();
            tm.text = item;
            TextObject.transform.localPosition = new Vector3(0f, k*1.5f, 0f);
            TextObject.transform.localScale = new Vector3(scale * 0.1f, scale * 0.1f, scale * 0.1f);
            tm.fontSize = 50;
            tm.color = Color.black;

            TextObject.transform.parent = snaxHost.transform;

            //TextObject.AddComponent<GUIEvents>();
            k += scale;
        }
    }

}
