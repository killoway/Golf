using UnityEngine.InputSystem;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(LineRenderer))]
public class Player : MonoBehaviour
{
    public AudioSource HoleAudioSource;
    public CinemachineVirtualCameraBase PlayerCamera;
    public Camera MainCamera;
    public TrackManager TrackManager;
    public float MaxForce = 1.5f;
    public float ForceAcceleration = 1.5f;
    public Color MinForceColor = Color.green;
    public Color MaxForceColor = Color.red;
    public UIManager UIManager;


    private Rigidbody _rigidbody;
    private LineRenderer _lineRenderer;
    private float _currentForce;
    private float _pingPongTime;
    private bool _canShoot;
    private float _timeInHole;
    private int _hitCount = 0;


    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _rigidbody = GetComponent<Rigidbody>();
        _lineRenderer = GetComponent<LineRenderer>();

        _lineRenderer.sortingOrder = 1;
        _lineRenderer.material = new Material (Shader.Find ("Sprites/Default"));

        _lineRenderer.enabled = false;
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

    }

    public void SpawnTo(Vector3 point)
    {
        _rigidbody.MovePosition(point);
    }

    private void Update()
    {
        _canShoot = _rigidbody.linearVelocity.magnitude < 0.1f;

        if (!_canShoot)
        {
            return;
        }

        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        ProcessOnMouseDown();
        ProcessOnMouseUp();
        ProcessOnMouseHold();
    }

    private void ProcessOnMouseDown()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            PlayerCamera.gameObject.SetActive(true);

            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.enabled = true;

            _currentForce = 0;
            _pingPongTime = 0;
        }
    }

    private void ProcessOnMouseUp()
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            PlayerCamera.gameObject.SetActive(true);
            _lineRenderer.enabled = false;

            var cameraForward = MainCamera.transform.forward;
            var forceDirection = new Vector3(cameraForward.x, 0, cameraForward.z) * _currentForce;

            _rigidbody.AddForce(forceDirection, ForceMode.Impulse);

            _hitCount++;
            UIManager.UpdateHitCounter(_hitCount);
        }
    }

    private void ProcessOnMouseHold()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            _pingPongTime += Time.deltaTime;

            _currentForce = Mathf.PingPong(ForceAcceleration * _pingPongTime, MaxForce);

            var cameraForward = MainCamera.transform.forward;
            var playerPosition = transform.position;
            var newPosition = playerPosition + new Vector3(
                cameraForward.x,
                0,
                cameraForward.z
            ) * _currentForce;

            _lineRenderer.SetPosition(1, newPosition);
            _lineRenderer.startColor = _lineRenderer.endColor = Color.Lerp(MinForceColor, MaxForceColor, _currentForce);
        }
    }

    private bool _hasPlayedHoleSound = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hole"))
        {
            _timeInHole = 0;

            // Проигрываем звук с объекта лунки, если там есть AudioSource
            AudioSource holeAudio = other.GetComponent<AudioSource>();
            if (holeAudio != null && !holeAudio.isPlaying)
            {
                holeAudio.Play();
            }
        }
        else if (other.CompareTag("FallZone"))
        {
            TrackManager.RespawnCurrentTrack();
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Hole"))
        {
            _timeInHole += Time.deltaTime;

            if (_timeInHole > 1.5f)
            {
                Hole hole = other.GetComponent<Hole>();
                if (hole != null)
                {
                    bool isLastTrack = hole.TrackIndex == TrackManager.Tracks.Length - 1;

                    bool isLastHole = false;
                    if (isLastTrack)
                    {
                        // Получаем последний трек
                        var lastTrack = TrackManager.Tracks[hole.TrackIndex];

                        if (lastTrack.Holes != null && lastTrack.Holes.Length > 0)
                        {
                            isLastHole = hole.HoleIndex == lastTrack.Holes.Length - 1;
                        }
                    }

                    if (isLastTrack && isLastHole)
                    {
                        LoadNextScene();
                    }
                    else
                    {
                        TrackManager.NextTrack();
                    }
                }
                else
                {
                    TrackManager.NextTrack();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hole"))
        {
            _timeInHole = 0;
        }
    }

   

    private void LoadNextScene()
    {
        // Можно грузить по индексу или по имени
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

}


