using Optimization.Caching;
using SimpleJSON;
using UnityEngine;

internal class Test_CSharp : MonoBehaviour
{
    private string m_InGameLog = string.Empty;

    private Vector2 m_Position = Vectors.v2zero;

    private void OnGUI()
    {
        this.m_Position = GUILayout.BeginScrollView(this.m_Position, new GUILayoutOption[0]);
        GUILayout.Label(this.m_InGameLog, new GUILayoutOption[0]);
        GUILayout.EndScrollView();
    }

    private void P(string aText)
    {
        this.m_InGameLog = this.m_InGameLog + aText + "\n";
    }

    private void Start()
    {
        this.Test();
        Debug.Log("Test results:\n" + this.m_InGameLog);
    }

    private void Test()
    {
        JSONNode jsonnode = JSONNode.Parse("{\"name\":\"test\", \"array\":[1,{\"data\":\"value\"}]}");
        jsonnode["array"][1]["Foo"] = "Bar";
        this.P("'nice formatted' string representation of the JSON tree:");
        this.P(jsonnode.ToString(string.Empty));
        this.P(string.Empty);
        this.P("'normal' string representation of the JSON tree:");
        this.P(jsonnode.ToString());
        this.P(string.Empty);
        this.P("content of member 'name':");
        this.P(jsonnode["name"]);
        this.P(string.Empty);
        this.P("content of member 'array':");
        this.P(jsonnode["array"].ToString(string.Empty));
        this.P(string.Empty);
        this.P("first element of member 'array': " + jsonnode["array"][0]);
        this.P(string.Empty);
        jsonnode["array"][0].AsInt = 10;
        this.P("value of the first element set to: " + jsonnode["array"][0]);
        this.P("The value of the first element as integer: " + jsonnode["array"][0].AsInt);
        this.P(string.Empty);
        this.P("N[\"array\"][1][\"data\"] == " + jsonnode["array"][1]["data"]);
        this.P(string.Empty);
        string text = jsonnode.SaveToBase64();
        string aText = jsonnode.SaveToCompressedBase64();
        this.P("Serialized to Base64 string:");
        this.P(text);
        this.P("Serialized to Base64 string (compressed):");
        this.P(aText);
        this.P(string.Empty);
        jsonnode = JSONNode.LoadFromBase64(text);
        this.P("Deserialized from Base64 string:");
        this.P(jsonnode.ToString());
        this.P(string.Empty);
        JSONClass jsonclass = new JSONClass();
        jsonclass["version"].AsInt = 5;
        jsonclass["author"]["name"] = "Bunny83";
        jsonclass["author"]["phone"] = "0123456789";
        jsonclass["data"][-1] = "First item\twith tab";
        jsonclass["data"][-1] = "Second item";
        jsonclass["data"][-1]["value"] = "class item";
        jsonclass["data"].Add("Forth item");
        jsonclass["data"][1] = jsonclass["data"][1] + " 'addition to the second item'";
        jsonclass.Add("version", "1.0");
        this.P("Second example:");
        this.P(jsonclass.ToString());
        this.P(string.Empty);
        this.P("I[\"data\"][0]            : " + jsonclass["data"][0]);
        this.P("I[\"data\"][0].ToString() : " + jsonclass["data"][0].ToString());
        this.P("I[\"data\"][0].Value      : " + jsonclass["data"][0].Value);
        this.P(jsonclass.ToString());
    }
}