using UnityEngine;

public class MapPointer : MonoBehaviour
{
    [SerializeField] private float rayDistance = 10f;

    IMap mapComponent;
    IMap clickedMap;

    private LineRenderer _lineRenderer;

    [SerializeField] private Transform _hmd;
    [SerializeField] private Transform _lineOrigin;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        transform.SetParent(null);
    }

    void Update()
    {
        ShootRaycast();
    }

    public void OnClick()
    {
        if(mapComponent != null)
        {
            mapComponent.OnPointerClickBegin(this);
            clickedMap = mapComponent;
        }
    }
    public void OnClickEnd()
    {
        if(clickedMap != null)
        {
            clickedMap.OnPointerClickEnd(this);
        }
    }

    void ShootRaycast()
    {
        var dir = _lineOrigin.position - _hmd.position;
        Ray ray = new Ray(_lineOrigin.position, dir);
        RaycastHit[] hits = Physics.RaycastAll(ray, rayDistance);

        foreach (RaycastHit hit in hits)
        {
            mapComponent = hit.collider.GetComponentInParent<IMap>();
            if (mapComponent != null)
            {
               // Debug.Log($"Hit IMap component on {hit.collider.gameObject.name}");
                transform.position = hit.point;
                _lineRenderer.SetPosition(0,_lineOrigin.position);
                _lineRenderer.SetPosition(1,transform.position);
                mapComponent.OnRaycastHit(this);
                return;
            }
        }

        // Optional: Draw the ray in the scene view for debugging
      //  Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red);
    }
}