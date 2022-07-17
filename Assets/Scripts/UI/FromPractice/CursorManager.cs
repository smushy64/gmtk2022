using UnityEngine;

public static class CursorManager
{
    static Texture2D normal {
        get {
            if( _normal == null )
                _normal = Resources.Load<Texture2D>( "cursors/normal" );
            return _normal;
        }
    }
    static Texture2D _normal;
    static Texture2D link {
        get {
            if( _link == null )
                _link = Resources.Load<Texture2D>( "cursors/link" );
            return _link;
        }
    }
    static Texture2D _link;

    static Vector2 normalHotspot = Vector2.zero;
    static Vector2 linkHotspot = new Vector2( 6f, 0f );

    public enum CursorType {
        Normal,
        Link
    }

    public static CursorType CurrentCursor { get; private set; } = CursorType.Normal;

    public static void SetCursor( CursorType cursor ) {
        CurrentCursor = cursor;
        Texture2D texture;
        Vector2 hotspot;
        switch(CurrentCursor) {
            default: case CursorType.Normal:
                texture = normal;
                hotspot = normalHotspot;
                break;
            case CursorType.Link:
                texture = link;
                hotspot = linkHotspot;
                break;
        }

        Cursor.SetCursor(texture, hotspot, CursorMode.Auto);
    }

}
