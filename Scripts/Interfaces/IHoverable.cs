using UnityEngine;
using System.Collections;

public interface IHoverable {
	
	void DisableHover ();

	IEnumerator HoverTimer();
}
