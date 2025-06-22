using UnityEngine;

public class CubeInputHandler : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private CubeSpawner _spawner;

    private void Awake()
    {
        if (_mainCamera == null)
            _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            HandleClick();
    }

    private void HandleClick()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.TryGetComponent(out ExplodableCube cube))
            {
                _spawner.HandleCubeClick(cube);
            }
        }
    }
}