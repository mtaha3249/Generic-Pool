# Generic-Pool
---
## How to use it?
* Add Pool Manager to GameObject.
* The Editor will look like this.

<img width="367" alt="Screenshot 2021-11-08 at 10 52 08 PM" src="https://user-images.githubusercontent.com/38559882/140792858-bd349949-faf4-47dd-b0cd-1d2e3ed8828f.png">

* Check or Uncheck Don't Destroy on Load.
* Add Element to the list.
* Give initial amount.
* Use the following code to spawn. (Note: Prefab should be same in the prefabToSpawn as given in the Pool Manager)
```cSharp
GameObject spawned = PoolManager.Instance.Spawn(prefabToSpawn, Position, Rotation, Parent);
```

* To Despawn use the following code.
```cSharp
PoolManager.Instance.Despawn(gameObjectToDeSpawn, cleanHierarchy);
```

* I have Added delegates called on item Spawned and item Despawned.
```cSharp
void Start()
{
  PoolManager.Instance.OnSpawned += Func;
  PoolManager.Instance.OnDespawned += Func1;
}

void Func(GameObject itemSpawned)
{
  // do something
}

void Func1(GameObject itemDespawned)
{
  // do something
}

void Destroy()
{
  PoolManager.Instance.OnSpawned -= Func;
  PoolManager.Instance.OnDespawned -= Func1;
}
```
