using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private int[,] _fieldMatrix =
    {
        {0, 1, 1},
        {2, 2, 0}
    };

    private Color[] colors = {Color.black, Color.yellow, Color.blue, Color.white};

    public GameObject SquarePfb;

    public GameObject[,] Field { get; private set; }

    // Use this for initialization
    void Start()
    {
        Debug.Log("Hello!");
        FieldFill();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FieldFill()
    {
        Field = new GameObject[_fieldMatrix.GetLength(0), _fieldMatrix.GetLength(1)];
        for (int i = 0; i < _fieldMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < _fieldMatrix.GetLength(1); j++)
            {
                Field[i, j] = Instantiate(SquarePfb);
                Field[i, j].transform.position = new Vector3(j, _fieldMatrix.GetLength(0) - 1 - i, 0);
            }
        }

        Draw();
    }

    void Draw()
    {
        for (int i = 0; i < _fieldMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < _fieldMatrix.GetLength(1); j++)
            {
                Field[i, j].GetComponent<Renderer>().material.color = colors[_fieldMatrix[i, j]];
            }
        }
    }
}