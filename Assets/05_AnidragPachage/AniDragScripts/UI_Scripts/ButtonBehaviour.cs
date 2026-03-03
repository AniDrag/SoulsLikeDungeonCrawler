using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ButtonBehaviour : MonoBehaviour
{
    public enum ButtonActivationResult
    {
        OnlySound,
        DissableThisGameObject,
        DissableChildObject,
        DissableGrandChildObject,
        DissableParent,
        DissableGrandParent,
        DissableTargetObject,

    }
    [Header(
        "--------------------------------------------" + 
        "\n Button Settings \n" + 
        "--------------------------------------------"
    )]
    [SerializeField] public ButtonActivationResult resolveType = ButtonActivationResult.OnlySound;
    [Tooltip("Target is a gameobject that will be dissabled on click")]
    [SerializeField] private GameObject target;
    [SerializeField] private AudioClip audioClip;
    [Tooltip("Tis is an audio mixer Group that should be asigned to buttons")]
    [SerializeField] private AudioMixerGroup audioAutput;
    [SerializeField] private bool showChildIndexes;
    [SerializeField] private int childIndex;
    [SerializeField] private int grandchildIndex;


    private AudioSource _audioSource;
    private Button _button;

    // Here to asigne vereything we can on a reset. Possible check for on Enable
    private void OnValidate()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        if (_audioSource.clip == null && GetComponent<AudioSource>() != null) _audioSource.clip = audioClip;
        else { Debug.LogWarning($"No Auido clip found on {transform.name}!!!"); }
        if (_audioSource.outputAudioMixerGroup == null && audioAutput != null)
        {
            _audioSource.outputAudioMixerGroup = audioAutput;
        }
        else
        {
            Debug.LogWarning($"No Auido Output / Audio Mixer Group assigned on {transform.name}!!!");
        }
        if (_button == null)
        {
            _button = GetComponent<Button>();
            if (_button == null)
            {
                Debug.LogWarning($"No Button component found on {transform.name}!!!");
                return;
            }
            
        }
    }
    private void OnEnable()
    {
        _button.onClick.AddListener(OnCaseDecision);
    }
    private void OnDisable()
    {
        if (_button != null)
            _button.onClick.RemoveListener(OnCaseDecision);
    }

    private void OnCaseDecision()
    {
        switch (resolveType)
        {
            case ButtonActivationResult.OnlySound: BTN_Pressed(); break;
            case ButtonActivationResult.DissableThisGameObject: BTN_DissableThis(); break;
            case ButtonActivationResult.DissableChildObject: BTN_DissableChild(); break;
            case ButtonActivationResult.DissableGrandChildObject: BTN_DissableGrandChild(); break;
            case ButtonActivationResult.DissableParent: BTN_DissableParent(); break;
            case ButtonActivationResult.DissableGrandParent: BTN_DissableGrandParent(); break;
            case ButtonActivationResult.DissableTargetObject: BTN_DissableTargetObject(); break;
        }

    }
    /// <summary>
    /// Trigger Audio while keeping everything safe.
    /// Will Dissable target if it is not null
    /// </summary>
    public void BTN_Pressed()
    {
        if (GetComponent<AudioSource>() == null) { Debug.LogWarning("!! No audio provided !!"); return; }
        else if (_audioSource == null)
        {
            Debug.LogWarning("!! No audio Source detected !!");
            _audioSource = GetComponent<AudioSource>();

            if (_audioSource == null) { Debug.LogError("!! No audio Source component detected !!"); return; }
           
        }
        else if (_audioSource.clip == null && audioClip != null) _audioSource.clip = audioClip;

        if (target != null) { target.SetActive(false); }

        _audioSource?.Stop();
        _audioSource?.Play();
        //enable and trigger sound
    }
    /// <summary>
    /// Hides / dissables the button and plays sound.
    /// </summary>
    public void BTN_DissableThis()
    {
        target = transform.gameObject;
        BTN_Pressed();
        target.SetActive(false);
    }
    /// <summary>
    /// Hides / dissables a selected game object that is in the target sloot, and plays sound.
    /// </summary>
    public void BTN_DissableTargetObject()
    {
        if (target == null)
        {
            Debug.Log("This button has no assigned target: " + gameObject.name);
        }
        BTN_Pressed();
        target.SetActive(false);
    }

    /// <summary>
    /// Hides / Dissables parent of this transform, and plays sound.
    /// </summary>
    public void BTN_DissableParent()
    {
        target = transform.parent.gameObject;
        BTN_Pressed();
        target.SetActive(false);
    }
    /// <summary>
    /// Hides / Dissables parent of of this transforms parent, this dissabeling a grandparent. Plays sound.
    /// </summary>
    public void BTN_DissableGrandParent()
    {
        target = transform.parent.parent.gameObject;
        BTN_Pressed();
        target.SetActive(false);
    }
    /// <summary>
    /// Hides / Dissables child of this transform.
    /// Contains a safety check if the child index is correct.
    /// Plays sound.
    /// </summary>
    /// <param name="childIndex"> what child to dissable Or if Target slot is filled dissables target</param>
    public void BTN_DissableChild()
    {
        if (transform.childCount - 1 < childIndex)
        {
            Debug.LogError($"child index on{transform.name} is out of bounds");
            return;
        }
        else if (target == null)
            target = transform.GetChild(childIndex).gameObject;
        BTN_Pressed();
        target.SetActive(false);
    }
    /// <summary>
    /// Same princible as Dissable child.
    /// Contais safty checks.
    /// Plays sound.
    /// </summary>
    /// <param name="childIndex"></param>
    /// <param name="grandchildIndex"></param>
    public void BTN_DissableGrandChild()
    {
        if (transform.childCount - 1 < childIndex)
        {
            Debug.LogError($"child index on{transform.name} is out of bounds");
            return;
        }
        else if (transform.GetChild(childIndex).childCount - 1 < grandchildIndex)
        {
            Debug.LogError($"Grand Child index on{transform.GetChild(childIndex).name} is out of bounds");
            return;
        }
        else if (target == null)
            target = transform.GetChild(childIndex).GetChild(grandchildIndex).gameObject;
        BTN_Pressed();
        target.SetActive(false);
    }
    public class SelectChild
    {
        public int child;
        public int grandchild;
    }
}


