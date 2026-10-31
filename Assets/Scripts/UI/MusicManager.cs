using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Events")]
    [SerializeField] private VoidEventChannel _pauseToggledEvent;
    [SerializeField] private VoidEventChannel _victoryEvent;
    [SerializeField] private VoidEventChannel _defeatEvent;

    [Header("Music Tracks")]
    [SerializeField] private AudioClip _mainMenuMusic;
    [SerializeField] private AudioClip _gameMusic;

    [Header("Settings")]
    [SerializeField] private float _pausedVolumeMultiplier = 0.3f;
    [SerializeField] private float _fadeSpeed = 1.5f;

    private AudioSource _audioSource;
    private float _defaultVolume;
    private float _targetVolume;
    private bool _isPaused = false;
    private bool _isGameOver = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _audioSource = GetComponent<AudioSource>();
        _defaultVolume = _audioSource.volume;
        _targetVolume = _defaultVolume;
    }

    private void OnEnable()
    {
        if (_pauseToggledEvent != null) _pauseToggledEvent.OnEventRaised += HandlePauseState;
        if (_victoryEvent != null) _victoryEvent.OnEventRaised += StopMusic;
        if (_defeatEvent != null) _defeatEvent.OnEventRaised += StopMusic;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        if (_pauseToggledEvent != null) _pauseToggledEvent.OnEventRaised -= HandlePauseState;
        if (_victoryEvent != null) _victoryEvent.OnEventRaised -= StopMusic;
        if (_defeatEvent != null) _defeatEvent.OnEventRaised -= StopMusic;

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _isGameOver = false;
        _isPaused = false;
        _targetVolume = _defaultVolume;

        if (scene.buildIndex == 0)
        {
            if (_mainMenuMusic != null) SwitchTrack(_mainMenuMusic);
        }
        else
        {
            if (_gameMusic != null) SwitchTrack(_gameMusic);
        }
    }

    private void SwitchTrack(AudioClip clip)
    {
        if (_audioSource.clip == clip && _audioSource.isPlaying) return;

        _audioSource.clip = clip;
        _audioSource.Play();
        _audioSource.volume = 0f;
    }

    private void HandlePauseState()
    {
        if (_isGameOver) return;
        _isPaused = !_isPaused;
        _targetVolume = _isPaused ? _defaultVolume * _pausedVolumeMultiplier : _defaultVolume;
    }

    private void StopMusic()
    {
        _isGameOver = true;
        _targetVolume = 0f;
    }

    private void Update()
    {
        if (!Mathf.Approximately(_audioSource.volume, _targetVolume))
        {
            _audioSource.volume = Mathf.MoveTowards(
                _audioSource.volume,
                _targetVolume,
                _fadeSpeed * Time.unscaledDeltaTime
            );

            if (_audioSource.volume <= 0.01f && _targetVolume == 0f)
            {
                _audioSource.volume = 0f;
                _audioSource.Stop();
            }
        }
    }

    public void PlayMusic(AudioClip newClip)
    {
        if (newClip == null) return;

        _isGameOver = false;
        _isPaused = false;
        _targetVolume = _defaultVolume;

        SwitchTrack(newClip);
    }
}