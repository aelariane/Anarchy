using UnityEngine;

namespace Anarchy.Skins.Maps
{
    internal class CitySkin : LevelSkin
    {
        private const int WallIndex = 10;
        private const int GroundIndex = 9;
        private const int GateIndex = 11;
        private const int HouseIndexStart = 1;

        public override int DataLength => 18;

        public CitySkin(string[] data) : base(new GameObject(), data)
        {
        }

        public override void Apply()
        {
            ApplySkybox();
            int index = 0;
            GameObject[] objects = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));
            foreach (GameObject go in objects)
            {
                if (go == null)
                {
                    continue;
                }

                if (go.name.Contains("Cube_") && go.transform.parent.gameObject.tag != "Player")
                {
                    int elementIntex = 0;
                    bool canBe = false;
                    switch (go.name.Substring(5, 3))
                    {
                        case "001":
                            elementIntex = GroundIndex;
                            canBe = true;
                            break;

                        case "006":
                        case "007":
                        case "015":
                        case "000":
                        case "labelWall":
                            elementIntex = WallIndex;
                            break;

                        case "002":
                            Vector3 pos = go.transform.position;
                            if (pos.x == 0f && pos.y == 0f && pos.z == 0f)
                            {
                                goto case "labelWall";
                            }

                            goto case "labelHouse";

                        case "005":
                        case "003":
                        case "labelHouse":
                            int houseIndex;
                            int.TryParse(random[index++].ToString(), out houseIndex);
                            if (houseIndex < 0 || houseIndex >= 8)
                            {
                                houseIndex = Random.Range(0, 8);
                            }
                            elementIntex = HouseIndexStart + houseIndex;
                            break;

                        case "019":
                        case "020":
                            elementIntex = GateIndex;
                            break;

                        default:
                            continue;
                    }
                    foreach (Renderer rend in go.GetComponentsInChildren<Renderer>())
                    {
                        TryApplyTexture(elements[elementIntex], rend, canBe);
                    }
                }
            }

            Minimap.TryRecaptureInstance();
        }
    }
}