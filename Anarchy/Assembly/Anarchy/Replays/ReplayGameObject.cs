using UnityEngine;

namespace Anarchy.Replays
{
    public class ReplayGameObject
    {
        public Animation Animation { get; }
        public int Id { get; }
        public bool IsObservableObject { get; set; }
        public GameObject SourceObject { get; }
        public Transform Transform { get; }

        public ReplayGameObject(int id, GameObject source)
        {
            Id = id;
            SourceObject = source;
            Transform = source.transform ?? null;
            Animation = source.animation ?? null;
        }

        public static bool operator !=(ReplayGameObject go1, ReplayGameObject go2)
        {
            return go1.Id != go2.Id;
        }

        public static bool operator ==(ReplayGameObject go1, ReplayGameObject go2)
        {
            return go1.Id == go2.Id;
        }

        public override bool Equals(object obj)
        {
            ReplayGameObject go = obj as ReplayGameObject;
            return go != null && go.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}