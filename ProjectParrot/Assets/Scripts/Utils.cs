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

    public static string FormatByteCount(int byteAmount)
    {
		string[] sizeSuffixes = { "b", "kb", "mb", "gb" };
		int index = 0;
		int originalByteAmount = byteAmount;

		while (byteAmount >= 1024 && index < sizeSuffixes.Length - 1)
		{
			byteAmount /= 1024;
			++index;
		}

		string originalAmount = originalByteAmount != byteAmount ? $" ({originalByteAmount}b)" : "";
		return $"{byteAmount:0.#}{sizeSuffixes[index]}{originalAmount}";
	}

}
