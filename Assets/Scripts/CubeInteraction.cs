using UnityEngine;

public class CubeInteraction : MonoBehaviour
{
    [SerializeField] private CubeSpawner _spawner;
    [SerializeField] private float _explosionForce = 20f;
    [SerializeField] private float _explosionRadius = 10f;
    [SerializeField] private float _upwardsModifier = 0.5f;

    public void HandleCubeClick(ExplodableCube cube)
    {
        if (cube == null) 
            return;

        Vector3 explosionCenter = cube.transform.position;

        if (cube.ShouldSplit())
        {
            ExplodableCube[] newCubes = _spawner.SpawnChildCubes(
                explosionCenter,
                cube.transform.localScale,
                cube.SplitChance,
                cube.CubeColor
            );
            ApplyExplosion(newCubes, explosionCenter, useCubePhysics: true);
        }
        else
        {
            Collider[] nearbyCubes = Physics.OverlapSphere(explosionCenter, _explosionRadius);
            ApplyExplosion(nearbyCubes, explosionCenter, sourceCube: cube);
        }

        Destroy(cube.gameObject);
    }

    private void ApplyExplosion(Collider[] colliders, Vector3 center, ExplodableCube sourceCube = null, bool useCubePhysics = false)
    {
        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out ExplodableCube cube) == false || cube == sourceCube)
                continue;

            float force = useCubePhysics ? cube.ExplosionForce : _explosionForce;
            float radius = useCubePhysics ? cube.ExplosionRadius : _explosionRadius;
            float distance = Vector3.Distance(center, cube.transform.position);

            cube.CubeRigidbody.AddExplosionForce(
                force * (1 - distance / radius),
                center,
                radius,
                _upwardsModifier,
                ForceMode.Impulse
            );
        }
    }

    private void ApplyExplosion(ExplodableCube[] cubes, Vector3 center, bool useCubePhysics = false)
    {
        if (cubes == null) 
            return;

        foreach (var cube in cubes)
        {
            if (cube == null) 
                continue;

            float force = useCubePhysics ? cube.ExplosionForce : _explosionForce;
            float radius = useCubePhysics ? cube.ExplosionRadius : _explosionRadius;
            float distance = Vector3.Distance(center, cube.transform.position);

            cube.CubeRigidbody.AddExplosionForce(
                force * (1 - distance / radius),
                center,
                radius,
                _upwardsModifier,
                ForceMode.Impulse
            );
        }
    }
}