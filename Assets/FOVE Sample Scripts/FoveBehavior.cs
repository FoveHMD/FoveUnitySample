using Fove.Unity;
using UnityEngine;

public class FOVEBehavior: MonoBehaviour
{
    private static FoveInterface foveInterface;
    public static FoveInterface FoveInterface
    {
        get
        {
            if (foveInterface == null)
            {
                // returns the first FoveInterface found here but you should adapt this code to your game
                // especially in the case where you could have no or several FoveInterface in your game
                foveInterface = FindObjectOfType<FoveInterface>();
            }

            return foveInterface;
        }
    }
}
