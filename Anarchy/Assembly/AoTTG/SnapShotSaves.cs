using UnityEngine;

public class SnapShotSaves
{
    private static int currentIndex;

    private static int[] dmg;

    private static Texture2D[] img;

    private static int index;

    private static bool inited;

    private static int maxIndex;

    public static void addIMG(Texture2D t, int d)
    {
        SnapShotSaves.init();
        SnapShotSaves.img[SnapShotSaves.index] = t;
        SnapShotSaves.dmg[SnapShotSaves.index] = d;
        SnapShotSaves.currentIndex = SnapShotSaves.index;
        SnapShotSaves.index++;
        if (SnapShotSaves.index >= SnapShotSaves.img.Length)
        {
            SnapShotSaves.index = 0;
        }
        SnapShotSaves.maxIndex = Mathf.Max(SnapShotSaves.index, SnapShotSaves.maxIndex);
    }

    public static int getCurrentDMG()
    {
        if (SnapShotSaves.maxIndex == 0)
        {
            return 0;
        }
        return SnapShotSaves.dmg[SnapShotSaves.currentIndex];
    }

    public static Texture2D getCurrentIMG()
    {
        if (SnapShotSaves.maxIndex == 0)
        {
            return null;
        }
        return SnapShotSaves.img[SnapShotSaves.currentIndex];
    }

    public static int getCurrentIndex()
    {
        return SnapShotSaves.currentIndex;
    }

    public static int getLength()
    {
        return SnapShotSaves.maxIndex;
    }

    public static int getMaxIndex()
    {
        return SnapShotSaves.maxIndex;
    }

    public static Texture2D GetNextIMG()
    {
        SnapShotSaves.currentIndex++;
        if (SnapShotSaves.currentIndex >= SnapShotSaves.maxIndex)
        {
            SnapShotSaves.currentIndex = 0;
        }
        return SnapShotSaves.getCurrentIMG();
    }

    public static Texture2D GetPrevIMG()
    {
        SnapShotSaves.currentIndex--;
        if (SnapShotSaves.currentIndex < 0)
        {
            SnapShotSaves.currentIndex = SnapShotSaves.maxIndex - 1;
        }
        return SnapShotSaves.getCurrentIMG();
    }

    public static void init()
    {
        if (SnapShotSaves.inited)
        {
            return;
        }
        SnapShotSaves.inited = true;
        SnapShotSaves.index = 0;
        SnapShotSaves.maxIndex = 0;
        SnapShotSaves.img = new Texture2D[99];
        SnapShotSaves.dmg = new int[99];
    }
}