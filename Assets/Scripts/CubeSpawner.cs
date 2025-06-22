using UnityEngine;

[RequireComponent(typeof(CubeVisuals))]
public class CubeSpawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _minSpawnCount = 2;
    [SerializeField] private int _maxSpawnCount = 6;
    [SerializeField] private ExplodableCube _cubePrefab;
    [SerializeField] private float _explosionForce = 20f;
    [SerializeField] private float _explosionRadius = 10f;

    private CubeVisuals _cubeVisuals;

    private void Awake()
    {
        _cubeVisuals = GetComponent<CubeVisuals>();
    }

    public void SpawnInitialCube()
    {
        var cube = Instantiate(_cubePrefab, Vector3.zero, Quaternion.identity);
        cube.Initialize(1f, _cubeVisuals.GetRandomColor());
    }

    public void HandleCubeClick(ExplodableCube cube)
    {
        if (cube == null) 
            return;

        Vector3 explosionCenter = cube.transform.position;

        if (cube.ShouldSplit())
        {
            var newCubes = SpawnChildCubes(explosionCenter, cube.transform.localScale, cube.SplitChance, cube.CubeColor);
            ApplyExplosionToCubes(newCubes, explosionCenter);
        }
        else
        {
            ApplyExplosionToNearbyCubes(cube, explosionCenter);
        }

        Destroy(cube.gameObject);
    }

    private ExplodableCube[] SpawnChildCubes(Vector3 center, Vector3 parentScale, float parentSplitChance, Color parentColor)
    {
        int count = Random.Range(_minSpawnCount, _maxSpawnCount + 1);
        var newCubes = new ExplodableCube[count];

        for (int i = 0; i < count; i++)
        {
            newCubes[i] = Instantiate(_cubePrefab, center, Random.rotation);
            newCubes[i].transform.localScale = parentScale * 0.5f;
            newCubes[i].Initialize(parentSplitChance * 0.5f, _cubeVisuals.GetVariedColor(parentColor));
        }

        return newCubes;
    }

    private void ApplyExplosionToCubes(ExplodableCube[] cubes, Vector3 explosionCenter)
    {
        if (cubes == null) 
            return;

        foreach (var cube in cubes)
        {
            if (cube == null) 
                continue;

            float distance = Vector3.Distance(cube.transform.position, explosionCenter);
            float force = cube.ExplosionForce * (1 - distance / cube.ExplosionRadius);

            cube.CubeRigidbody.AddExplosionForce(
                force,
                explosionCenter,
                cube.ExplosionRadius,
                0.5f,
                ForceMode.Impulse
            );
        }
    }

    private void ApplyExplosionToNearbyCubes(ExplodableCube sourceCube, Vector3 explosionCenter)
    {
        Collider[] nearbyCubes = Physics.OverlapSphere(explosionCenter, _explosionRadius);

        foreach (var collider in nearbyCubes)
        {
            if (collider.TryGetComponent(out ExplodableCube cube) && cube != sourceCube)
            {
                float distance = Vector3.Distance(explosionCenter, cube.transform.position);
                float force = _explosionForce * (1 - distance / _explosionRadius);

                cube.CubeRigidbody.AddExplosionForce(
                    force,
                    explosionCenter,
                    _explosionRadius,
                    0.5f,
                    ForceMode.Impulse
                );
            }
        }
    }
}