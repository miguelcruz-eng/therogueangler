using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera[] allVirtualCameras;
    private CinemachineVirtualCamera currentCamera;
    private CinemachineFramingTransposer framingTransposer;

    [Header("Y Damping Settings for Player Jump/Fall:")]
    [SerializeField] private float panAmount = 0.1f;
    [SerializeField] private float panTime = 0.2f;
    public float playerFallSpeedTheshold = -10f;
    public bool isLerpingYDamp;
    public bool hasLerpingYDamp;

    private float normalYDamp;
    public static CameraManager Instance {get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        for (int i = 0; i < allVirtualCameras.Length; i++)
        {
            if (allVirtualCameras[i]. enabled)
            {
                currentCamera = allVirtualCameras[i];

                framingTransposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            }
        }

        normalYDamp = framingTransposer.m_YDamping;
    }

    private void Start()
    {
        for (int i = 0; i < allVirtualCameras.Length; i++)
        {
            allVirtualCameras[i].Follow = PlayerController.Instance.transform;
        }
    }

    public void SwapCamera(CinemachineVirtualCamera _newCam)
    {
        currentCamera.enabled = false;

        currentCamera = _newCam;

        currentCamera.enabled = true;
    }

    public IEnumerator LerpYDamping(bool _isPlayerFalling)
    {
        isLerpingYDamp = true;
        float _startYDamp = framingTransposer.m_YDamping;
        float _endYDamp = 0;

        if (_isPlayerFalling)
        {
            _endYDamp = panAmount;
            hasLerpingYDamp = true;
        }
        else
        {
            _endYDamp = normalYDamp;
        }

        float _timer = 0;
        while (_timer < panTime)
        {
            _timer += Time.deltaTime;
            float _lerpedPanAmount = Mathf.Lerp(_startYDamp, _endYDamp, (_timer / panTime));
            framingTransposer.m_YDamping = _lerpedPanAmount;
            yield return null;
        }
        isLerpingYDamp = false;
    }
}
