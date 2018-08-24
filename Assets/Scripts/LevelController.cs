using UnityEngine.UI;
using UnityEngine;
using System.Collections;
 
public class LevelController: MonoBehaviour
{
    public Transform Panel;
	private Color[] colors = {Color.black, Color.yellow, Color.blue, Color.white};
	public int num = 1;
    public string ButtonName;
    GameObject ButtonController;
    void Start ()
    {       
        ButtonController = GameObject.Find("ButtonController");
        for (num=1; num<=3; num++)
        {
        CreateButton();
        }
    }
     
    void Update ()
    {
        	ButtonName=ButtonController.GetComponent<ButtonController>().objectName ;
    }
 
    void CreateButton ()
    {
        GameObject newButton = new GameObject(""+num, typeof(Image), typeof(Button), typeof(LayoutElement));
        newButton.transform.SetParent(Panel);
        newButton.GetComponent<LayoutElement>().minHeight = 35;
        newButton.AddComponent<ButtonController>();
    }
}