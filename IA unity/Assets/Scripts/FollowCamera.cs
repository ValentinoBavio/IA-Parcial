using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField]
    private Transform _Target;

    private Vector3 _Offset;

    void Start()
    {
        _Offset = transform.position - _Target.transform.position;
    }

    void LateUpdate()
    {
        transform.position = _Target.transform.position + _Offset;
    }
}
