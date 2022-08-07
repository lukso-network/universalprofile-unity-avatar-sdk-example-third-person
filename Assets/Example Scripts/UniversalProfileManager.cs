using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using StarterAssets;
using TMPro;
using UnityEngine;
using UniversalProfileSDK;
using UniversalProfileSDK.Avatars;

namespace UniversalProfileAvatars
{
    /// <summary>
    /// Example Universal Profile manager class. Handles instantiating avatars and the likes. 
    /// </summary>
    public class UniversalProfileManager : MonoBehaviour
    {
        public GameObject playerObject;
        
        [Header("Temporary references")]
        public CinemachineVirtualCamera cinemachineCamera;
        public UICanvasControllerInput UICanvasControllerInput;
        public UPMenuHandler MenuHandler;

        public UniversalProfile loadedProfile;
        Transform playerTransform;
        Transform avatarTransform;
        Animator playerAnimator;

        public TextMeshPro avatarLoadingPercentage;
        GameObject avatarLoadingPercentageParentObject;

        void Start()
        {
            playerTransform = GameObject.FindWithTag("Player").transform;
            avatarTransform = playerTransform.transform.Find("Avatar").transform;
            playerAnimator = playerTransform.GetComponent<Animator>();
            avatarLoadingPercentageParentObject = avatarLoadingPercentage.transform.parent.gameObject;
        }

        /// <summary>
        /// Filter avatars to our current platform. Currently unused.
        /// </summary>
        /// <param name="avatars">Avatars to filter</param>
        /// <param name="platform">Platform to filter to</param>
        /// <returns>Filtered list of avatars</returns>
        IEnumerable<UPAvatar> FilterAvatarsToPlatform(UPAvatar[] avatars, RuntimePlatform platform)
        {
            return avatars.Where(av => av.UPAvatarIsForCurrentPlatform());
        }

        /// <summary>
        /// Load avatar at index. Called from a UI dropdown.
        /// </summary>
        /// <param name="index"></param>
        public void LoadAvatarIndex(int index)
        {
            UPAvatar[] avatars = loadedProfile.Avatars;
            if(index >= loadedProfile.Avatars.Length)
            {
                LogAndSetStatus($"Invalid avatar index. Cache count is {avatars.Length}, index was {index}", LogType.Error);
                return;
            }

            UPAvatar avatar = avatars[index];
            
            LogAndSetStatus($"Loading avatar of type {avatar.FileType} with hash {avatar.Hash}");
            
            avatarLoadingPercentageParentObject.SetActive(true);
            StartCoroutine(AvatarCache.LoadAvatar(avatars[index], OnAvatarLoadedInstantiatePlayer, OnAvatarLoadFailed, OnAvatarProgressChanged));
        }

        /// <summary>
        /// Callback that updates loading text
        /// </summary>
        /// <param name="percent"></param>
        void OnAvatarProgressChanged(float percent)
        {
            avatarLoadingPercentage.text = $"{(int)(percent * 100)}%";
        }

        /// <summary>
        /// Callback when avatar loading succeeds. Destroys old avatar and instantiates a new one.
        /// </summary>
        /// <param name="prefab"></param>
        void OnAvatarLoadedInstantiatePlayer(GameObject prefab)
        {
            LogAndSetStatus("Instantiating avatar...");
            
            avatarLoadingPercentageParentObject.SetActive(false);
            Destroy(avatarTransform.gameObject);

            avatarTransform = Instantiate(prefab, avatarTransform).transform;
            avatarTransform.name = "Avatar";
            if(avatarTransform.TryGetComponent(out Animator anim))
            {
                playerAnimator.avatar = anim.avatar;
                playerAnimator.applyRootMotion = anim.applyRootMotion;
                Destroy(anim);
                
                // Potential fix to some avatars not animating, but it doesn't seem to work.
                playerAnimator.Rebind();
            }

            avatarTransform.SetParent(playerTransform, false);
            LogAndSetStatus("Done!");
        }

        /// <summary>
        /// Avatar failed loading callback. Hides the loading percentage display, logs to console and status label.
        /// </summary>
        /// <param name="ex"></param>
        void OnAvatarLoadFailed(Exception ex)
        {
            avatarLoadingPercentageParentObject.SetActive(false);
            LogAndSetStatus(ex.Message, LogType.Exception);
        }

        /// <summary>
        /// Log a message to console as well as the status label in the UI
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="logType">Message type</param>
        void LogAndSetStatus(string message, LogType logType = LogType.Log)
        {
            if(MenuHandler)
                MenuHandler.StatusText = message;
            switch(logType)
            {
                case LogType.Assert:
                case LogType.Exception:
                case LogType.Error:
                    Debug.LogError(message);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(message);
                    break;
                case LogType.Log:
                default:
                    Debug.Log(message);
                    break;
            }
        }
    }
}