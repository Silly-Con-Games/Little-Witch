using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Add tweening
public class AimingGfxController : Tweenable
{
    public bool isOn = false;

    private SpriteRenderer gfx;

    private float minZ;
    [SerializeField] private Vector2 minMaxScale;

    private Color defaultColor;
    [SerializeField] private Color notReadyColor;

    //public PlayerController playerController;

    private void Start()
    {
        gfx = GetComponent<SpriteRenderer>();
        minZ = transform.position.z;
        defaultColor = gfx.color;

        Hide();
    }

    void Update()
    {
        // move and stretch w mouse cursor ?
        // move on z, scale on y
        // only update if visible
/*        if (isOn)
        {
            if (!playerController) return;

            float mousePlayerDist = Vector3.Distance(playerController.transform.position, playerController.mouseWorldPosition);

        }*/
    }

    public void Show(bool ready)
    {
        isOn = true;
        if(gfx)
            gfx.enabled = true;        // change this to tweenning later
        SetReady(ready);
    }
    public void Hide()
    {
        isOn = false;
        if(gfx)
            gfx.enabled = false;       // change this to tweenning later
    }

    public void SetReady(bool ready)
    {
        if(gfx)
            gfx.color = ready ? defaultColor : notReadyColor;  // change this to tweenning later
    }



}
