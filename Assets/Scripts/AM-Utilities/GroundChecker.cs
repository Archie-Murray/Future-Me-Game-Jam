using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Utilities {
    public class GroundChecker : MonoBehaviour {
        [SerializeField] private float _distanceToGround;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private Transform _checkPoint;
        
        private void Start() {
            if (!_checkPoint) {
                Debug.LogWarning("Could not find ground check point");
                Destroy(this);
            }
        }

        public bool IsGrounded => Physics.SphereCast(_checkPoint.position, _distanceToGround, Vector3.down, out _, _distanceToGround, _groundLayer);
    }
}
