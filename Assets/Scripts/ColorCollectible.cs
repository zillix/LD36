using UnityEngine;
using System.Collections;

public class ColorCollectible : MonoBehaviour
{

	public ColorType colorType;

	void Start () {
	
	}
	
	void Update () {
	
	}
}

public enum ColorType
{
	Red,
	Green,
	Blue
}
