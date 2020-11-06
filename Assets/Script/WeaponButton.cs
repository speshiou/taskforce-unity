using UnityEngine;
using System.Collections;

public class WeaponButton : MonoBehaviour {

	public Weapon weapon;
	public GameObject mask;
	public TextMesh textCount;
	public TextMesh textTotal;

	// Use this for initialization
	void Start () {
		//zIndexOfUnEquipped = mask.transform.position.z;
	}
	
	// Update is called once per frame
	void Update () {
		if(textCount != null)
			textCount.text = weapon.numberOfProjectile.ToString();
		if(textTotal != null) {
			textTotal.text = weapon.totalNumberOfProjectile == -1 ? "999" : weapon.totalNumberOfProjectile.ToString();
			Color color = Color.white;
			if(weapon.totalNumberOfProjectile == -1)
				color = Color.yellow;
			else if(weapon.totalNumberOfProjectile == 0)
				color = Color.red;
			textTotal.renderer.material.color = color;
		}
	}
	
	public void setEquipped(bool enable) {
		mask.SetActive (!enable);
	}
}
