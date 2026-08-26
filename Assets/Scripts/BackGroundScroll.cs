using UnityEngine;
using UnityEngine.UI;

public class LoopScrollImage : MonoBehaviour
{
    private const float MAX_OFFSET = 1f;
    private const string PROPERTY_NAME = "_MainTex";

    [SerializeField] private Vector2 _offsetSpeed;
    [SerializeField] private Material _material;

    private void Reset()
    {
        // コンポーネントがアタッチされたタイミングでマテリアルを取得する
        if (TryGetComponent(out Image image))
        {
            _material = image.material;
        }
    }

    private void Update()
    {
        if (_material != null)
        {
            var x = Mathf.Repeat(Time.time * _offsetSpeed.x, MAX_OFFSET);
            var y = Mathf.Repeat(Time.time * _offsetSpeed.y, MAX_OFFSET);
            var offset = new Vector2(x, y);
            _material.SetTextureOffset(PROPERTY_NAME, offset);
        }
    }

    private void OnDestroy()
    {
        // オブジェクトが破棄されるタイミングに位置をリセットする
        if (_material != null)
        {
            _material.SetTextureOffset(PROPERTY_NAME, Vector2.zero);
        }
    }
}