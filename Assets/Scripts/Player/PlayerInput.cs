using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private LayerMask shooterBlockLayerMask;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main; 
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, shooterBlockLayerMask))
            {
                if (hit.collider.gameObject)
                {
                    ShooterBlock shooterBlock = hit.collider.gameObject.GetComponent<ShooterBlock>();
                    inventory.TryDeployShooterBlock(shooterBlock);
                }
            }
        }
    }
}
