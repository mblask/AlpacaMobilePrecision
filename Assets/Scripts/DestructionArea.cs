using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionArea : MonoBehaviour
{
    private float _destructionSpeed = 60.0f;
    private float _destructionRadius = 6.0f;

    private void Update()
    {
        destructionImage();
        destructionField();
    }

    private void destructionImage()
    {
        Vector3 localScale = transform.localScale;
        localScale += new Vector3(1.0f, 1.0f, 0.0f) * _destructionSpeed * Time.deltaTime;
        transform.localScale = localScale;

        if (transform.localScale.x >= _destructionRadius)
            Destroy(gameObject);
    }

    private void destructionField()
    {
        List<Obstacle> obstaclesInArea = getObjectsInDestructionArea<Obstacle>();

        foreach (Obstacle obstacle in obstaclesInArea)
            obstacle.DestroyObstacle();
    }

    private List<T> getObjectsInDestructionArea<T>()
    {
        Vector3 position = transform.position;
        float radius = transform.localScale.x / 2.0f;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, radius);

        List<T> objectTList = new List<T>();

        if (colliders.Length != 0)
            foreach (Collider2D collider in colliders)
            {
                T objectT = collider.GetComponent<T>();
                if (objectT != null)
                    objectTList.Add(objectT);
            }

        return objectTList;
    }
}
