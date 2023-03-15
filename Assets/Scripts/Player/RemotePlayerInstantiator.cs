using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

class InstantiationTask
{
    public GameObject reference;
    public TaskCompletionSource<RemotePlayer> instantiated =  new TaskCompletionSource<RemotePlayer>();
}
public class RemotePlayerInstantiator : MonoBehaviour
{
    static ConcurrentQueue<InstantiationTask> toInstantiate =  new ConcurrentQueue<InstantiationTask>();
  
    void Update()
    {
        if(toInstantiate.TryDequeue(out var task))
        {
            var clone = GameObject.Instantiate(task.reference);
            clone.SetActive(true);
            var c = clone.AddComponent<RemotePlayer>();
            task.instantiated.TrySetResult(c);
        }
    }

    public static async Task<RemotePlayer> InstantiatePlayer(GameObject reference)
    {
        var task = new InstantiationTask();
        task.reference = reference;
        toInstantiate.Enqueue(task);
        return await task.instantiated.Task;
    }
}
