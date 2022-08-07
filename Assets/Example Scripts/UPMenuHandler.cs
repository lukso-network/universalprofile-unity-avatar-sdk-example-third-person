using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UniversalProfileAvatars;
using UniversalProfileSDK;
using UniversalProfileSDK.Avatars;

/// <summary>
/// Handler class for various UI functions related to the Universal Profile menu.
/// </summary>
public class UPMenuHandler : MonoBehaviour
{
    [SerializeField] GameObject universalProfilePanel;
    [SerializeField] TMP_InputField universalProfileAddressField;
    [SerializeField] TMP_InputField localBundlePathField;
    [SerializeField] TMP_Text statusField;
    [SerializeField] TMP_Dropdown avatarsDropdown;
    [SerializeField] TMP_Text helpLabel;
    [SerializeField] TMP_Text availableAvatarsLabel;

    /// <summary>
    /// Example profiles. Might be outdated.
    /// </summary>
    public string[] ExampleProfiles = 
    {
        "0x107B4183180A3e0AaEd2b6266e5b7387e3B9cBf8",
        "0x92e3c9712e3A851d1226eD6e52783EB6884352F6",
        
    };

    public bool ShowAddressPanel
    {
        get 
        {
            if(universalProfilePanel)
                return universalProfilePanel.activeSelf;
            return false;
        }
        set
        {
            if(universalProfilePanel)
                universalProfilePanel.SetActive(value);
        }
    }

    public string StatusText
    {
        set
        {
            if(statusField != null)
                statusField.text = value;
        }
    }

    public UniversalProfileManager manager;

    void Start()
    {
        if(!universalProfileAddressField)
            Debug.LogError("Universal profile address field is not assigned");

        if(avatarsDropdown)
            avatarsDropdown.interactable = false;
        
        #if UNITY_ANDROID || UNITY_IOS
        HideHelp();
        #endif
    }

    /// <summary>
    /// Called from the UP menu UI.
    /// </summary>
    public void OnPressedLoadProfile()
    {
        string address = universalProfileAddressField.text;
        if(address.StartsWith("0x"))
            StartCoroutine(AvatarSDK.GetProfileRemote(address, OnProfileLoaded, OnProfileLoadFailed));
        else
            StartCoroutine(AvatarSDK.GetProfileLocal(address, OnProfileLoaded, OnProfileLoadFailed));
    }

    /// <summary>
    /// Callback when loading profile fails
    /// </summary>
    /// <param name="ex">Exception thrown</param>
    void OnProfileLoadFailed(Exception ex)
    {
        string error = ex.Message;
        statusField.text = error;
        Debug.LogException(ex);
    }

    /// <summary>
    /// Callback called when loading profile succeeds
    /// </summary>
    /// <param name="profile">Loaded profile</param>
    void OnProfileLoaded(UniversalProfile profile)
    {
        manager.loadedProfile = profile;
        SetAvatarsDropdownNames(profile.Avatars.Select(av => av.Hash));
        
        if(availableAvatarsLabel)
            availableAvatarsLabel.text = $"Available avatars for profile `{profile.Name}`";
    }

    /// <summary>
    /// Sets the dropdown options to a IEnumerable of avatar names
    /// </summary>
    /// <param name="avatarNames"></param>
    void SetAvatarsDropdownNames(IEnumerable<string> avatarNames)
    {
        var options = avatarNames?.Select(av => new TMP_Dropdown.OptionData(av));
        avatarsDropdown.options = options != null ? new List<TMP_Dropdown.OptionData>(options) : null;
        avatarsDropdown.interactable = avatarsDropdown.options != null && avatarsDropdown.options.Count > 0;
        if(avatarsDropdown.interactable)
            SelectAvatar();
    }

    /// <summary>
    /// Load avatar at currently selected index in the dropdown
    /// </summary>
    public void SelectAvatar()
    {
        manager.LoadAvatarIndex(avatarsDropdown.value);
    }

    /// <summary>
    /// Hides the "Press esc to open menu" banner
    /// </summary>
    public void HideHelp()
    {
        helpLabel.gameObject.SetActive(false);
    }

    /// <summary>
    /// Inputs one of the example profiles into the UP address field
    /// </summary>
    /// <param name="index">Example index</param>
    public void InputExampleProfile(int index)
    {
        universalProfileAddressField.text = ExampleProfiles[Mathf.Clamp(index, 0, ExampleProfiles.Length - 1)];
    }
}
