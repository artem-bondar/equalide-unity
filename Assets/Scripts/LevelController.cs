using UnityEngine.UI;
using UnityEngine;
using System.Collections;
 
public class LevelController: MonoBehaviour
{
    public Transform canvas;
    public Font font;
	private Color[] colors = {Color.black, Color.yellow, Color.blue, Color.white};
	public int num = 1;
    void Start ()
    {
        CreateButton();

    }
     
    void Update ()
    {
     
    }
 
    void CreateButton ()
    {
        GameObject newButton = new GameObject("New button", typeof(Image), typeof(Button), typeof(LayoutElement));
        newButton.transform.SetParent(canvas);
        newButton.GetComponent<LayoutElement>().minHeight = 35;
        newButton.GetComponent<Button>().onClick.AddListener(delegate { press(); });
    }
	
	public void press ()
    {
        
    }
}