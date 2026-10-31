using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CloseDoor : MonoBehaviour
{
    [SerializeField] GameObject _door;
    [SerializeField] LayerMask _mask;
    [SerializeField] LayerMask _player;
    [SerializeField] LayerMask _playerInv;
    int _playerIndex => Mathf.RoundToInt(Mathf.Log(_player.value, 2));
    int _playerInvIndex => Mathf.RoundToInt(Mathf.Log(_playerInv.value, 2));

    private void OnTriggerEnter(Collider other)
    {
        if (other.GameObject().layer == _playerIndex || other.GameObject().layer == _playerInvIndex)
        {
            _door.layer = 31;
            print("hehe");
        }
    }
}
