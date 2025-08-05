using UnityEngine;

public class SnowBrush : MonoBehaviour
{
    private static readonly int DrawPosition = Shader.PropertyToID("_DrawPosition");
    private static readonly int DrawAngle = Shader.PropertyToID("_DrawAngle");
    private static readonly int RestoreAmount = Shader.PropertyToID("_RestoreAmount");
      
    [SerializeField] private CustomRenderTexture _snowMap;
    [SerializeField] private Material _showMaterial;
    [SerializeField, Range(0, int.MaxValue)] private float SecondsToRestore = 100;  
    [SerializeField] private GameObject[] Tires;
    [SerializeField] private GameObject[] Paws;
    
    private Camera _mainCamera;
    private int tireIndex;
    private float timeToRestoreOneTick;
    
    private void Start()
    {
        _snowMap.Initialize();
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        // Раскомментируйте одну из этих строчек, чтобы выбрать какие объекты будут копать снег
        DrawWithMouse();
        //DrawWithTires();
        //DrawWithPaws();

        // Считаем таймер до восстановления каждого пикселя текстуры на единичку 
        timeToRestoreOneTick -= Time.deltaTime;
        
        if (timeToRestoreOneTick < 0)
        {
            // Если в этот update мы хотим увеличить цвет всех пикселей карты высот на 1
            _showMaterial.SetFloat(RestoreAmount, 1 / 250f);
            timeToRestoreOneTick = SecondsToRestore / 250f;
        }
        else
        {
            // Если не хотим
            _showMaterial.SetFloat(RestoreAmount, 0);
        }
        
        // Обновляем текстуру вручную, можно это убрать и поставить Update Mode: Realtime
        _snowMap.Update();
    }
    
    private void DrawWithMouse()
    {
        if (!Input.GetMouseButton(0)) 
            return;
        
        var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector2 hitTextureCoord = hit.textureCoord;

            _showMaterial.SetVector(DrawPosition, hitTextureCoord);
            _showMaterial.SetFloat(DrawAngle, 45 * Mathf.Deg2Rad);
        }
    }

    private void DrawWithTires()
    {
        var tire = Tires[tireIndex++ % Tires.Length];
        var ray = new Ray(tire.transform.position, Vector3.down);
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector2 hitTextureCoord = hit.textureCoord;
            float angle = tire.transform.rotation.eulerAngles.y;

            _showMaterial.SetVector(DrawPosition, hitTextureCoord);
            _showMaterial.SetFloat(DrawAngle, angle * Mathf.Deg2Rad);
        }
    }

    private void DrawWithPaws()
    {
        var paw = Paws[tireIndex++ % Paws.Length];
        var ray = new Ray(paw.transform.position, Vector3.down);
        
        if (Physics.Raycast(ray, out RaycastHit hit, 0.32f))
        {
            Vector2 hitTextureCoord = hit.textureCoord;
            float angle = 180 + paw.transform.rotation.eulerAngles.y;

            _showMaterial.SetVector(DrawPosition, hitTextureCoord);
            _showMaterial.SetFloat(DrawAngle, angle * Mathf.Deg2Rad);
        }
    }
}