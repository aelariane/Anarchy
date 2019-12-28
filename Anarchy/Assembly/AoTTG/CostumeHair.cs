public class CostumeHair
{
    public static CostumeHair[] hairsF;
    public static CostumeHair[] hairsM;
    public string hair = string.Empty;

    public string hair_1 = string.Empty;
    public bool hasCloth;
    public int id;
    public string texture = string.Empty;

    public static void init()
    {
        CostumeHair.hairsM = new CostumeHair[11];
        CostumeHair.hairsM[0] = new CostumeHair();
        CostumeHair.hairsM[0].hair = (CostumeHair.hairsM[0].texture = "hair_boy1");
        CostumeHair.hairsM[1] = new CostumeHair();
        CostumeHair.hairsM[1].hair = (CostumeHair.hairsM[1].texture = "hair_boy2");
        CostumeHair.hairsM[2] = new CostumeHair();
        CostumeHair.hairsM[2].hair = (CostumeHair.hairsM[2].texture = "hair_boy3");
        CostumeHair.hairsM[3] = new CostumeHair();
        CostumeHair.hairsM[3].hair = (CostumeHair.hairsM[3].texture = "hair_boy4");
        CostumeHair.hairsM[4] = new CostumeHair();
        CostumeHair.hairsM[4].hair = (CostumeHair.hairsM[4].texture = "hair_eren");
        CostumeHair.hairsM[5] = new CostumeHair();
        CostumeHair.hairsM[5].hair = (CostumeHair.hairsM[5].texture = "hair_armin");
        CostumeHair.hairsM[6] = new CostumeHair();
        CostumeHair.hairsM[6].hair = (CostumeHair.hairsM[6].texture = "hair_jean");
        CostumeHair.hairsM[7] = new CostumeHair();
        CostumeHair.hairsM[7].hair = (CostumeHair.hairsM[7].texture = "hair_levi");
        CostumeHair.hairsM[8] = new CostumeHair();
        CostumeHair.hairsM[8].hair = (CostumeHair.hairsM[8].texture = "hair_marco");
        CostumeHair.hairsM[9] = new CostumeHair();
        CostumeHair.hairsM[9].hair = (CostumeHair.hairsM[9].texture = "hair_mike");
        CostumeHair.hairsM[10] = new CostumeHair();
        CostumeHair.hairsM[10].hair = (CostumeHair.hairsM[10].texture = string.Empty);
        for (int i = 0; i <= 10; i++)
        {
            CostumeHair.hairsM[i].id = i;
        }
        CostumeHair.hairsF = new CostumeHair[11];
        CostumeHair.hairsF[0] = new CostumeHair();
        CostumeHair.hairsF[0].hair = (CostumeHair.hairsF[0].texture = "hair_girl1");
        CostumeHair.hairsF[1] = new CostumeHair();
        CostumeHair.hairsF[1].hair = (CostumeHair.hairsF[1].texture = "hair_girl2");
        CostumeHair.hairsF[2] = new CostumeHair();
        CostumeHair.hairsF[2].hair = (CostumeHair.hairsF[2].texture = "hair_girl3");
        CostumeHair.hairsF[3] = new CostumeHair();
        CostumeHair.hairsF[3].hair = (CostumeHair.hairsF[3].texture = "hair_girl4");
        CostumeHair.hairsF[4] = new CostumeHair();
        CostumeHair.hairsF[4].hair = (CostumeHair.hairsF[4].texture = "hair_girl5");
        CostumeHair.hairsF[4].hasCloth = true;
        CostumeHair.hairsF[4].hair_1 = "hair_girl5_cloth";
        CostumeHair.hairsF[5] = new CostumeHair();
        CostumeHair.hairsF[5].hair = (CostumeHair.hairsF[5].texture = "hair_annie");
        CostumeHair.hairsF[6] = new CostumeHair();
        CostumeHair.hairsF[6].hair = (CostumeHair.hairsF[6].texture = "hair_hanji");
        CostumeHair.hairsF[6].hasCloth = true;
        CostumeHair.hairsF[6].hair_1 = "hair_hanji_cloth";
        CostumeHair.hairsF[7] = new CostumeHair();
        CostumeHair.hairsF[7].hair = (CostumeHair.hairsF[7].texture = "hair_mikasa");
        CostumeHair.hairsF[8] = new CostumeHair();
        CostumeHair.hairsF[8].hair = (CostumeHair.hairsF[8].texture = "hair_petra");
        CostumeHair.hairsF[9] = new CostumeHair();
        CostumeHair.hairsF[9].hair = (CostumeHair.hairsF[9].texture = "hair_rico");
        CostumeHair.hairsF[10] = new CostumeHair();
        CostumeHair.hairsF[10].hair = (CostumeHair.hairsF[10].texture = "hair_sasha");
        CostumeHair.hairsF[10].hasCloth = true;
        CostumeHair.hairsF[10].hair_1 = "hair_sasha_cloth";
        for (int j = 0; j <= 10; j++)
        {
            CostumeHair.hairsF[j].id = j;
        }
    }
}