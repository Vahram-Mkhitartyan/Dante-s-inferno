using UnityEngine;

public static class PersistentServiceLifecycle
{
    public static void PersistIndependently(Component service)
    {
        if (service == null)
            return;

        Transform serviceTransform = service.transform;
        if (serviceTransform.parent != null)
            serviceTransform.SetParent(null, true);

        Object.DontDestroyOnLoad(service.gameObject);
    }
}
