using UnityEngine;
using UnityEngine.UI;

public class PhysicsInteraction : MonoBehaviour
{
    [SerializeField] private float maxGrabDistance = 20f; // Maximum raycast distance to check if there is a rigidbody on the way
    [SerializeField] private float maxEmptyDistance = 25f; // Maximum empty distance from the camera when moving shpere with mouse
    [SerializeField] private float throwForce = 10.0f;
    [SerializeField] private float emptyMoveSpeed = 0.1f; // move empty with mouse speed 
    [SerializeField] private Image image1;  // Can grab image
    [SerializeField] private Image image2;  // Is grabbing image
    public GameObject TakeText;

    private Rigidbody _hitRigidbody;
    private bool _isShooting;
    private RaycastHit _hitInfo;

    private GameObject _empty;
    private Rigidbody _emptyRb;

    [SerializeField] private Camera mainCamera;
    private void Start()
    {
        _empty = new GameObject();
        _empty.transform.parent = mainCamera.transform;
        _empty.AddComponent<Rigidbody>();
        _emptyRb = _empty.GetComponent<Rigidbody>();
        _emptyRb.isKinematic = true;
    }

    private void Update()
    {
        if (TakeText.activeSelf)
            TakeText.SetActive(false);

        ShootRaycast();

        if (Input.GetMouseButton(0) && _isShooting)
        {
            ApplyFixedConstraint();
            MoveEmptyWithMouse();

            if (Input.GetMouseButtonDown(1) && _hitRigidbody != null)
            {
                ThrowObject(_hitRigidbody);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            ResetShooting();
        }
    }

    private void ShootRaycast()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out _hitInfo, maxGrabDistance))
        {

            Item itemInfo = _hitInfo.collider.GetComponent<Item>();
            if (itemInfo != null)
            {
                TakeText.SetActive(true);
            }

            if (Input.GetMouseButtonDown(0))
            {
                _hitRigidbody = _hitInfo.collider.GetComponent<Rigidbody>();
                MoveEmpty(_hitInfo.point);
                _isShooting = true;
            }

            if (_isShooting == false)
            {
                DisplayImage(image1); // Display the first image if raycasting to a rigidbody                
            }
            else HideImage(image1);

            if (!_hitInfo.collider.GetComponent<Rigidbody>()) HideImage(image1);
        }
        else HideImage(image1);
    }

    private void MoveEmpty(Vector3 position)
    {
        _empty.transform.position = position;
        _empty.transform.parent = mainCamera.transform;
    }

    private void ApplyFixedConstraint()
    {
        if (_hitRigidbody && _empty)
        {
            FixedJoint spring = _hitRigidbody.gameObject.GetComponent<FixedJoint>();
            if (!spring)
            {
                spring = _hitRigidbody.gameObject.AddComponent<FixedJoint>();
                spring.autoConfigureConnectedAnchor = false;
                spring.connectedBody = _emptyRb;
                spring.connectedAnchor = Vector3.zero;

                spring.massScale = 1f;

                DisplayImage(image2);
            }
        }
    }

    private void MoveEmptyWithMouse()
    {
        float mouseY = Input.GetAxis("Mouse Y");
        if (_empty)
        {
            float scrollWheelFactor = Input.GetAxis("Mouse ScrollWheel") * 5;
            Vector3 emptyMovement = mainCamera.transform.forward * (emptyMoveSpeed * mouseY) + scrollWheelFactor * Camera.main.transform.forward;
            Vector3 emptyPos = _empty.transform.position + emptyMovement;

            // Clamp empty position relative to the camera
            float emptyDistanceFromCamera = Vector3.Distance(Camera.main.transform.position, emptyPos);
            if (emptyDistanceFromCamera <= maxEmptyDistance && emptyDistanceFromCamera > 0.4f)
            {
                _empty.transform.position = emptyPos;
            }
        }
    }

    private void ResetShooting()
    {
        if (_hitRigidbody)
        {
            Destroy(_hitRigidbody.GetComponent<FixedJoint>());
        }

        _isShooting = false;
        _hitRigidbody = null;
        HideImage(image2);
        HideImage(image1);
    }

    private void ThrowObject(Rigidbody objectToThrow)
    {
        Vector3 throwDirection = mainCamera.transform.forward;
        objectToThrow.AddForce(throwDirection * throwForce, ForceMode.Impulse);
        ResetShooting();
    }

    private void DisplayImage(Image img)
    {
        img.gameObject.SetActive(true);
    }

    private void HideImage(Image img)
    {
        img.gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (_empty != null)
        {
            Gizmos.DrawWireSphere(_empty.transform.position, 0.2f);
        }
    }
}