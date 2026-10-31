using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CloseDoor : MonoBehaviour
{
    [SerializeField] GameObject _door;
    [SerializeField] LayerMask _mask;
    [SerializeField] LayerMask _player;
    int _playerIndex => Mathf.RoundToInt(Mathf.Log(_player.value, 2));
    int _doorIndex => Mathf.RoundToInt(Mathf.Log(_mask.value, 2));

    private void OnTriggerEnter(Collider other)
    {
        if (other.GameObject().layer == _playerIndex)
        {
            _door.layer = 31;
            print("hehe");
        }
    }
}
