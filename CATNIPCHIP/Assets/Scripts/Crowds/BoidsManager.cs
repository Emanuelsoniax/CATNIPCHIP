using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsManager : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private uint _boidCount;
    [SerializeField] private Boid _boidPrefab;
    [SerializeField] private float _randomSpawnRange;

    [SerializeField] private BoidParameters _parameters = new BoidParameters() {
        maxVelocity = 5.0f,
        seperationDistance = 0.5f, 
        alignmentDistance = 2.0f,
        cohesionDistance = 2.0f,
        seperationWeight = 1.0f,
        alignmentWeight = 1.5f,
        cohesionWeight = 1.0f,
        targetWeight = 1.0f
    };


    private Boid[] _boids;

    [Serializable]
    public struct BoidParameters
    {
        public float maxVelocity;

        [Header("Distances")]
        public float seperationDistance;
        public float alignmentDistance;
        public float cohesionDistance;

        [Header("Weights")]
        public float seperationWeight;
        public float alignmentWeight;
        public float cohesionWeight;
        public float targetWeight;
    }


    private void Start()
    {
        _boids = new Boid[_boidCount];

        for (int i = 0; i < _boidCount; i++)
        {
            Boid boid = Instantiate(_boidPrefab, transform);
            boid.name = "Boid " + i;

            boid.transform.position = transform.position + new Vector3(
                (UnityEngine.Random.value - .5f) * _randomSpawnRange,
                (UnityEngine.Random.value - .5f) * _randomSpawnRange,
                (UnityEngine.Random.value - .5f) * _randomSpawnRange
                );

            _boids[i] = boid;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        foreach(Boid boid in _boids)
        {
            boid.UpdateBoid(_parameters, _target, _boids);


        }
    }
}
