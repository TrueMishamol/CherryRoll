﻿using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour, IItemParent {


    public static Player LocalInstance { get; private set; }
    public static event EventHandler OnAnyPlayerSpawned;

    public static void ResetStaticData() {
        OnAnyPlayerSpawned = null;
    }

    //^ Interaction
    public event EventHandler<OnSelectedInteractableObjectChangedEventArgs> OnSelectedInteractableObjectChanged;
    public event EventHandler OnInteract;
    public class OnSelectedInteractableObjectChangedEventArgs : EventArgs {
        public IInteractableObject selectedInteractableObject;
    }

    [SerializeField] private LayerMask interactLayerMask;
    private Vector3 lastInteractDir;
    private IInteractableObject selectedInteractableObject;

    [SerializeField] private Transform itemHolder;
    private Item item;

    //^ Cinemachine
    private PlayerCameraFollow playerCameraFollow;
    private GameObject cameraFollow;
    private string cameraName = "CameraFollow";


    private void Start() {
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;

        if (IsClient && IsOwner) {
            //^ Cinemachine
            cameraFollow = GameObject.Find(cameraName);
            cameraFollow.TryGetComponent<PlayerCameraFollow>(out playerCameraFollow);
            playerCameraFollow.FollowPlayer(transform);
        }
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1) {
        selectedInteractableObject = null;
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e) {

        if (selectedInteractableObject != null) {
            selectedInteractableObject.Interact(this);
            OnInteract?.Invoke(this, EventArgs.Empty);
        }
    }

    public override void OnNetworkSpawn() {
        if (IsOwner) {
            LocalInstance = this;
        }

        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);

        //^ Handle Disconnect
        if (IsServer) {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId) {
        if (clientId == OwnerClientId && HasItem()) {
            Item.DestroyItem(GetItem());
        }
    }

    private void Update() {
        if (!IsOwner) return;
        HandleInteractions();
    }

    private void HandleInteractions() {

        Vector2 inputVector = GameInput.Instance.GetMovementVectorSmoothed();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != Vector3.zero) {
            lastInteractDir = moveDir;
        }

        float interactDistance = .55f;
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, interactLayerMask)) {
            if (raycastHit.transform.TryGetComponent(out IInteractableObject interactableObject)) {
                //^ Has BaseInteractableObject
                if (interactableObject != selectedInteractableObject) {
                    SetSelectedInteractableObject(interactableObject);
                }
            } else {
                //^ Has no BaseInteractableObject
                if (selectedInteractableObject != null) {
                    SetSelectedInteractableObject(null);
                }
            }
        } else {
            //^ Raycast do not hit interactLayerMask
            if (selectedInteractableObject != null) {
                SetSelectedInteractableObject(null);
            }
        }
    }

    private void SetSelectedInteractableObject(IInteractableObject selectedInteractableObject) {
        this.selectedInteractableObject = selectedInteractableObject;

        OnSelectedInteractableObjectChanged?.Invoke(this, new OnSelectedInteractableObjectChangedEventArgs {
            selectedInteractableObject = selectedInteractableObject
        });
    }

    public void RefreshItem() {
        if (!IsServer) return;

        RefreshItemClientRpc(item.GetNetworkObject());
    }

    [ClientRpc]
    private void RefreshItemClientRpc(NetworkObjectReference itemNetworkObjectReference) {
        itemNetworkObjectReference.TryGet(out NetworkObject itemNetworkObject);
        item = itemNetworkObject.GetComponent<Item>();
    }

    public Transform GetItemFollowTransform() {
        return itemHolder;
    }

    public void SetItem(Item item) {
        this.item = item;
    }

    public Item GetItem() {
        return item;
    }

    public void ClearItem() {
        item = null;
    }

    public bool HasItem() {
        return item != null;
    }

    public NetworkObject GetNetworkObject() {
        return NetworkObject;
    }
}

