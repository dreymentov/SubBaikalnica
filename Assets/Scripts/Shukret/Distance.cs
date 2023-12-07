using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Distance : MonoBehaviour
{
	[SerializeField] private Transform player;
	[SerializeField] private TextMeshProUGUI distanceText;
	[SerializeField] private Transform canvas;
	[SerializeField] private Transform cam;

	void Update()
	{
		distanceText.text = ((int)(Vector3.Distance(player.position, transform.position)) * 2).ToString() + "м";
		canvas.LookAt(cam);
	}
}
