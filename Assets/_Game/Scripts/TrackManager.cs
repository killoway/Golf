using UnityEngine;
using UnityEngine.SceneManagement;

public class TrackManager : MonoBehaviour
{
    public Track[] Tracks;
    public Player Player;

    private int _currentTrack;

    void Start()
    {
        if (Player && Tracks.Length > 0)
        {
            _currentTrack = 0;
            Player.SpawnTo(Tracks[_currentTrack].SpawnPoint.position);
        }
    }

    public void NextTrack()
    {
        // Если еще остались треки — перейти на следующий
        if (_currentTrack < Tracks.Length - 1)
        {
            _currentTrack++;
            Player.SpawnTo(Tracks[_currentTrack].SpawnPoint.position);
        }
        else
        {
            // Последний трек завершён — перейти на следующую сцену
            Debug.Log("Последний трек. Загружаем следующую сцену...");

            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

            // Если следующая сцена существует в Build Settings
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.Log("Следующая сцена не найдена. Это был финал.");
            }
        }
    }

    public void RespawnCurrentTrack()
    {
        var rb = Player.GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Player.SpawnTo(Tracks[_currentTrack].SpawnPoint.position);
    }
}
