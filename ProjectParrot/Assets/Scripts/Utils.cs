using UnityEngine;

public static partial class Utils 
{
    public static T GetComponentOrSearchInParentAndChilderen<T>(this GameObject obj) where T : Component
    {
        if (obj.TryGetComponent(out T component))
        {
            return component;
        }
        component = obj.GetComponentInParent<T>();
		if (component != null)
		{
			return component;
		}
        component = obj.GetComponentInChildren<T>();
		if (component != null)
		{
			return component;
		}

        component = obj.transform.root.GetComponentInChildren<T>();

        return component;
	}
    public static T GetComponentOrSearchInParentAndChilderen<T>(this MonoBehaviour mb) where T : Component
    {
        return GetComponentOrSearchInParentAndChilderen<T>(mb.gameObject);
    }

}
