using System.Linq;

using UnityEngine;
public class MouseRaycast : MonoBehaviour {
    private Ray _ray;

    private void Update() {
        if (Input.anyKeyDown) {
            _ray = Globals.Instance.MainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(_ray, out hit, 50, 1 << LayerMask.NameToLayer("UI"))) {
                Debug.Log(hit.collider.name);
            }    
        }
    }
}