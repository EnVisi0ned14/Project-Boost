using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

    [SerializeField] float rcsThrust = 250f;
    [SerializeField] float rcsFlight = 100f;
    [SerializeField] float LevelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip LevelLoadSound;

    [SerializeField] ParticleSystem engineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem successParticles;

    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Alive, Dieing, Transcending };

    State state = State.Alive;

	// Use this for initialization
	void Start () {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        RespondToRotateInput();
        RespondToThrustInput();
	}

    void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartDeathSequence()
    {
        state = State.Dieing;
        audioSource.Stop();
        audioSource.PlayOneShot(deathSound);
        deathParticles.Play();
        Invoke("LoadLevelTwo", .3f);
    }

    private void StartSuccessSequence()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(LevelLoadSound);
        state = State.Transcending;
        successParticles.Play();
        Invoke("LoadLevelTwo", LevelLoadDelay);
    }

    private void LoadLevelOne()
    {
        SceneManager.LoadScene(0);
    }
    private void LoadLevelTwo()
    {
        SceneManager.LoadScene(1);
    }

    private void RespondToThrustInput()
    {
        ApplyThrust();

    }

    private void ApplyThrust()
    {
        if (state == State.Alive)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                rigidBody.AddRelativeForce(Vector3.up * rcsFlight);
                if (!audioSource.isPlaying)
                {
                    audioSource.PlayOneShot(mainEngine);
                    engineParticles.Play();
                }
            }
            else
            {
                audioSource.Stop();
                engineParticles.Stop();
            }
        }
    }

    private void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true;

        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if(state == State.Alive || state == State.Transcending)
        {
            if (Input.GetKey(KeyCode.A))
            {
                transform.Rotate(Vector3.forward * rotationThisFrame);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.Rotate(-Vector3.forward * rotationThisFrame);
            }

            rigidBody.freezeRotation = false;
        }
    }

}
