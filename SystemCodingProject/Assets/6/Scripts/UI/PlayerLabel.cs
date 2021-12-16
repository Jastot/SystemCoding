using System.Linq;
using Characters;
using UnityEngine;
using UnityEngine.Networking;

namespace UI
{
    public class PlayerLabel : MonoBehaviour
    {
        public void DrawLabel(Camera camera)
        {
            if (camera == null)
            {
                return;
            }

            var style = new GUIStyle();
            style.normal.background = Texture2D.redTexture;
            style.normal.textColor = Color.blue;
            var objects = ClientScene.objects;
            for (int i = 0; i < objects.Count; i++)
            {
                var obj = objects.ElementAt(i).Value;
                var position = camera.WorldToScreenPoint(obj.transform.position);
                var collider = obj.GetComponent<Collider>();
                if (collider != null && camera.Visible(collider) && obj.transform != transform)
                {
                    var player = obj.gameObject.GetComponent<ShipController>();
                    if (player)
                        GUI.Label(new Rect(new Vector2(position.x,Screen.height - position.y), new Vector2(10, name.Length * 10.5f )),
                            player.PlayerName, style);
                    
                    else
                        GUI.Label(new Rect(new Vector2(position.x,Screen.height - position.y), new Vector2(10, name.Length * 10.5f )),
                            obj.name, style);
                }
            }
        }
    }
}
