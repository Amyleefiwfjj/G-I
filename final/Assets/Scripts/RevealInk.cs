using UnityEngine;
public class RevealInk : MonoBehaviour
{
    public Material revealMaterial;
    public Transform playerCursor;
    public float revealRadius = 0.1f;

    void Update()
    {
        Vector3 uv = GetUVUnderCursor(); // 마우스 위치 → UV좌표 변환
        revealMaterial.SetVector("_RevealPos", new Vector4(uv.x, uv.y, 0, 0));
        revealMaterial.SetFloat("_RevealRadius", revealRadius);
    }

    Vector2 GetUVUnderCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
            return hit.textureCoord;
        return Vector2.zero;
    }
}
